using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayInteraction : MonoBehaviour
{
    public Transform[] slotPositions; // Array of all the slot positions on the tray
    private GameObject[] placedObjects; // Array of placed objects
    private GameObject heldObject; // Object that is currently held by user
    public GameObject rightHand;
    public GameObject leftHand;

 //public class


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

        GameObject slotObject3 = new GameObject("Fourth slot");
        slotObject3.transform.position = new Vector3(-0.202f, 0.83f, 0.19f);
        slotPositions[3] = slotObject3.transform;

        GameObject slotObject4 = new GameObject("Fifth slot");
        slotObject4.transform.position = new Vector3(-0.202f, 0.83f, -0.036f);
        slotPositions[4] = slotObject4.transform;

        GameObject slotObject5 = new GameObject("Sixth slot");
        slotObject5.transform.position = new Vector3(-0.202f, 0.83f, -0.265f);
        slotPositions[5] = slotObject5.transform;

        placedObjects = new GameObject[slotPositions.Length];
    }

    // FUNCTION FOR PLACING GAME OBJECT ON TRAY
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
            // calculating the distance between the tray and the held object
            float distanceToTray = Vector3.Distance(heldObject.transform.position, transform.position);
            float placeThreshold = 0.1f; // Here we adjust the distance if we want to
            if (distanceToTray < placeThreshold) 
            {
                ReleaseObjectOnTray(); // then release the object onto  the tray
            }
        }
    }

    // function to pick up object 
    // should make the picked up object become the variable heldObject
    void PickUpObject(GameObject objectToPickUp)
    {
        heldObject = objectToPickUp; // Vad innebär detta?
        //objectToPickUp.transform.SetParent(controllerTransform); // Objektet ska vara där handen är, så att vi kan se när den är nära Tray
                                                                 // Men objektet kommer redan följa med handen när det är upplockat? Behövs verkligen denna kodrad?
        
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
                // sound/message that it cannot be placed?
            }
        }
    }
}

// LeftHand.ObjectAttachmentPoint

// Hand position
// Vector3 handPos = player.transform.localPosition + pose.GetLocalPosition(inputHands[i]);