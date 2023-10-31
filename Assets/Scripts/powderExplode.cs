using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Manipulators;
using com.zibra.smoke_and_fire.Manipulators;
using System;
using com.zibra.common;

public class powderExplode : MonoBehaviour
{
    private ZibraLiquidVoid deletedLiquid;
    private ZibraSmokeAndFireEmitter fireSmoke;
    public float reactThreshold = 100;
    private long currentDetectedParticles;
    public GameObject oidvoid;
    public bool oilcheck;
    public bool setFire;
    public bool fireStarted;
    public GameObject fire;
    // instruction
    public GameObject insuct2; //water instuction
    public GameObject insuct3; //fire instuction
    public GameObject waterGlass; //water
    public GameObject oilGlass; //glass
    private bool isHeld; // water/oil bottle is held

    public AudioSource audioS;
    public AudioClip smokeSound, fireSound;

    // Start is called before the first frame update
    void Start()
    {
        deletedLiquid = GetComponent<ZibraLiquidVoid>();
        currentDetectedParticles = 0;
        setFire = false;
        fireStarted = false;
        insuct2.SetActive(false);
    }

// Update is called once per frame
    void Update()
    {
        currentDetectedParticles = deletedLiquid.DeletedParticleCountTotal;
        oilcheck = oidvoid.GetComponent<oilCheck>().oilReady;
        if (currentDetectedParticles > reactThreshold)
        {
            setFire = true;
            insuct2.SetActive(false);
        }
        if (!fireStarted&&oilcheck&&setFire)
        {
            fireStarted = true;
            startSmoke();
        }

        isHeld = waterGlass.GetComponent<PouringDetector>().isHeld || oilGlass.GetComponent<oilRefill>().isHeld;
        //when the glass of water or oil is held, guide the user to pour it on the powder
        if (isHeld && !fireStarted)
        { 
            insuct3.SetActive(true);
        }
        else
        {
            insuct3.SetActive(false);
        }
    }

    private void startSmoke()
    {
        print("smoke starts");
        fireSmoke = fire.GetComponent<ZibraSmokeAndFireEmitter>();
        fire.SetActive(true);
        audioS.clip = smokeSound;
        audioS.volume = 0.1f;
        audioS.Play();
        Invoke("startFire", 5f);
    }

    private void startFire()
    {
        print("fire starts");
        fireSmoke = fire.GetComponent<ZibraSmokeAndFireEmitter>();
        //todo: change them gradually in the future to make it look more realistics.
        fireSmoke.EmitterTemperature = 0.6f;
        fireSmoke.EmitterFuel = 0.2f;
        audioS.volume = 1f;
        audioS.clip = fireSound;
        audioS.Play();
        Invoke("putOffFire", 10f);
    }

    private void putOffFire()
    {
        print("fire ends");
        fireSmoke = fire.GetComponent<ZibraSmokeAndFireEmitter>();
        //todo: change them gradually in the future to make it look more realistics.
        fireSmoke.EmitterTemperature = 0.2f;
        fireSmoke.EmitterFuel = 0.0f;
    }

}

