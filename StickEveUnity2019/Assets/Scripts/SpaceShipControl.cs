using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipControl : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 currTargetPosition;
    private Vector3 targetDir;
    private Quaternion targetRotationQuaternion;
    private Queue<Vector3> targets = new Queue<Vector3>();
    private Queue<GameObject> targetsCircles = new Queue<GameObject>();
    private int movementMode = 3;
    private float speed = 5f;
    private bool isThisObjectSelected = false;
    private SpriteRenderer spriteRendererForSelection;

    public GameObject targetCirclePrefab;
    void HandleMovement1()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Debug.Log("Mouse Click");
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetDir = targetPosition - transform.position;
            targetDir.z = transform.position.z;
            var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            targetRotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            //transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
        }
        var diffAngle = Mathf.Abs(targetRotationQuaternion.eulerAngles.z - transform.rotation.eulerAngles.z);
        if (targetDir != Vector3.zero && diffAngle > 15)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, Time.fixedDeltaTime * 0.5f);
            //transform.Translate(transform.right * speed * Time.deltaTime);
            transform.position += transform.right * speed * Time.deltaTime;
        }
        else if (transform.position != targetPosition && Vector3.Distance(targetPosition, transform.position) > 10 && diffAngle <= 15)
        {
            //transform.rotation = targetRotationQuaternion;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, Time.fixedDeltaTime * 0.5f);
            //transform.Translate(transform.right * speed * Time.deltaTime);
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    void HandleMovement2()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
        }
    }

    void HandleMovement3()
    {

        if (Input.GetKey(KeyCode.Mouse0))
        {

            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            if (targets.Count == 0)
            {
                currTargetPosition = targetPosition;
                targetDir = currTargetPosition - transform.position;
                targetDir.z = transform.position.z;
                var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                targetRotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            GameObject copyCircleTargetPrefab = GameObject.Instantiate<GameObject>(targetCirclePrefab);
            copyCircleTargetPrefab.transform.position = targetPosition;

            targetsCircles.Enqueue(copyCircleTargetPrefab);
            targets.Enqueue(copyCircleTargetPrefab.transform.position);
        }

        if (targets.Count > 0 || currTargetPosition != transform.position)
        {
            if (currTargetPosition == transform.position)
            {
                Debug.Log("Destroyed");
                Destroy(targetsCircles.Dequeue());
                currTargetPosition = targets.Dequeue();
                targetDir = currTargetPosition - transform.position;
                targetDir.z = transform.position.z;
                var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                targetRotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                transform.rotation = targetRotationQuaternion;
                transform.position = Vector2.MoveTowards(transform.position, currTargetPosition, Time.deltaTime * 5);
            }
        }



    }

    void HandleMovementMode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Key 1");
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            movementMode = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Key 2");
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.blue;
            movementMode = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Key 3");
            currTargetPosition = transform.position;
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.green;
            movementMode = 3;
        }

        
    }

    private void Start()
    {
        Debug.Log("New code 2");
        targetPosition = transform.position;
        currTargetPosition = transform.position;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }

    void HandleIsSelected()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
            if (hit && hit.collider == this.GetComponent<Collider2D>()) {
                isThisObjectSelected = true;
                SpriteRenderer[] spritesList =  GetComponentsInChildren<SpriteRenderer>(true);
                foreach(var sprite in spritesList)
                {
                    if(sprite.name == "focusSprite")
                    {
                        spriteRendererForSelection = sprite;
                        sprite.enabled = true;
                    }
                }
            }
            else
            {
                if (spriteRendererForSelection)
                {
                    spriteRendererForSelection.enabled = false;
                }
                //isThisObjectSelected = false;
            }
        }
    }

    void Update()
    {
        HandleIsSelected();

        if (isThisObjectSelected)
        {
            HandleMovementMode();

            switch (movementMode)
            {
                case 1:
                    {
                        HandleMovement1();
                        break;
                    }
                case 2:
                    {
                        HandleMovement2();
                        break;
                    }
                case 3:
                    {
                        HandleMovement3();
                        break;
                    }
                default:
                    {
                        break;
                    }

            }
        }
        

    }
}
