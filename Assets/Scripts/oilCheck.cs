using System.Collections;
using System.Collections.Generic;
using com.zibra.liquid.Manipulators;
using com.zibra.smoke_and_fire.Manipulators;
using UnityEngine;

public class oilCheck : MonoBehaviour
{
    private ZibraLiquidVoid deletedLiquid;
    public float reactThreshold = 100;
    private long currentDetectedParticles;
    public bool oilReady;
    public GameObject insuct1;
    public GameObject insuct2;
    // Start is called before the first frame update
    void Start()
    {
        deletedLiquid = GetComponent<ZibraLiquidVoid>();
        currentDetectedParticles = 0;
        oilReady = false;
        insuct1.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        currentDetectedParticles = deletedLiquid.DeletedParticleCountTotal;

        if (currentDetectedParticles > reactThreshold)
        {
            oilReady = true;
            insuct1.SetActive(false);
            insuct2.SetActive(true);
        }
    }
    public bool oilStatus()
    {
        return oilReady;
    }
}
