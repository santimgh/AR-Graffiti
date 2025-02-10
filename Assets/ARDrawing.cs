using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class ARDrawing : MonoBehaviour
{
    public Camera arCamera;
    public float brushSize = 0.01f;
    public Color brushColor = Color.red;
    
    private LineRenderer currentLine;
    private List<Vector3> positions = new List<Vector3>();

    private InputAction touchInput;

    void Awake()
    {
        touchInput = new InputAction("Touch", binding: "<Pointer>/position");
        touchInput.Enable();
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
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (currentLine == null)
                StartNewStroke();

            AddPointToStroke();
        }
    }

    void SimulateTouchWithMouse()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (currentLine == null)
                StartNewStroke();

            AddPointToStroke();
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
        Vector2 screenPos = touchInput.ReadValue<Vector2>(); // Get touch/mouse position
        Ray ray = arCamera.ScreenPointToRay(screenPos);
        return ray.origin + ray.direction * 0.5f; // Adjust the drawing distance
    }
}
