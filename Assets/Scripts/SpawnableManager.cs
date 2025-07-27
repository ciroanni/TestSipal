using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class SpawnableManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] GameObject spawnablePrefab;

    [Header("Deletion Settings")]
    [SerializeField] private KeyCode deleteKey = KeyCode.X; // Key to delete object (for editor testing)
    [SerializeField] private float doubleTapTime = 0.3f; // Time window for double tap detection

    private Camera arCamera;
    GameObject spawnedObject = null;
    
    // Double tap detection variables
    private float lastTapTime = 0f;
    private int tapCount = 0;

    [SerializeField] private UIManipulationManager[] UIButtons;

    // Public property to access the spawned object
    public GameObject GetSpawnedObject() { return spawnedObject; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDeletion();
        HandleSpawning();
    }

    void HandleDeletion()
    {
        // Handle keyboard deletion for editor testing
        if (Input.GetKeyDown(deleteKey) && spawnedObject != null)
        {
            DeleteSpawnedObject();
        }
    }

    void HandleSpawning()
    {
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                HandleInput(touchPosition);
            }
        }
        // Handle mouse input for simulation in editor
        else if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            HandleInput(mousePosition);
        }
    }

    void HandleInput(Vector2 inputPosition)
    {
        // If object already exists, check for deletion input
        if (spawnedObject != null)
        {
            // Check for double tap to delete
            float currentTime = Time.time;
            if (currentTime - lastTapTime < doubleTapTime)
            {
                tapCount++;
                if (tapCount >= 2)
                {
                    // Double tap detected - delete object
                    if (IsInputOnObject(inputPosition))
                    {
                        DeleteSpawnedObject();
                        tapCount = 0;
                        return;
                    }
                }
            }
            else
            {
                tapCount = 1;
            }
            lastTapTime = currentTime;
            
            // Don't allow spawning if object already exists
            Debug.Log("Object already spawned. Double-tap on the object to delete it, or press X key (editor only).");
            return;
        }

        // No object exists, try to spawn one
        if (arRaycastManager.Raycast(inputPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            SpawnPrefabAtTouchPosition(inputPosition);
        }
    }

    bool IsInputOnObject(Vector2 inputPosition)
    {
        if (spawnedObject == null) return false;

        // Cast a ray to see if we hit the spawned object
        Ray ray = arCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == spawnedObject;
        }
        
        return false;
    }

    void SpawnPrefabAtTouchPosition(Vector2 inputPosition)
    {
        if (arRaycastManager.Raycast(inputPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Only spawn if no object exists
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(spawnablePrefab, hitPose.position, hitPose.rotation);
                foreach (var button in UIButtons)
                {
                    button.SetTouchManipulator(spawnedObject.GetComponent<TouchManipulator>());
                }
                Debug.Log("Object spawned successfully!");
            }
        }
    }

    void DeleteSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Debug.Log("Deleting spawned object.");
            Destroy(spawnedObject);
            spawnedObject = null;
            tapCount = 0; // Reset tap count
        }
    }

    // Public method to delete object (can be called from UI buttons)
    public void DeleteCurrentObject()
    {
        DeleteSpawnedObject();
    }

    // Public method to check if object exists
    public bool HasSpawnedObject()
    {
        return spawnedObject != null;
    }
}
