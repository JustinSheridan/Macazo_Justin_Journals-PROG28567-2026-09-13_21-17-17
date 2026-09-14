using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; 

public class SquareSpawner : MonoBehaviour
{
    public InputActionReference _leftMouse; // Left mouse click -> Spawn Square
    public InputActionReference _scrollWheel; // Scroll wheel -> Change size


    // Create a structure containing the data needed for the spawned squares (pos, color, alpha, size)
    private struct SquareData
    {
        // The public/ current values of the square
        public Vector2 Position;
        public Color Color;
        public float Alpha;
        public float Size;

        // The storable values of each individual square
        public SquareData(Vector2 position, Color color, float alpha, float size)
        {
            Position = position;
            Color = color;
            Alpha = alpha;
            Size = size;
        }
    }
    
    // Declare a new list with the squares spawned when left click is used
    private List<SquareData> _spawnedSquares = new List<SquareData>();
    private float _currentSizeOffset = 0f;

    // Wake up when player inputs are used
    private void OnEnable()
    {
        _leftMouse.action.Enable();
        _scrollWheel.action.Enable();
    }
    // Sleep when player inputs are unused
    private void OnDisable()
    {
        _leftMouse.action.Disable();
        _scrollWheel.action.Disable();
    }

    // Update is called once per frame
    void Update() 
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue(); // Convert mousepos -> world pos with no Z
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mousePos.z = 0;
        
        Vector2 drawPos = new Vector2(mousePos.x, mousePos.y);  // Read mouse pos, and scroll "pos"/value
        Vector2 scrollDelta = _scrollWheel.action.ReadValue<Vector2>();
        
        // Use the Y component of the scroll delta for size adjustment
        _currentSizeOffset += scrollDelta.y * 0.01f;

        // Draw the template (using the current accumulated size offset)
        DrawSquare(drawPos, Color.forestGreen, 0.3f, _currentSizeOffset);
        
        // Check if the button was clicked this frame to spawn a permanent square
        if (_leftMouse.action.WasPressedThisFrame())
        {   
            SpawnSquare(drawPos, _currentSizeOffset);
        }

        // Draw all stored squares
        foreach (var square in _spawnedSquares)
        {
            DrawSquare(square.Position, square.Color, square.Alpha, square.Size);
        }
    }

    private void SpawnSquare(Vector2 position, float sizeOffset)
    {
        _spawnedSquares.Add(new SquareData(position, Color.red, 1.0f, sizeOffset));
    }

    private void DrawSquare(Vector2 position, Color color, float alpha, float sizeOffset)
    {
        float baseSize = 1/3f;
        // Apply the offset to the base size
        float halfSize = baseSize + sizeOffset;

        // Apply the specified alpha to the provided color
        Color finalColor = new Color(color.r, color.g, color.b, alpha);
        
        // Top left to Top right
        Debug.DrawLine(new Vector3(position.x - halfSize, position.y + halfSize, 0), new Vector3(position.x + halfSize, position.y + halfSize, 0), finalColor);
        // Top right to Bottom right
        Debug.DrawLine(new Vector3(position.x + halfSize, position.y + halfSize, 0), new Vector3(position.x + halfSize, position.y - halfSize, 0), finalColor);
        // Bottom right to Bottom left
        Debug.DrawLine(new Vector3(position.x + halfSize, position.y - halfSize, 0), new Vector3(position.x - halfSize, position.y - halfSize, 0), finalColor);
        // Bottom left to Top left
        Debug.DrawLine(new Vector3(position.x - halfSize, position.y - halfSize, 0), new Vector3(position.x - halfSize, position.y + halfSize, 0), finalColor);
    }
}