using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInteraction : MonoBehaviour
{
    private GameObject grabbedObject; // Variable to store the reference to the held object
    private SteamVR_TrackedObject trackedObj; // Attach this script to your Vive controller GameObject

    private void Start()
    {
        trackedObj = GetComponent < SteamVR_TrackedObject();
    }

    private void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            // Detect collisions between the controller and objects
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f); // Adjust the radius as needed

            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Interactable"))
                {
                    GrabObject(col.gameObject);

                    // Now the 'grabbedObject' variable contains the reference to the held object
                    break;
                }
            }
        }

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            if (grabbedObject != null)
            {
                ReleaseObject();
            }
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj; // Store the reference to the held object
        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
        grabbedObject.transform.SetParent(transform);
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject.transform.SetParent(null);
            grabbedObject = null; // Clear the reference to the held object
        }
    }
}