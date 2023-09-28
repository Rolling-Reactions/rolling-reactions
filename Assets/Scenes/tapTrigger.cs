using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tapTrigger : MonoBehaviour
{
    public GameObject emitter;
    public bool turningOn = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turningOn)
        {
            emitter.SetActive(true);
        }
        else
        {
            emitter.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        turningOn = !turningOn;
     
    }
}
