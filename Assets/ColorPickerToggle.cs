using UnityEngine;
using UnityEngine.UI;

public class ColorPickerToggle : MonoBehaviour
{
    public GameObject colorPickerPanel; // Assign the ColorPickerPanel
    public Button openButton;  // Assign OpenColorPicker button
    public Button closeButton; // Assign CloseButton inside the panel

    void Start()
    {
        // Ensure the panel starts as hidden
        colorPickerPanel.SetActive(false);

        // Attach button click events
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
    }

    void OpenPanel()
    {
        colorPickerPanel.SetActive(true); // Show the color picker panel
    }

    void ClosePanel()
    {
        colorPickerPanel.SetActive(false); // Hide the color picker panel
    }
}
