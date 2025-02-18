using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleDirector : MonoBehaviour
{
    [Header("0 = SE\n1 = BGM")]
    [SerializeField] TransitionPostEffect effect;
    [SerializeField] GameObject[] firstButton = new GameObject[3];
    [SerializeField] AudioSource[] audioSource = new AudioSource[2];
    [SerializeField] Slider[] volumeSlider = new Slider[2];
    [SerializeField] GameObject settingExit;
    [SerializeField] GameObject gameEndUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.audiovolumes[0] = volumeSlider[0].value;
        GameManager.instance.audiovolumes[1] = volumeSlider[1].value;
        audioSource[1].volume = volumeSlider[1].value;
        audioSource[0].volume = volumeSlider[0].value;
    }
    public void OptionON()
    {
        for (int i = 0; i < firstButton.Length; i++) 
        {
            firstButton[i].SetActive(false);
        }
        volumeSlider[0].gameObject.SetActive(true);
        volumeSlider[1].gameObject.SetActive(true);
        settingExit.SetActive(true);
    }
    public void OptionOFF()
    {
        for (int i = 0; i < firstButton.Length; i++)
        {
            firstButton[i].SetActive(true);
        }
        volumeSlider[0].gameObject.SetActive(false);
        volumeSlider[1].gameObject.SetActive(false);
        settingExit.SetActive(false);
    }
    public void GameChange()
    {
        gameEndUI.SetActive(true);
        effect.StartTransition(0, "SampleScene");
    }
    public void VolumeCheck()
    {
        audioSource[0].Play();
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
