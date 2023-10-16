using System;
using System.Collections;
using UnityEngine;

public class PouringDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public bool overThreshold = false;
    public Transform origin = null;
    public GameObject emitter;

    private bool isPouring = false;

    private void Update()
    {
        overThreshold = currentContainerAngle() > pourThreshold;

        if (isPouring != overThreshold)
        {
            isPouring = overThreshold;
            if (isPouring)
            {
                startPouring();
            }
            else
            {
                stopPouring();
            }
        }
    }

    private void startPouring()
    {
        print("start");
        emitter.SetActive(true);

    }

    private void stopPouring()
    {
        print("stop");
        emitter.SetActive(false);
    }

    private float currentContainerAngle()
    {
        //float currentDegree = Mathf.Abs(transform.forward.y * Mathf.Rad2Deg);
        float currentDegree = Vector3.Angle(Vector3.up, transform.up);
        //print(" y degree" + currentDegree);
        return currentDegree;
    }

}