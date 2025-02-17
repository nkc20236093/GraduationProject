using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleDirector : MonoBehaviour
{
    [Header("0 = SE\n1 = BGM")]
    [SerializeField] AudioSource[] audioSource = new AudioSource[0];
    [SerializeField] Slider[] volumeSlider = new Slider[2];
    [SerializeField] GameObject settingExit;
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider[0].value = 0.5f;
        volumeSlider[1].value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.audiovolumes[0] = volumeSlider[0].value;
        GameManager.instance.audiovolumes[1] = volumeSlider[1].value;
        audioSource[1].volume = volumeSlider[1].value;
    }
    public void OptionON()
    {
        volumeSlider[0].gameObject.SetActive(true);
        volumeSlider[1].gameObject.SetActive(true);
        settingExit.SetActive(true);
    }
    public void OptionOFF()
    {
        volumeSlider[0].gameObject.SetActive(false);
        volumeSlider[1].gameObject.SetActive(false);
        settingExit.SetActive(false);
    }
    public void GameStart()
    {

    }
    public void VolumeCheck()
    {
        audioSource[1].Play();
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
