using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isDragging = false;
    Vector3 startMousePos = Vector2.zero;
    Vector3 startPos = Vector2.zero;
    [SerializeField] float movementSpeed;
    [SerializeField] private Transform maxCamPos;
    [SerializeField] private Transform minCamPos;
    void Start()
    {
        startPos.x = transform.position.x; 
        startPos.y = transform.position.z; 
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 screenMovement = Camera.main.ScreenToViewportPoint(currentMousePos)- Camera.main.ScreenToViewportPoint( startMousePos);
            Vector3 remap = new Vector3(-screenMovement.x, 0,-screenMovement.y) * movementSpeed;
            Debug.Log(remap);
            Vector3 newPos = startPos - remap;
            newPos.z = Mathf.Clamp(newPos.z, minCamPos.position.z, maxCamPos.position.z);
            newPos.x = Mathf.Clamp(newPos.x, minCamPos.position.x, maxCamPos.position.x);
            transform.position = newPos;
        }
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
}
