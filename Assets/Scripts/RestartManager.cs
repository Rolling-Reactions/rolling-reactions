using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class RestartManager : MonoBehaviour
{
    private InputData inputData;
    public GameObject wheelchair;

    private void Start()
    {
        inputData = this.GetComponent<InputData>();
    }

    private void Update()
    {
        // Check if the menu button on either controller is pressed
        if ((inputData.controllers[0].TryGetFeatureValue(CommonUsages.menuButton, out bool menuLeft) && menuLeft) ||
            (inputData.controllers[1].TryGetFeatureValue(CommonUsages.menuButton, out bool menuRight) && menuRight))
        {
            Destroy(wheelchair);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}