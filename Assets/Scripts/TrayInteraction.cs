using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayInteraction : MonoBehaviour
{
    public Transform[] slotPositions; // Array of all the slot positions on the tray
    private GameObject[] placedObjects; // Array of placed objects
    private GameObject heldObject; // Object that is currently held by user                             


    // Start is called before the first frame update
    void Start()
    {
        slotPositions = new Transform[6]; // We have 6 slots

        GameObject slotObject0 = new GameObject("First slot");
        slotObject0.transform.position = new Vector3(0.127f, 0.83f, 0.19f);
        slotPositions[0] = slotObject0.transform;

        GameObject slotObject1 = new GameObject("Second slot");
        slotObject1.transform.position = new Vector3(0.127f, 0.83f, -0.036f);
        slotPositions[1] = slotObject1.transform;

        GameObject slotObject2 = new GameObject("Third slot");
        slotObject2.transform.position = new Vector3(0.127f, 0.83f, -0.265f);
        slotPositions[2] = slotObject2.transform;

        GameObject slotObject3 = new GameObject("First slot");
        slotObject3.transform.position = new Vector3(-0.202f, 0.83f, 0.19f);
        slotPositions[3] = slotObject3.transform;

        GameObject slotObject4 = new GameObject("First slot");
        slotObject4.transform.position = new Vector3(-0.202f, 0.83f, -0.036f);
        slotPositions[4] = slotObject4.transform;

        GameObject slotObject5 = new GameObject("First slot");
        slotObject5.transform.position = new Vector3(-0.202f, 0.83f, -0.265f);
        slotPositions[5] = slotObject5.transform;

        placedObjects = new GameObject[slotPositions.Length];
    }

    // Function for placing game object on tray
    public bool PlaceObject(GameObject objectToPlace) 
    {
        for (int i = 0; i < slotPositions.Length; i++) 
        {
            if (placedObjects[i] == null) // If this slot is empty, put the object here
            { 
                objectToPlace.transform.position = slotPositions[i].position;

                // we disable the object's rigidbody, so it wont be affected by any external forces
                Rigidbody rb = objectToPlace.GetComponent<Rigidbody>();
                if (rb != null) 
                {
                    rb.isKinematic = true; // This makes the object kinematic
                }
                placedObjects[i] = objectToPlace;
                return true; // gameobject succcessfully placed
            }
        }
        return false; // no available slots on tray: could not place the object
    }

    // Update is called once per frame
    void Update()
    {
        if (heldObject != null) 
        {
            float distanceToTray = Vector3.Distance(heldObject.transform.position, transform.position);

            float placeThreshold = 0.1f; // Here we adjust the distance if we want to

            if (distanceToTray < placeThreshold) 
            {
                // then release the object onto  the tray
                ReleaseObjectOnTray();
            }
        }
    }

    // function to pick up object with the controller, though we already have this?
    void PickUpObject(GameObject objectToPickUp)
    {
        heldObject = objectToPickUp;
        objectToPickUp.transform.SetParent(controllerTransform);
    }

    void ReleaseObjectOnTray() 
    {
        if (heldObject != null)
        {
            // try to place object on tray
            bool placed = PlaceObject(heldObject);
            if (placed)
            {
                // object successfully placed on the tray
                heldObject = null; // clear the held object reference
            }
            else 
            {
                // What do we want to happen if there are no available slots on the tray?
                // sound/message that it cannot be placed
            }
        }
    }
}

