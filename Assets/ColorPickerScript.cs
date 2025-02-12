using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPickerScript : MonoBehaviour, IPointerClickHandler
{
    public Image imageToPick; // Assign the UI color picker image
    public ARDrawing drawingScript; // Reference to the ARDrawing script

    public void OnPointerClick(PointerEventData eventData)
    {
        Color selectedColor = Pick(eventData.position, imageToPick);
        drawingScript.ChangeBrushColor(selectedColor); // Apply color to brush
        Debug.Log("Selected Color: " + selectedColor);
    }

    /// Converts the clicked point into texture coordinates and returns the color.
    Color Pick(Vector2 screenPoint, Image imageToPick)
    {
        if (imageToPick == null || imageToPick.sprite == null || imageToPick.sprite.texture == null)
        {
            Debug.LogError("Image or texture is null!");
            return Color.white;
        }

        // Convert screen position to local position in the UI Image
        RectTransform rectTransform = imageToPick.rectTransform;
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out localPoint))
        {
            Debug.LogError("Failed to convert screen point to local point.");
            return Color.white;
        }

        // Convert local position to texture coordinates
        Texture2D texture = imageToPick.sprite.texture;
        Rect rect = imageToPick.sprite.rect;

        // Normalize local point relative to rectTransform
        float normalizedX = Mathf.InverseLerp(-rectTransform.rect.width / 2, rectTransform.rect.width / 2, localPoint.x);
        float normalizedY = Mathf.InverseLerp(-rectTransform.rect.height / 2, rectTransform.rect.height / 2, localPoint.y);

        // Map to texture coordinates
        int pixelX = Mathf.Clamp(Mathf.RoundToInt(rect.x + (rect.width * normalizedX)), 0, texture.width - 1);
        int pixelY = Mathf.Clamp(Mathf.RoundToInt(rect.y + (rect.height * normalizedY)), 0, texture.height - 1);

        // Get the pixel color
        return texture.GetPixel(pixelX, pixelY);
    }
}
