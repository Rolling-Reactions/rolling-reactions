using System.Collections;
using System.Collections.Generic;
using com.zibra.smoke_and_fire.Manipulators;
using UnityEngine;
using UnityEngine.UI;

public class instuctUpdate : MonoBehaviour
{
    public Text m_MyText;

    public GameObject powder;
    public bool isStep1Done;
    public bool isStep2Done;
    public bool isStep3Done;

    private bool isStep1SoundDone;
    private bool isStep2SoundDone;
    private bool isStep3SoundDone;


    public AudioSource audioS;
    public AudioClip feedbackSound, negativeFBSound, successFBSound;

    void Start()
    {
        //Text sets your text to say this message
        m_MyText.text = "Step 1: Pour glycerol on potassium permanganate \n                 ☐\n              ----\nStep 2: Add a few drops of water to accelerate the process\n                 ☐\n              ----\nStep 3: Wait 5 seconds\n                 ☐";
        isStep1SoundDone = false;
        isStep2SoundDone = false;
        isStep3SoundDone = false;
    }

    void Update()
    {
        isStep1Done = powder.GetComponent<powderExplode>().oilcheck;
        isStep2Done = powder.GetComponent<powderExplode>().setFire;
        isStep3Done = powder.GetComponent<powderExplode>().fireStarted;

        //Press the space key to change the Text message
        if (isStep1Done)
        {
            m_MyText.text = "Step 1: Pour glycerol on potassium permanganate \n                 ☑\n              ----\nStep 2: Add a few drops of water to accelerate the process\n                 ☐\n              ----\nStep 3: Wait 5 seconds\n                 ☐";
            if (!isStep1SoundDone)
            {
                isStep1SoundDone = true;
                feedbackSoundPlay();
            }
        }
        if (!isStep1Done && isStep2Done)
        {
            m_MyText.text = "Step 1: Pour glycerol on potassium permanganate \n                 ☐\n              ----\nStep 2: Add a few drops of water to accelerate the process\n                 ☑\n              ----\nStep 3: Wait 5 seconds\n                 ☐";
            if (!isStep2SoundDone)
            {
                isStep2SoundDone = true;
                negativeFeedbackSoundPlay();
            }
        }

        if (isStep1Done && isStep2Done)
        {
            m_MyText.text = "Step 1: Pour glycerol on potassium permanganate \n                 ☑\n              ----\nStep 2: Add a few drops of water to accelerate the process\n                 ☑\n              ----\nStep 3: Wait 5 seconds\n                 ☐";
            if (!isStep2SoundDone)
            {
                isStep2SoundDone = true;
                feedbackSoundPlay();
            }
        }
        if (isStep3Done)
        {
            Invoke("startChangeStep3", 5f);
        }
    }

    private void startChangeStep3()
    {
        m_MyText.text = "Step 1: Pour glycerol on potassium permanganate \n                 ☑\n              ----\nStep 2: Add a few drops of water to accelerate the process\n                 ☑\n              ----\nStep 3: Wait 5 seconds\n                 ☑";
        if (!isStep3SoundDone)
        {
            isStep3SoundDone = true;
            successFeedbackSoundPlay();
        }
    }

    private void feedbackSoundPlay()
    {
        audioS.clip = feedbackSound;
        audioS.Play();
    }


    private void negativeFeedbackSoundPlay()
    {
        audioS.clip = negativeFBSound;
        audioS.Play();
    }

    private void successFeedbackSoundPlay()
    {
        audioS.clip = successFBSound;
        audioS.Play();
    }

}