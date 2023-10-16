using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RestartManager : MonoBehaviour
{
    public SteamVR_Action_Boolean menuButtonAction; // SteamVR menu button action
    public string sceneToRestart; // Name of the scene to restart
    public GameObject wheelchair;

    private void Update()
    {
        // Check if the menu button on either controller is pressed
        if (menuButtonAction.GetStateDown(SteamVR_Input_Sources.LeftHand) ||
            menuButtonAction.GetStateDown(SteamVR_Input_Sources.RightHand) || Input.GetKey(KeyCode.R))
        {
            // Restart the scene
            //UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToRestart);
            Destroy(wheelchair);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}