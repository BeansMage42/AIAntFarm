using System.Threading;
using TreeEditor;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraDrag : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isDragging = false;
    Vector3 startMousePos = Vector2.zero;
    Vector3 startPos = Vector2.zero;
    [SerializeField] float movementSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] private Transform maxCamPos;
    [SerializeField] private Transform minCamPos;
    [SerializeField] private float maxCameraHeight;
    [SerializeField] private float minCameraHeight;
    [SerializeField] private float zoomSpeed;
    [SerializeField] CinemachineCamera dragCamera;
    [SerializeField] GameObject cameraControl;
    [SerializeField] Transform pivotPoint;
    private float rotateDir;
    private Vector3 moveDir;
    void Start()
    {
        startPos.x = transform.position.x; 
        startPos.y = transform.position.z; 
        startPos = transform.position;
        //topRingRadius = orbitalCam.Orbits.Top.Radius;

    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {

            


            Vector3 currentMousePos = Input.mousePosition;
            Vector3 screenMovement = Camera.main.ScreenToViewportPoint(currentMousePos) - Camera.main.ScreenToViewportPoint(startMousePos);
                Vector3 remap = pivotPoint.rotation* new Vector3(-screenMovement.x, 0, -screenMovement.y) * movementSpeed ;
                Debug.Log(remap);

                Vector3 newPos = startPos - remap;
                newPos.z = Mathf.Clamp(newPos.z, minCamPos.position.z, maxCamPos.position.z);
                newPos.x = Mathf.Clamp(newPos.x, minCamPos.position.x, maxCamPos.position.x);
                transform.position = newPos;
            
               
        }
        else
        {
            Vector3 newPos = transform.position + (pivotPoint.rotation * moveDir * movementSpeed * Time.deltaTime);

            newPos.z = Mathf.Clamp(newPos.z, minCamPos.position.z, maxCamPos.position.z);
            newPos.x = Mathf.Clamp(newPos.x, minCamPos.position.x, maxCamPos.position.x);
            transform.position = newPos;
        }
        if (rotateDir != 0)
        {
            Quaternion rot = Quaternion.AngleAxis(rotateDir * rotateSpeed, Vector3.up);
            pivotPoint.rotation *= rot;
        }
    }

    public void OnRotate(InputAction.CallbackContext callback)
    {
        rotateDir = callback.ReadValue<float>();
        Debug.Log("rotationChanging : " + rotateDir);
    }
  

    public void OnDrag(InputAction.CallbackContext callback)
    {
        if (callback.started) 
        {
            startMousePos = Input.mousePosition;//Camera.main.ViewportToScreenPoint( Input.mousePosition);
            startPos = transform.position;
            isDragging = true;
        }
        if (callback.canceled)
        {
            isDragging = false;
        }
    }
    public void CameraZoom(InputAction.CallbackContext callback)
    {
        if (!callback.performed) return;
        float zoom = callback.ReadValue<float>();
        if (zoom == 0) return;
        float newHeight = transform.position.y - zoom * zoomSpeed;
        newHeight = Mathf.Clamp(newHeight,minCameraHeight,maxCameraHeight);
        transform.position = new(transform.position.x, newHeight,transform.position.z);
    }

    public void MoveDir(InputAction.CallbackContext callback)
    {
        Vector2 input = callback.ReadValue<Vector2>();
        moveDir = new Vector3(-input.x,0,-input.y).normalized;
    }

    
}
