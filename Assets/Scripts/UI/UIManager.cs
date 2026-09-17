using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI hoverNameText;
    public GameObject hoverNameCanvas;

    void Awake()
    {
        Instance = this;
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.transform.parent.gameObject.SetActive(true);
        }
    }

    public void ShowHoverName(string name, Vector3 worldPosition)
    {
        if (hoverNameText != null) hoverNameText.text = name;
        if (hoverNameCanvas != null)
        {
            hoverNameCanvas.transform.position = worldPosition;
            hoverNameCanvas.SetActive(true);
        }
    }

    public void HideHoverName()
    {
        if (hoverNameCanvas != null)
            hoverNameCanvas.SetActive(false);
    }
}