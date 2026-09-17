using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DisassemblablePart : MonoBehaviour
{
    [Header("Part Info")]
    public string displayName = "Part";

    [Header("References")]
    public Transform engineCenter;

    [Header("Animation Settings")]
    public float popDistance = 0.3f;
    public float popDuration = 0.4f;
    public Vector3 manualPopDirection = Vector3.zero;

    [Header("Return Settings")]
    public Key returnKey = Key.N;

    private Rigidbody rb;
    private XRSimpleInteractable simpleInteractable;
    private XRGrabInteractable grabInteractable;
    private XRInteractionManager interactionManager;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private bool isDetached = false;
    private bool isAnimating = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
            interactionManager = grabInteractable.interactionManager;
        }

        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            simpleInteractable.hoverExited.AddListener(OnHoverExited);
            simpleInteractable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void Update()
    {
        if (isDetached && !isAnimating && Keyboard.current[returnKey].wasPressedThisFrame)
        {
            StartCoroutine(ReturnRoutine());
        }
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        UIManager.Instance.ShowHoverName(displayName, transform.position + Vector3.up * 0.3f);
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        UIManager.Instance.HideHoverName();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isDetached && !isAnimating)
            StartCoroutine(DetachRoutine());
    }

    IEnumerator DetachRoutine()
    {
        isAnimating = true;
        isDetached = true;

        UIManager.Instance.ShowFeedback("Removed: " + displayName);

        Vector3 startPos = transform.position;
        Vector3 direction = manualPopDirection != Vector3.zero
            ? manualPopDirection.normalized
            : (transform.position - engineCenter.position).normalized;
        Vector3 endPos = startPos + direction * popDistance;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t / popDuration);
            yield return null;
        }
        transform.position = endPos;

        if (simpleInteractable != null) simpleInteractable.enabled = false;
        if (grabInteractable != null) grabInteractable.enabled = true;

        isAnimating = false;
    }

    IEnumerator ReturnRoutine()
    {
        isAnimating = true;

        // Force-release from hand if currently held
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            var interactorsCopy = new System.Collections.Generic.List<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor>(grabInteractable.interactorsSelecting);
            foreach (var interactor in interactorsCopy)
            {
                interactionManager.SelectExit(interactor, grabInteractable);
            }
        }

        if (grabInteractable != null) grabInteractable.enabled = false;

        UIManager.Instance.ShowFeedback("Attached: " + displayName);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 targetWorldPos = transform.parent.TransformPoint(originalLocalPosition);
        Quaternion targetWorldRot = transform.parent.rotation * originalLocalRotation;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetWorldPos, t / popDuration);
            transform.rotation = Quaternion.Slerp(startRot, targetWorldRot, t / popDuration);
            yield return null;
        }
        transform.position = targetWorldPos;
        transform.rotation = targetWorldRot;

        if (simpleInteractable != null) simpleInteractable.enabled = true;

        isDetached = false;
        isAnimating = false;
    }
}