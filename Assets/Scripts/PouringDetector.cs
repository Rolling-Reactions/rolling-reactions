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
    public bool isHeld; //TODO: add detection for glasshold.

    public AudioSource audioS;
    public AudioClip waterSound;


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
        audioS.clip = waterSound;
        audioS.volume = 0.3f;
        audioS.loop = true;
        audioS.Play();

    }

    private void stopPouring()
    {
        print("stop");
        emitter.SetActive(false);
        audioS.loop = false;
        audioS.Stop();
    }

    private float currentContainerAngle()
    {
        //float currentDegree = Mathf.Abs(transform.forward.y * Mathf.Rad2Deg);
        float currentDegree = Vector3.Angle(Vector3.up, transform.up);
        //print(" y degree" + currentDegree);
        return currentDegree;
    }

}