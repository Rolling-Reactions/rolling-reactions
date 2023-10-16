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

    // Start is called before the first frame update
    void Start()
    {
        deletedLiquid = GetComponent<ZibraLiquidVoid>();
        currentDetectedParticles = 0;
        setFire = false;
        fireStarted = false;
    }

// Update is called once per frame
    void Update()
    {
        currentDetectedParticles = deletedLiquid.DeletedParticleCountTotal;
        oilcheck = oidvoid.GetComponent<oilCheck>().oilReady;
        if (currentDetectedParticles > reactThreshold)
        {
            setFire = true;
        }
        if (!fireStarted&&oilcheck&&setFire)
        {
            fireStarted = true;
            startSmoke();
        }
    }

    private void startSmoke()
    {
        print("smoke start");
        fireSmoke = fire.GetComponent<ZibraSmokeAndFireEmitter>();
        fire.SetActive(true);
        Invoke("startFire", 5f);
    }

    private void startFire()
    {
        print("fire start");
        fireSmoke = fire.GetComponent<ZibraSmokeAndFireEmitter>();
        //todo: change them gradually in the future to make it look more realistics.
        fireSmoke.EmitterTemperature = 0.6f;
        fireSmoke.EmitterFuel = 0.2f;
    }
}

