using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeStarSystemUIManager : MonoBehaviour
{
    enum CurrenOpenUI
    {
        Closed,
        Planet,
        SpaceStructure
    }

    private CurrenOpenUI CurrentOpenUIState = CurrenOpenUI.Closed;
    private GameObject lastOpenedGameObject;
    void HandleIsSelected()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePosition.x, mousePosition.y), Vector2.zero, 0f);

            if (hit)
            {
                if(hit.collider.gameObject == lastOpenedGameObject && (CurrentOpenUIState != CurrenOpenUI.Closed))
                {
                    return;
                }

                if (lastOpenedGameObject && (hit.collider.gameObject != lastOpenedGameObject) && (CurrentOpenUIState != CurrenOpenUI.Closed))
                {
                    SpriteRenderer sprite = lastOpenedGameObject.GetComponent<SpriteRenderer>();
                    sprite.color = Color.white;
                    Debug.Log("Closed: " + lastOpenedGameObject.name + " UI");
                }


                if (hit && hit.collider)
                {
                    if (hit.collider.tag == "Planet")
                    {
                        lastOpenedGameObject = hit.collider.gameObject;
                        SpriteRenderer sprite = lastOpenedGameObject.GetComponent<SpriteRenderer>();
                        sprite.color = Color.red;
                        CurrentOpenUIState = CurrenOpenUI.Planet;
                        Debug.Log("Open: " + lastOpenedGameObject.name + " UI");
                        //HandleOpenPlanetUI();
                    }
                }
            }

            if(!hit && lastOpenedGameObject && (CurrentOpenUIState!=CurrenOpenUI.Closed))
            {
                SpriteRenderer sprite = lastOpenedGameObject.GetComponent<SpriteRenderer>();
                sprite.color = Color.white;
                Debug.Log("Closed: " + lastOpenedGameObject.name + " UI");
                CurrentOpenUIState = CurrenOpenUI.Closed;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleIsSelected();
    }
}
