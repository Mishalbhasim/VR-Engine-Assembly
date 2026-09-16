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
    public Transform engineCenter;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI hoverNameText;
    public GameObject hoverNameCanvas;   // drag HoverNameCanvas here
    public float nameTagHeightOffset = 0.15f;

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
        {
            grabInteractable.enabled = false;
            grabInteractable.selectEntered.AddListener(OnFirstGrabbed);
            grabInteractable.hoverEntered.AddListener(OnHoverEntered);
            grabInteractable.hoverExited.AddListener(OnHoverExited);
        }

        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
            simpleInteractable.hoverExited.AddListener(OnHoverExited);
            simpleInteractable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (hoverNameText != null) hoverNameText.text = displayName;
        if (hoverNameCanvas != null)
        {
            hoverNameCanvas.transform.position = transform.position + Vector3.up * 0.3f + (Camera.main.transform.position - transform.position).normalized * 0.15f;
            hoverNameCanvas.SetActive(true);
        }
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (hoverNameCanvas != null)
            hoverNameCanvas.SetActive(false);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("SELECT ENTERED on " + displayName);
        if (!isDetached)
            StartCoroutine(DetachRoutine());
    }

    void OnFirstGrabbed(SelectEnterEventArgs args)
    {
        if (rb != null && rb.isKinematic)
            rb.isKinematic = false;
    }

    IEnumerator DetachRoutine()
    {
        isDetached = true;

        if (feedbackText != null)
        {
            feedbackText.text = "Removed: " + displayName;
            feedbackText.transform.parent.gameObject.SetActive(true); // activates FeedbackBackground
        }

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