using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.zibra.liquid.Manipulators;

public class EmitterController : MonoBehaviour
{
    private ZibraLiquidEmitter emitter;
    private AudioSource audio;
    void Start()
    {
        emitter = GetComponent<ZibraLiquidEmitter>();
        audio = GetComponent<AudioSource>();
    }

    public void ToggleEmitter()
    {
        emitter.enabled = !emitter.enabled;
        if (emitter.enabled) audio.Play();
        else audio.Stop();
    }
}
