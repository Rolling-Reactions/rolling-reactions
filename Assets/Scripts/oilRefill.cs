using System;
using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Manipulators;
using UnityEngine;
//using static UnityEditor.Rendering.CoreEditorDrawer<TData>;

public class oilRefill : MonoBehaviour
{
    private ZibraLiquidEmitter createdLiquid;
    public GameObject emitter;
    private ZibraLiquidVoid deletedLiquid;
    public GameObject oilVoid;
    public float turnOffThreshold = 1500;
    public float currentParticles;
    public bool refill;
    public bool isHeld;  

    // Start is called before the first frame update
    void Start()
    {
        createdLiquid = emitter.GetComponent<ZibraLiquidEmitter>();
        deletedLiquid = oilVoid.GetComponent<ZibraLiquidVoid>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: add a button to refill the glass. ex.refill = Input.GetKeyDown(KeyCode.Space);
        currentParticles = createdLiquid.CreatedParticlesTotal - deletedLiquid.DeletedParticleCountTotal;
        if (currentParticles > turnOffThreshold)
        {
            emitter.SetActive(false);
        }
        else if(refill)
        {
            emitter.SetActive(true);
        }
    }

    public void OnSelectEntered()
    {
        isHeld = true;
    }

    public void OnSelectExited()
    {
        isHeld = false;
    }
}
