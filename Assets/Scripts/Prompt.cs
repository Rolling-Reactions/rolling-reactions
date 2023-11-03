using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prompt : MonoBehaviour
{
    public WheelchairAxleController wheelchair;
    private Canvas canvas;
    private RawImage image;
    private TMP_Text text;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>(true);
        image = canvas.gameObject.GetComponentInChildren<RawImage>(true);
        text = canvas.gameObject.GetComponentInChildren<TMP_Text>(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        canvas.gameObject.SetActive(true);
        if (wheelchair.isKinematic)
        {
            image.color = new Color(0.5f, 0.1f, 0.1f);
            text.text = "Exit experiment";
        }
        else
        {
            image.color = new Color(0.5f, 0.7f, 0.1f);
            text.text = "Enter experiment";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canvas.gameObject.SetActive(false);
    }
}
