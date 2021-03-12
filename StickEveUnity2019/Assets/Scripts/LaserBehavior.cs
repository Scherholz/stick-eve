using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Laser!");
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        Debug.Log("Exterminated");
        Destroy(gameObject);
    }

    public int timer =0;
    // Update is called once per frame
    void Update()
    {
        if (timer == 120)
        {
            Debug.Log("Exterminated");
            Destroy(gameObject);
        }
        timer++;
        transform.position += transform.right * 0.1f * Time.deltaTime;
    }
}
