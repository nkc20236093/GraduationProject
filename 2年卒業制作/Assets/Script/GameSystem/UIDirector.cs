using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDirector : MonoBehaviour
{
    private bool corStop = false;
    [SerializeField] TutorialDirector tutorialDirector;
    [SerializeField] Image DamageImg;
    [SerializeField] Text describeText;
    void Start()
    {
        DamageImg.color = Color.clear;
        StartCoroutine(Tutorial(corStop));
    }


    void Update()
    {
        if(DamageImg.color != Color.clear)
        {
            DamageImg.color = Color.Lerp(DamageImg.color, Color.clear, Time.deltaTime);
        }
    }

    public void Damaged()
    {
        DamageImg.color = new Color(0.7f, 0, 0, 0.7f);
    }
    public void DeadUI()
    {

    }
    IEnumerator Tutorial(bool stop)
    {
        describeText.enabled = true;
        while (!stop)
        {
            for (int i = 0; i < tutorialDirector.tutorialFlags.Length; i++)
            {
                if (!tutorialDirector.tutorialFlags[i])
                {
                    describeText.text = tutorialDirector.text;
                    yield return null;
                    break;
                }
            }
        }
        describeText.enabled = false;
    }
}
