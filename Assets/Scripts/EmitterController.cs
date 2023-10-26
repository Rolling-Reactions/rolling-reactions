using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Manipulators;

public class EmitterController : MonoBehaviour
{
    private ZibraLiquidEmitter emitter;
    void Start()
    {
        emitter = GetComponent<ZibraLiquidEmitter>();
    }

    public void ToggleEmitter()
    {
        emitter.enabled = !emitter.enabled;
    }
}
