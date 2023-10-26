using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    private RawImage image;
    private bool isFading;
    private float fadeStart;
    private float fadeStop;

    private float blackTime = 0.5f;

    void Start()
    {
        image = GetComponent<RawImage>();
        image.enabled = false;
    }
    void Update()
    {
        if (isFading)
        {
            float t = Mathf.Clamp01((Time.time - (fadeStart + blackTime)) / (fadeStop - (fadeStart + blackTime)));
            image.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - t);
            if (t >= 1.0f)
            {
                isFading = false;
                image.enabled = false;
            }
        }
    }

    public void Fade(float fadeTime)
    {
        if (Time.time > fadeStop)
        {
            isFading = true;
            image.enabled = true;
            image.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            fadeStart = Time.time;
            fadeStop = fadeStart + fadeTime;
        }
    }
}
