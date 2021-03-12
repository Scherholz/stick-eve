using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private bool cameraFollowPlayerFlag = true;
    private float ScrollSpeed = 15f;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform objectToFollow;
    public Text textCameraMode;

    // Start is called before the first frame update
    void Start()
    {
        cameraFollowPlayerFlag = true;
        textCameraMode.text = "RTS";
        
    }

    void HandleCameraMode()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            textCameraMode.text = "Follow";
            Debug.Log("Key F");
            cameraFollowPlayerFlag = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            textCameraMode.text = "RTS";
            Debug.Log("Key S");
            cameraFollowPlayerFlag = false;
        }
    }

    void HandleCameraMovement()
    {
        if (Input.mousePosition.y >= Screen.height * 0.95)
        {
            Camera.main.transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * ScrollSpeed, Space.World);
        }

        if (Input.mousePosition.x >= Screen.width * 0.95)
        {
            Camera.main.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * ScrollSpeed, Space.World);
        }

        if (Input.mousePosition.y <= Screen.height * 0.05)
        {
            Camera.main.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * ScrollSpeed, Space.World);
        }

        if (Input.mousePosition.x <= Screen.width * 0.05)
        {
            Camera.main.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * ScrollSpeed, Space.World);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!objectToFollow)
        {
            if (GameObject.FindGameObjectWithTag("PlayerSpaceShip"))
            {
                objectToFollow = GameObject.FindGameObjectWithTag("PlayerSpaceShip").GetComponent<Transform>();
            }
            
        }
        HandleCameraMode();
        /// TO-DO : make camera child to player
        if (cameraFollowPlayerFlag)
        {
            if (objectToFollow)
            {
                Vector3 point = Camera.main.WorldToViewportPoint(objectToFollow.position);
                Vector3 delta = objectToFollow.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
                Vector3 destination = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }
        }
        else
        {
            HandleCameraMovement();
        }
    }
}
