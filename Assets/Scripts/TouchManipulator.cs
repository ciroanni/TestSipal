using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TouchManipulator : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;

    [Header("Manipulation Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float scaleSpeed = 0.5f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 5f;

    [Header("Touch Detection")]
    [SerializeField] private float touchDistance = 20f;

    private GameObject selectedObject;
    private Vector3 initialScale;
    private Vector2 lastSingleTouchPosition;
    private float lastTwoFingerDistance;
    private Vector2 lastTwoFingerCenter;
    private bool isDragging = false;
    private bool isScaling = false;
    private bool isRotating = false;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public enum ManipulationMode
    {
        Translate,
        Rotate,
        Scale,
        Auto // Automatically determine based on touch input
    }

    [Header("Current Mode")]
    [SerializeField] public ManipulationMode currentMode = ManipulationMode.Auto;

    void Start()
    {
        arRaycastManager = FindFirstObjectByType<ARRaycastManager>();
        if (arCamera == null)
            arCamera = Camera.main;
    }

    void Update()
    {
        HandleTouchInput();
        HandleKeyboardModeSwitch(); // For editor testing
    }

    void HandleKeyboardModeSwitch()
    {
        // Switch modes with keyboard (for testing in editor)
        if (Input.GetKeyDown(KeyCode.T))
            currentMode = ManipulationMode.Translate;
        else if (Input.GetKeyDown(KeyCode.R))
            currentMode = ManipulationMode.Rotate;
        else if (Input.GetKeyDown(KeyCode.S))
            currentMode = ManipulationMode.Scale;
        else if (Input.GetKeyDown(KeyCode.A))
            currentMode = ManipulationMode.Auto;
    }
    void HandleTouchInput()
    {
        int touchCount = Input.touchCount;

        // Handle mouse input for editor simulation
        if (touchCount == 0 && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)))
        {
            HandleMouseInput();
            return;
        }

        if (touchCount == 0)
        {
            // No touches - end any current manipulation
            EndManipulation();
            return;
        }

        if (touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (touchCount == 2)
        {
            HandleTwoFingerTouch();
        }
        else
        {
            // More than 2 fingers - end manipulation
            EndManipulation();
        }
    }

    void HandleMouseInput()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectObject(mousePosition);
        }
        else if (Input.GetMouseButton(0) && selectedObject != null)
        {
            if (currentMode == ManipulationMode.Auto || currentMode == ManipulationMode.Translate)
            {
                HandleTranslation(mousePosition);
            }
            else if (currentMode == ManipulationMode.Rotate)
            {
                HandleRotationSingle(mousePosition);
            }
            else if (currentMode == ManipulationMode.Scale)
            {
                HandleScaleSingle(mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndManipulation();
        }
    }

    void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = touch.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TrySelectObject(touchPosition);
                break;

            case TouchPhase.Moved:
                if (selectedObject != null && isDragging)
                {
                    if (currentMode == ManipulationMode.Auto || currentMode == ManipulationMode.Translate)
                    {
                        HandleTranslation(touchPosition);
                    }
                    else if (currentMode == ManipulationMode.Rotate)
                    {
                        HandleRotationSingle(touchPosition);
                    }
                    else if (currentMode == ManipulationMode.Scale)
                    {
                        HandleScaleSingle(touchPosition);
                    }
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndManipulation();
                break;
        }
    }

    void HandleTwoFingerTouch()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        Vector2 touch1Position = touch1.position;
        Vector2 touch2Position = touch2.position;
        Vector2 centerPoint = (touch1Position + touch2Position) / 2f;
        float currentDistance = Vector2.Distance(touch1Position, touch2Position);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            // Start two-finger manipulation
            if (selectedObject == null)
            {
                TrySelectObject(centerPoint);
            }

            if (selectedObject != null)
            {
                lastTwoFingerDistance = currentDistance;
                lastTwoFingerCenter = centerPoint;
                isScaling = true;
                isRotating = true;
                isDragging = false; // Disable single-finger dragging
            }
        }
        else if ((touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) && selectedObject != null)
        {
            if (currentMode == ManipulationMode.Auto)
            {
                // Auto mode: handle both scale and rotation simultaneously
                HandleTwoFingerScale(currentDistance);
                HandleTwoFingerRotation(touch1Position, touch2Position);
                HandleTwoFingerTranslation(centerPoint);
            }
            else if (currentMode == ManipulationMode.Scale)
            {
                HandleTwoFingerScale(currentDistance);
            }
            else if (currentMode == ManipulationMode.Rotate)
            {
                HandleTwoFingerRotation(touch1Position, touch2Position);
            }
            else if (currentMode == ManipulationMode.Translate)
            {
                HandleTwoFingerTranslation(centerPoint);
            }

            lastTwoFingerDistance = currentDistance;
            lastTwoFingerCenter = centerPoint;
        }
        else if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended ||
                 touch1.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
        {
            EndManipulation();
        }
    }

    void TrySelectObject(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, touchDistance))
        {
            if (hit.collider.gameObject.GetComponent<Car>() != null)
            {
                selectedObject = hit.collider.gameObject;
                initialScale = selectedObject.transform.localScale;
                lastSingleTouchPosition = screenPosition;
                isDragging = true;
                
                Debug.Log($"Selected object: {selectedObject.name} for manipulation");
            }
        }
    }

    void HandleTranslation(Vector2 screenPosition)
    {
        if (arRaycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            selectedObject.transform.position = hitPose.position;
        }
    }

    void HandleRotationSingle(Vector2 screenPosition)
    {
        Vector2 deltaPosition = screenPosition - lastSingleTouchPosition;
        float rotationAmount = deltaPosition.x * rotationSpeed * Time.deltaTime;
        
        selectedObject.transform.Rotate(Vector3.up, rotationAmount, Space.World);
        lastSingleTouchPosition = screenPosition;
    }

    void HandleScaleSingle(Vector2 screenPosition)
    {
        Vector2 deltaPosition = screenPosition - lastSingleTouchPosition;
        float scaleAmount = deltaPosition.y * scaleSpeed * Time.deltaTime;
        
        Vector3 currentScale = selectedObject.transform.localScale;
        float newScaleValue = currentScale.x + scaleAmount;
        newScaleValue = Mathf.Clamp(newScaleValue, minScale, maxScale);
        
        selectedObject.transform.localScale = Vector3.one * newScaleValue;
        lastSingleTouchPosition = screenPosition;
    }

    void HandleTwoFingerScale(float currentDistance)
    {
        if (!isScaling) return;

        float scaleRatio = currentDistance / lastTwoFingerDistance;
        Vector3 currentScale = selectedObject.transform.localScale;
        Vector3 newScale = currentScale * scaleRatio;
        
        // Clamp the scale
        float clampedScaleValue = Mathf.Clamp(newScale.x, minScale, maxScale);
        selectedObject.transform.localScale = Vector3.one * clampedScaleValue;
    }

    void HandleTwoFingerRotation(Vector2 touch1Pos, Vector2 touch2Pos)
    {
        if (!isRotating) return;

        Vector2 currentVector = touch2Pos - touch1Pos;
        Vector2 lastTouch1 = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
        Vector2 lastTouch2 = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;
        Vector2 lastVector = lastTouch2 - lastTouch1;

        float angle = Vector2.SignedAngle(lastVector, currentVector);
        selectedObject.transform.Rotate(Vector3.up, angle, Space.World);
    }

    void HandleTwoFingerTranslation(Vector2 centerPoint)
    {
        if (arRaycastManager.Raycast(centerPoint, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            selectedObject.transform.position = hitPose.position;
        }
    }

    void EndManipulation()
    {
        selectedObject = null;
        isDragging = false;
        isScaling = false;
        isRotating = false;
    }

    // Public methods for UI control
    public void SetTranslateMode() { currentMode = ManipulationMode.Translate; }
    public void SetRotateMode() { currentMode = ManipulationMode.Rotate; }
    public void SetScaleMode() { currentMode = ManipulationMode.Scale; }
    public void SetAutoMode() { currentMode = ManipulationMode.Auto; }

    public ManipulationMode GetCurrentMode() { return currentMode; }
    public GameObject GetSelectedObject() { return selectedObject; }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (selectedObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(selectedObject.transform.position, 0.1f);
        }
    }
}
