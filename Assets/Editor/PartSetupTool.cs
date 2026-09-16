using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PartSetupTool
{
    [MenuItem("Tools/Setup Disassemblable Part")]
    static void SetupSelectedPart()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("No object selected.");
            return;
        }

        // 1. Add Mesh Colliders to every mesh leaf found inside this object
        MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshFilters)
        {
            GameObject leaf = mf.gameObject;
            if (leaf.GetComponent<MeshCollider>() == null)
            {
                MeshCollider mc = leaf.AddComponent<MeshCollider>();
                mc.convex = true;
            }
        }

        // 2. Add Rigidbody to the top-level object
        Rigidbody rb = selected.GetComponent<Rigidbody>();
        if (rb == null) rb = selected.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // 3. Add XR Simple Interactable
        XRSimpleInteractable simple = selected.GetComponent<XRSimpleInteractable>();
        if (simple == null) simple = selected.AddComponent<XRSimpleInteractable>();

        // 4. Add XR Grab Interactable, disabled
        XRGrabInteractable grab = selected.GetComponent<XRGrabInteractable>();
        if (grab == null) grab = selected.AddComponent<XRGrabInteractable>();
        grab.enabled = false;

        // 5. Add DisassemblablePart script
        DisassemblablePart part = selected.GetComponent<DisassemblablePart>();
        if (part == null) part = selected.AddComponent<DisassemblablePart>();

        part.displayName = selected.name;

        // 6. Auto-wire shared scene references by name
        GameObject engineCenterObj = GameObject.Find("EngineCenter");
        if (engineCenterObj != null) part.engineCenter = engineCenterObj.transform;

        GameObject feedbackTextObj = GameObject.Find("FeedbackText");
        if (feedbackTextObj != null)
            part.feedbackText = feedbackTextObj.GetComponent<TMPro.TextMeshProUGUI>();

        GameObject hoverTextObj = GameObject.Find("HoverNameText");
        if (hoverTextObj != null)
            part.hoverNameText = hoverTextObj.GetComponent<TMPro.TextMeshProUGUI>();

        GameObject hoverCanvasObj = GameObject.Find("HoverNameCanvas");
        if (hoverCanvasObj != null) part.hoverNameCanvas = hoverCanvasObj;

        EditorUtility.SetDirty(selected);
        Debug.Log("Setup complete for: " + selected.name + " (" + meshFilters.Length + " colliders added)");
    }
}