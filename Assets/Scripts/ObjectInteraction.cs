using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float scaleSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 5f;

    [Header("AR Components")]
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;

    private GameObject selectedObject;
    private Vector3 lastTouchPosition;
    private Vector3 initialScale;
    private Vector3 planeNormal = Vector3.up; // Normal of the detected plane
    private float planeHeight = 0f; // Height of the detected plane

    public enum InteractionMode
    {
        Move,
        Scale,
        Rotate
    }

    [Header("Current Mode")]
    [SerializeField] private InteractionMode currentMode = InteractionMode.Move;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        HandleModeSwitch();
    }

    void HandleModeSwitch()
    {
        // Switch modes with keyboard (for testing in editor)
        if (Input.GetKeyDown(KeyCode.M))
            currentMode = InteractionMode.Move;
        else if (Input.GetKeyDown(KeyCode.S))
            currentMode = InteractionMode.Scale;
        else if (Input.GetKeyDown(KeyCode.R))
            currentMode = InteractionMode.Rotate;

        // For mobile, you could add UI buttons to switch modes
    }

    void HandleInput()
    {
        Vector2 inputPosition = Vector2.zero;
        bool inputDetected = false;

        // Handle touch input
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                OnInputBegan(inputPosition);
            }
            else if (touch.phase == TouchPhase.Moved && selectedObject != null)
            {
                OnInputMoved(inputPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                OnInputEnded();
            }
            inputDetected = true;
        }
        // Handle mouse input for editor simulation
        else if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
            OnInputBegan(inputPosition);
            inputDetected = true;
        }
        else if (Input.GetMouseButton(0) && selectedObject != null)
        {
            inputPosition = Input.mousePosition;
            OnInputMoved(inputPosition);
            inputDetected = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnInputEnded();
        }

        // Handle two-finger gestures for scale (touch only)
        if (Input.touchCount == 2 && selectedObject != null)
        {
            HandleTwoFingerGestures();
        }
    }

    void OnInputBegan(Vector2 inputPosition)
    {
        // Try to select an object
        Ray ray = arCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if we hit a spawned object
            if (hit.collider.gameObject.GetComponent<Renderer>() != null)
            {
                selectedObject = hit.collider.gameObject;
                initialScale = selectedObject.transform.localScale;
                lastTouchPosition = inputPosition;

                // Get the plane information where the object is placed
                UpdatePlaneInfo(inputPosition);
                
                Debug.Log($"Selected object: {selectedObject.name}, Mode: {currentMode}");
            }
        }
    }

    void OnInputMoved(Vector2 inputPosition)
    {
        if (selectedObject == null) return;

        switch (currentMode)
        {
            case InteractionMode.Move:
                HandleMove(inputPosition);
                break;
            case InteractionMode.Scale:
                HandleScale(inputPosition);
                break;
            case InteractionMode.Rotate:
                HandleRotation(inputPosition);
                break;
        }

        lastTouchPosition = inputPosition;
    }

    void OnInputEnded()
    {
        selectedObject = null;
    }

    void HandleMove(Vector2 inputPosition)
    {
        // Raycast to find new position on the plane
        if (arRaycastManager.Raycast(inputPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            
            // Move the object to the new position, maintaining its height relative to the plane
            Vector3 newPosition = hitPose.position;
            selectedObject.transform.position = newPosition;
            
            // Update plane info for consistent movement
            planeNormal = hitPose.up;
            planeHeight = hitPose.position.y;
        }
    }

    void HandleScale(Vector2 inputPosition)
    {
        // Calculate scale based on vertical movement
        float deltaY = (inputPosition.y - lastTouchPosition.y) * scaleSpeed * Time.deltaTime;
        Vector3 currentScale = selectedObject.transform.localScale;
        
        float newScaleValue = currentScale.x + deltaY;
        newScaleValue = Mathf.Clamp(newScaleValue, minScale, maxScale);
        
        selectedObject.transform.localScale = Vector3.one * newScaleValue;
    }

    void HandleRotation(Vector2 inputPosition)
    {
        // Calculate rotation based on horizontal movement
        float deltaX = (inputPosition.x - lastTouchPosition.x) * rotationSpeed * Time.deltaTime;
        
        // Rotate around the plane's normal (usually Y-axis for horizontal planes)
        selectedObject.transform.Rotate(planeNormal, deltaX, Space.World);
    }

    void HandleTwoFingerGestures()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        // Get current distance between fingers
        float currentDistance = Vector2.Distance(touch1.position, touch2.position);
        
        // Get previous distance between fingers
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
        float prevDistance = Vector2.Distance(touch1PrevPos, touch2PrevPos);

        // Calculate scale factor
        float deltaDistance = currentDistance - prevDistance;
        float scaleFactor = deltaDistance * scaleSpeed * Time.deltaTime;

        // Apply scale
        Vector3 currentScale = selectedObject.transform.localScale;
        float newScaleValue = currentScale.x + scaleFactor;
        newScaleValue = Mathf.Clamp(newScaleValue, minScale, maxScale);
        
        selectedObject.transform.localScale = Vector3.one * newScaleValue;
    }

    void UpdatePlaneInfo(Vector2 inputPosition)
    {
        if (arRaycastManager.Raycast(inputPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            planeNormal = hitPose.up;
            planeHeight = hitPose.position.y;
        }
    }

    // Public methods to change interaction mode (can be called from UI buttons)
    public void SetMoveMode() { currentMode = InteractionMode.Move; }
    public void SetScaleMode() { currentMode = InteractionMode.Scale; }
    public void SetRotateMode() { currentMode = InteractionMode.Rotate; }

    // Get current mode for UI display
    public InteractionMode GetCurrentMode() { return currentMode; }
}
