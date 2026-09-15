using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class DisassemblablePart : MonoBehaviour
{
    [Header("Part Info")]
    public string displayName = "Part";

    [Header("References")]
    public Transform engineCenter;       // drag the root engine object here
    public TextMeshProUGUI feedbackText; // drag your FeedbackText UI object here
    public TextMeshProUGUI nameTag;      // leave empty for now, we'll wire this up later

    [Header("Animation Settings")]
    public float popDistance = 0.3f;
    public float popDuration = 0.4f;

    private Rigidbody rb;
    private XRSimpleInteractable simpleInteractable;
    private XRGrabInteractable grabInteractable;
    private bool isDetached = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
            grabInteractable.enabled = false;

        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            simpleInteractable.hoverExited.AddListener(OnHoverExited);
            simpleInteractable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (nameTag != null)
        {
            nameTag.text = displayName;
            nameTag.gameObject.SetActive(true);
        }
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (nameTag != null)
            nameTag.gameObject.SetActive(false);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isDetached)
            StartCoroutine(DetachRoutine());
    }

    IEnumerator DetachRoutine()
    {
        isDetached = true;

        if (feedbackText != null)
            feedbackText.text = "Removed: " + displayName;

        Vector3 startPos = transform.position;
        Vector3 direction = (transform.position - engineCenter.position).normalized;
        Vector3 endPos = startPos + direction * popDistance;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t / popDuration);
            yield return null;
        }
        transform.position = endPos;

        if (rb != null)
            rb.isKinematic = false;

        if (simpleInteractable != null)
            simpleInteractable.enabled = false;
        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }
}