using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ARTouchTooltip : MonoBehaviour
{
    [Header("Tooltip Settings")]
    public SimpleTooltipStyle simpleTooltipStyle;
    [TextArea] public string infoLeft = "Hello";
    [TextArea] public string infoRight = "";
    
    [Header("AR Touch Settings")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private float touchDistance = 10f; // Maximum distance for touch detection
    
    private STController tooltipController;
    private EventSystem eventSystem;
    private bool isShowing = false;
    private bool isUIObject = false;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        tooltipController = FindObjectOfType<STController>();

        // Add a new tooltip prefab if one does not exist yet
        if (!tooltipController)
        {
            tooltipController = AddTooltipPrefabToScene();
        }
        if (!tooltipController)
        {
            Debug.LogWarning("Could not find the Tooltip prefab");
            Debug.LogWarning("Make sure you don't have any other prefabs named `SimpleTooltip`");
        }

        if (GetComponent<RectTransform>())
            isUIObject = true;

        // Always make sure there's a style loaded
        if (!simpleTooltipStyle)
            simpleTooltipStyle = Resources.Load<SimpleTooltipStyle>("STDefault");

        // Get AR camera if not assigned
        if (arCamera == null)
            arCamera = Camera.main;
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CheckTouchOnObject(touch.position);
            }
        }
        // Handle mouse input for simulation in editor (prioritize mouse when no touches)
        else if (Input.GetMouseButtonDown(0))
        {
            CheckTouchOnObject(Input.mousePosition);
        }
        
        // Alternative: Always check mouse input for debugging (uncomment if needed)
        // This allows mouse input even when touch is available
        /*
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouchOnObject(Input.mousePosition);
        }
        */
    }

    private void CheckTouchOnObject(Vector2 screenPosition)
    {
        // Skip if this is a UI object and we're over UI
        if (isUIObject)
        {
            if (eventSystem && eventSystem.IsPointerOverGameObject())
                return;
        }

        // Ensure we have a valid camera
        if (arCamera == null)
        {
            Debug.LogWarning("AR Camera is null! Cannot perform raycast.");
            return;
        }

        // Cast a ray from the touch/mouse position
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Debug.Log($"Casting ray from screen position: {screenPosition}");

        // Check if the ray hits this object
        if (Physics.Raycast(ray, out hit, touchDistance))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name} at distance: {hit.distance}");
            
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"Hit target object: {gameObject.name}");
                ToggleTooltip();
            }
            else
            {
                Debug.Log($"Hit different object, hiding tooltip");
                // If we hit something else, hide the tooltip
                HideTooltip();
            }
        }
        else
        {
            Debug.Log("No hit detected, hiding tooltip");
            // If we didn't hit anything, hide the tooltip
            HideTooltip();
        }
    }

    public static STController AddTooltipPrefabToScene()
    {
        return Instantiate(Resources.Load<GameObject>("SimpleTooltip")).GetComponentInChildren<STController>();
    }

    public void ToggleTooltip()
    {
        if (isShowing)
        {
            HideTooltip();
        }
        else
        {
            ShowTooltip();
        }
    }

    public void ShowTooltip()
    {
        if (isShowing) return;

        isShowing = true;

        // Update the text for both layers
        tooltipController.SetCustomStyledText(infoLeft, simpleTooltipStyle, STController.TextAlign.Left);
        tooltipController.SetCustomStyledText(infoRight, simpleTooltipStyle, STController.TextAlign.Right);

        // Then tell the controller to show it
        tooltipController.ShowTooltip();

        Debug.Log($"Showing tooltip for: {gameObject.name}");
    }

    public void HideTooltip()
    {
        if (!isShowing) return;

        isShowing = false;
        tooltipController.HideTooltip();

        Debug.Log($"Hiding tooltip for: {gameObject.name}");
    }

    // Public methods for external control
    public void ForceShowTooltip()
    {
        ShowTooltip();
    }

    public void ForceHideTooltip()
    {
        HideTooltip();
    }

    public bool IsTooltipShowing()
    {
        return isShowing;
    }

    // Method to update tooltip content dynamically
    public void UpdateTooltipContent(string newInfoLeft, string newInfoRight = "")
    {
        infoLeft = newInfoLeft;
        infoRight = newInfoRight;
        
        // If currently showing, update the display
        if (isShowing)
        {
            tooltipController.SetCustomStyledText(infoLeft, simpleTooltipStyle, STController.TextAlign.Left);
            tooltipController.SetCustomStyledText(infoRight, simpleTooltipStyle, STController.TextAlign.Right);
        }
    }

    private void Reset()
    {
        // Load the default style if none is specified
        if (!simpleTooltipStyle)
            simpleTooltipStyle = Resources.Load<SimpleTooltipStyle>("STDefault");

        // If UI, nothing else needs to be done
        if (GetComponent<RectTransform>())
            return;

        // If has a collider, nothing else needs to be done
        if (GetComponent<Collider>())
            return;

        // There were no colliders found when the component is added so we'll add a box collider by default
        // If you are making a 2D game you can change this to a BoxCollider2D for convenience
        // You can obviously still swap it manually in the editor but this should speed up development
        gameObject.AddComponent<BoxCollider>();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere to visualize the touch detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        
        // Draw a line to show the maximum touch distance
        if (arCamera != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = (transform.position - arCamera.transform.position).normalized;
            Gizmos.DrawLine(arCamera.transform.position, arCamera.transform.position + direction * touchDistance);
        }
    }
}
