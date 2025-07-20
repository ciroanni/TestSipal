using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class SpawnableManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] GameObject spawnablePrefab;

    private Camera arCamera;
    GameObject spawnedObject = null;
    
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
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position;
                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    SpawnPrefabAtTouchPosition(touchPosition);
                }
            }
        }
        // Handle mouse input for simulation in editor
        else if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            if (arRaycastManager.Raycast(mousePosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                SpawnPrefabAtTouchPosition(mousePosition);
            }
        }
    }

    void SpawnPrefabAtTouchPosition(Vector2 inputPosition)
    {
        if (arRaycastManager.Raycast(inputPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(spawnablePrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }
    }
}
