using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PartSetupTool
{
    [MenuItem("Tools/Setup Disassemblable Part")]
    static void SetupSelectedParts()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No objects selected.");
            return;
        }

        foreach (GameObject selected in selectedObjects)
        {
            SetupOnePart(selected);
        }

        Debug.Log("Setup complete for " + selectedObjects.Length + " part(s).");
    }

    static void SetupOnePart(GameObject selected)
    {
        MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>(true);
        foreach (MeshFilter mf in meshFilters)
        {
            GameObject leaf = mf.gameObject;
            if (leaf.GetComponent<MeshCollider>() == null)
            {
                MeshCollider mc = leaf.AddComponent<MeshCollider>();
                mc.convex = true;
            }
        }

        Rigidbody rb = selected.GetComponent<Rigidbody>();
        if (rb == null) rb = selected.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        XRSimpleInteractable simple = selected.GetComponent<XRSimpleInteractable>();
        if (simple == null) simple = selected.AddComponent<XRSimpleInteractable>();

        XRGrabInteractable grab = selected.GetComponent<XRGrabInteractable>();
        if (grab == null) grab = selected.AddComponent<XRGrabInteractable>();
        grab.enabled = false;

        DisassemblablePart part = selected.GetComponent<DisassemblablePart>();
        if (part == null) part = selected.AddComponent<DisassemblablePart>();
        part.displayName = selected.name;

        GameObject engineCenterObj = GameObject.Find("EngineCenter");
        if (engineCenterObj != null) part.engineCenter = engineCenterObj.transform;

        EditorUtility.SetDirty(selected);
    }
}