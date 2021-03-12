using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

public class SpaceShipControl : PlayerMovementBehavior
{
    public Vector3 targetPosition;
    public Vector3 currTargetPosition;
    private Vector3 targetDir;
    private Quaternion targetRotationQuaternion;
    private Queue<Vector3> targets = new Queue<Vector3>();
    private Queue<GameObject> targetsCircles = new Queue<GameObject>();
    private int movementMode = 1;
    private float speed = 5f;
    private bool isThisObjectSelected = true;
    private SpriteRenderer spriteRendererForSelection;
    private RealTimeCombatGameManager m_gameManager;

    public GameObject m_laserPrefab;
    public GameObject targetCirclePrefab;
    public float TurnSpeed = 10f;

    void HandleMovement1()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //Debug.Log("Mouse Click");
        //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (currTargetPosition != transform.position) {
            targetPosition = currTargetPosition;
            targetDir = targetPosition - transform.position;
            targetDir.z = transform.position.z;
            var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            targetRotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
        }
            //transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5);
        //}
        var diffAngle = Mathf.Abs(targetRotationQuaternion.eulerAngles.z - transform.rotation.eulerAngles.z);
        if (targetDir != Vector3.zero && diffAngle > 15)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, Time.fixedDeltaTime * TurnSpeed);
            //transform.Translate(transform.right * speed * Time.deltaTime);
            transform.position += transform.right * speed * Time.deltaTime;
        }
        else if (transform.position != targetPosition && Vector3.Distance(targetPosition, transform.position) > 1 && diffAngle <= 15)
        {
            //transform.rotation = targetRotationQuaternion;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationQuaternion, Time.fixedDeltaTime * TurnSpeed);
            //transform.Translate(transform.right * speed * Time.deltaTime);
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    void ClientSideHandleMovement1()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Debug.Log("Mouse Click");
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
            if (targetPosition != currTargetPosition)
            {
                currTargetPosition = targetPosition;
                Debug.Log("New currTargetPosition");
                networkObject.SendRpc(RPC_SET_CURR_TARGET_POSITION, Receivers.Server, currTargetPosition);
                
            }
            
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
        m_gameManager = FindObjectOfType<RealTimeCombatGameManager>();
        m_gameManager.players.Add(this.gameObject);

        Screen.SetResolution(800, 450, false);
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
    private int laserCD = 0;
    void HandleAutoGuns()
    {
        List<GameObject> players = m_gameManager.players;

        if (laserCD == 10000)
        {
            laserCD = 0;
        }
        
        
        foreach(var player in players)
        {
            if (player == this.gameObject)
            {
                continue;
            }
            if (Vector2.Distance(transform.position, player.transform.position) < 5)
            {
                if (laserCD == 0)
                {
                    Vector3 dir = player.transform.position - transform.position;
                    Quaternion quat = Quaternion.LookRotation(dir);
                    GameObject.Instantiate<GameObject>(m_laserPrefab, transform.position, quat);
                    //Debug.Log("in range!");
                    laserCD++;
                }
                else
                {
                    laserCD++;
                }
                
            }
        }
    }

    void Update()
    {
        if (networkObject.IsServer)
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
            networkObject.SendRpc(RPC_SET_NET_WORK_OBJ_POSITION, Receivers.All, transform.position);
            networkObject.SendRpc(RPC_SET_NET_WORK_OBJ_ROTATION, Receivers.All, transform.rotation);

            return;
        }
        if (!networkObject.IsOwner)
        {
            //Debug.Log("Not Owner");
            transform.position = Vector2.MoveTowards(transform.position, networkObject.position, Time.deltaTime * 5);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkObject.rotation, Time.fixedDeltaTime * TurnSpeed);
            return;
        }
        if (!networkObject.IsServer && networkObject.IsOwner)
        {
            ClientSideHandleMovement1();
            //HandleAutoGuns();
            //Debug.Log("Log1 : networkObject.position: " + networkObject.position + ", networkObject.rotation: " + networkObject.rotation);
           // transform.position = networkObject.position;
            transform.position = Vector2.MoveTowards(transform.position, networkObject.position, Time.deltaTime * 5);
            //transform.rotation = networkObject.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, networkObject.rotation, Time.fixedDeltaTime * TurnSpeed);
            return;
        }
        
        //Debug.Log("currTargetPosition" + networkObject.currTargetPosition.x + "," + networkObject.currTargetPosition.y);
        //currTargetPosition = networkObject.currTargetPosition;

        HandleIsSelected();

        //if (isThisObjectSelected)
        //{
           
        //}

        /*Debug.Log("Networkobj: " + networkObject.AttachedBehavior);
        Debug.Log("Setting networkObject.position: " + transform.position);
        networkObject.position = transform.position;
        Debug.Log("Setting networkObject.rotation: " + transform.rotation);
        networkObject.rotation = transform.rotation;*/

    }

    public override void setCurrTargetPosition(RpcArgs args)
    {
        Debug.Log("Got currTargetPosition: " + networkObject.currTargetPosition.x + "," + networkObject.currTargetPosition.y);
        currTargetPosition = args.GetNext<Vector3>();
    }

    public override void setNetWorkObjPosition(RpcArgs args)
    {
        networkObject.position = args.GetNext<Vector3>();
    }

    public override void setNetWorkObjRotation(RpcArgs args)
    {
        networkObject.rotation = args.GetNext<Quaternion>();
    }
}
