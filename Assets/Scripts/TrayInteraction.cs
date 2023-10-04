using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayInteraction : MonoBehaviour
{
    public Transform[] slotPositions; // Array of all the slot positions on the tray
    //public Vector3[] slotPositions2; // Array of all the slot positions on the tray
    private GameObject[] placedObjects; // Array of placed objects


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
    public bool PlaceObject(GameObject objectToPlace) {
        for (int i = 0; i < slotPositions.Length; i++) {
            if (placedObjects[i] == null) { // If this slot is empty, put the object here
                objectToPlace.transform.position = slotPositions[i].position;
                placedObjects[i] = objectToPlace;
                return true; // gameobject succcessfully placed
            }

        }
        return false; // no available slots on tray: could not place the object
    }

    // Update is called once per frame
    void Update()
    {
        // If object is released within tray area, then 
    }
}
