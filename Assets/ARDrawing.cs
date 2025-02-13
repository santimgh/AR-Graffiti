using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Required for UI elements
using UnityEngine.XR.ARFoundation;

public class ARDrawing : MonoBehaviour
{
    public Camera arCamera;  
    public float brushSize = 0.01f;  // Default brush size
    public Color brushColor = Color.red;  // Default brush color

    private LineRenderer currentLine; 
    private List<Vector3> positions = new List<Vector3>(); 
    private InputAction touchInput; 
    private bool isDrawing = false; 

    private List<GameObject> strokes = new List<GameObject>(); // Stores all strokes

    [Header("UI Elements")]
    public Slider brushSizeSlider;  // Reference to the UI slider
    public Image brushPreview; // Reference to the preview UI element
     public Button undoButton;  // Reference to Undo button

    void Awake()
    {
        // Initialize touch input
        touchInput = new InputAction("Touch", binding: "<Pointer>/position");
        touchInput.Enable();

        // Attach slider event listener
        brushSizeSlider.onValueChanged.AddListener(UpdateBrushSize);

         // Attach the Undo function to the button
        if (undoButton != null)
        {
            undoButton.onClick.AddListener(UndoLastStroke);
        }
    }

    void Update()
    {
        if (Application.isEditor)
        {
            SimulateTouchWithMouse();
        }
        else
        {
            HandleTouchInput();
        }
        
        UpdateBrushPreview(); // Update preview each frame
    }


    /// Updates the brush preview UI element.
    void UpdateBrushPreview()
    {
        if (brushPreview != null)
        {
            brushPreview.color = brushColor; // Change color dynamically

            // Convert world space size to UI space by using a multiplier
            float uiSize = brushSize * 2000;  // Adjust multiplier to better match stroke size
            brushPreview.rectTransform.sizeDelta = new Vector2(uiSize, uiSize);
        }
    }


    /// Updates the brush size when the slider is changed.
    void UpdateBrushSize(float newSize)
    {
        brushSize = newSize;
        UpdateBrushPreview(); // Ensure preview updates immediately
    }


    void HandleTouchInput()
    {
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame && !IsPointerOverUI())
        {
            StartNewStroke();
            isDrawing = true;
        }
        else if (touch.press.isPressed && isDrawing)
        {
            AddPointToStroke();
        }
        else if (touch.press.wasReleasedThisFrame)
        {
            isDrawing = false;
            currentLine = null;
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void SimulateTouchWithMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
        {
            StartNewStroke();
            isDrawing = true;
        }
        else if (Mouse.current.leftButton.isPressed && isDrawing)
        {
            AddPointToStroke();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDrawing = false;
            currentLine = null;
        }
    }

    void StartNewStroke()
    {
        GameObject newStroke = new GameObject("Stroke");
        currentLine = newStroke.AddComponent<LineRenderer>();

        currentLine.startWidth = brushSize;
        currentLine.endWidth = brushSize;
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startColor = brushColor;
        currentLine.endColor = brushColor;
        currentLine.positionCount = 0;

        positions.Clear();

        // Store the stroke in the list
        strokes.Add(newStroke);
    }

    /// Removes the last stroke from the scene.
    public void UndoLastStroke()
    {
        if (strokes.Count > 0)
        {
            GameObject lastStroke = strokes[strokes.Count - 1];
            strokes.RemoveAt(strokes.Count - 1); // Remove from list
            Destroy(lastStroke); // Destroy the GameObject
        }
    }


    void AddPointToStroke()
    {
        Vector3 worldPosition = GetWorldPosition();

        if (positions.Count == 0 || Vector3.Distance(worldPosition, positions[positions.Count - 1]) > 0.01f)
        {
            positions.Add(worldPosition);
            currentLine.positionCount = positions.Count;
            currentLine.SetPositions(positions.ToArray());
        }
    }

    Vector3 GetWorldPosition()
    {
        Vector2 screenPos = touchInput.ReadValue<Vector2>();
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        return ray.origin + ray.direction * 0.5f;
    }


    /// Allows the brush color to be updated from the color picker.
    public void ChangeBrushColor(Color newColor)
    {
        brushColor = newColor;
    }
}
