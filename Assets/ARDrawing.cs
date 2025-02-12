using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

public class ARDrawing : MonoBehaviour
{
    public Camera arCamera;  // Reference to the AR Camera
    public float brushSize = 0.01f;  // Size of the brush stroke
    public Color brushColor = Color.red;  // Default brush color
    
    private LineRenderer currentLine; // The current stroke being drawn
    private List<Vector3> positions = new List<Vector3>(); // Stores stroke positions
    private InputAction touchInput; // Handles touch/mouse input
    private bool isDrawing = false; // Flag to check if the user is drawing

    void Awake()
    {
        // Initialize the InputAction for touch input
        touchInput = new InputAction("Touch", binding: "<Pointer>/position");
        touchInput.Enable();
    }

    void Update()
    {
        // Handle input differently for Unity Editor and mobile devices
        if (Application.isEditor)
        {
            SimulateTouchWithMouse(); // Simulates touch using mouse input in the Unity Editor
        }
        else
        {
            HandleTouchInput(); // Handles actual touch input on mobile
        }
    }

    /// <summary>
    /// Handles real touch input for mobile devices.
    /// Ensures proper stroke separation by resetting currentLine when touch is released.
    /// </summary>
    void HandleTouchInput()
    {
        var touch = Touchscreen.current.primaryTouch; // Get primary touch data

        if (touch.press.wasPressedThisFrame && !IsPointerOverUI()) // If the user just touched the screen
        {
            StartNewStroke(); // Start a new stroke
            isDrawing = true;
        }
        else if (touch.press.isPressed && isDrawing) // If the user is still touching the screen
        {
            AddPointToStroke(); // Add points to the current stroke
        }
        else if (touch.press.wasReleasedThisFrame) // If the user lifted their finger
        {
            isDrawing = false;
            currentLine = null; // Reset the currentLine so new strokes are separate
        }
    }

    /// <summary>
    /// Prevents drawing if the touch/click is over a UI element.
    /// </summary>
    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// Simulates touch input using the mouse in the Unity Editor.
    /// Allows testing in Play Mode before deploying to a mobile device.
    /// </summary>
    void SimulateTouchWithMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI()) // If the user clicks the mouse
        {
            StartNewStroke(); // Start a new stroke
            isDrawing = true;
        }
        else if (Mouse.current.leftButton.isPressed && isDrawing) // If the user holds the mouse button
        {
            AddPointToStroke(); // Continue drawing
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame) // If the user releases the mouse button
        {
            isDrawing = false;
            currentLine = null; // Reset the currentLine so new strokes are separate
        }
    }

    /// <summary>
    /// Starts a new stroke by creating a new GameObject with a LineRenderer.
    /// </summary>
    void StartNewStroke()
    {
        GameObject newStroke = new GameObject("Stroke"); // Create a new empty GameObject for the stroke
        currentLine = newStroke.AddComponent<LineRenderer>(); // Add a LineRenderer to handle drawing

        // Set the properties of the LineRenderer
        currentLine.startWidth = brushSize;
        currentLine.endWidth = brushSize;
        currentLine.material = new Material(Shader.Find("Sprites/Default")); // Assign a default material
        currentLine.startColor = brushColor;
        currentLine.endColor = brushColor;
        currentLine.positionCount = 0; // No points initially

        positions.Clear(); // Clear the list of positions to start fresh
    }

    /// <summary>
    /// Adds a new point to the current stroke.
    /// Prevents unnecessary points by only adding if a minimum distance is met.
    /// </summary>
    void AddPointToStroke()
    {
        Vector3 worldPosition = GetWorldPosition(); // Convert touch position to world coordinates

        // Ensure we only add a new point if it's far enough from the last point
        if (positions.Count == 0 || Vector3.Distance(worldPosition, positions[positions.Count - 1]) > 0.01f)
        {
            positions.Add(worldPosition); // Add new position to the list
            currentLine.positionCount = positions.Count; // Update LineRenderer count
            currentLine.SetPositions(positions.ToArray()); // Apply new positions to the stroke
        }
    }

    /// <summary>
    /// Converts screen touch/mouse position into world space coordinates.
    /// Ensures the strokes appear in 3D AR space.
    /// </summary>
    /// <returns>World position of the touch/mouse input.</returns>
    Vector3 GetWorldPosition()
    {
        Vector2 screenPos = touchInput.ReadValue<Vector2>(); // Get touch/mouse position on the screen
        Ray ray = arCamera.ScreenPointToRay(screenPos); // Cast a ray from the AR camera into the world
        return ray.origin + ray.direction * 0.5f; // Return a position 0.5m in front of the camera
    }

    /// <summary>
    /// Allows the brush color to be updated from the color picker.
    /// </summary>
    public void ChangeBrushColor(Color newColor)
    {
        brushColor = newColor;
    }
}
