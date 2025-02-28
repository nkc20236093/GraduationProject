using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDirector : MonoBehaviour
{
    private bool corStop = false;
    Color[] damageColors = new Color[2]
    {
        new Color(0.7f, 0, 0, 0.5f),
        new Color(0.7f, 0, 0, 0.7f)
    };
    [SerializeField] TutorialDirector tutorialDirector;
    [SerializeField] Image DamageImg;
    [SerializeField] Text gameText;
    [SerializeField] Sprite[] JankenUIs = new Sprite[3];
    [SerializeField] Image JankenUI;
    void Start()
    {
        DamageImg.color = Color.clear;
    }


    void Update()
    {
        if(DamageImg.color != Color.clear)
        {
            DamageImg.color = Color.Lerp(DamageImg.color, Color.clear, Time.deltaTime);
        }
    }
    public void ChangeJankenUI(int num)
    {
        JankenUI.sprite = JankenUIs[num];
    }

    public void Damaged(int num)
    {
        DamageImg.color = damageColors[num - 1];
    }
    public IEnumerator Tutorial(bool stop)
    {
        while (!stop)
        {
            for (int i = 0; i < tutorialDirector.tutorialFlags.Length; i++)
            {
                if (tutorialDirector.AllConditionsTrue())
                {
                    gameText.enabled = false;
                    yield break;
                }
                else if (!tutorialDirector.tutorialFlags[i])
                {
                    gameText.text = tutorialDirector.text;
                    yield return null;
                }
            }
        }
    }
    public IEnumerator Message(int num)
    {
        if(corStop) yield break;
        corStop = true;
        var wait = new WaitForSeconds(3.5f);
        var wait2 = new WaitForSeconds(5.0f);
        switch (num)
        {
            case 0:
                gameText.text = "電気が通ってない";
                yield return wait;
                break;
            case 1:
                gameText.text = "どこかのドアが開いた";
                yield return wait;
                break;
            case 2:
                gameText.text = "通電中";
                yield return wait2;
                break;
                //case 2:
                //    gameText.text = "線を指定の形にしましょう\nマウスホイールで線を選択\n左クリック・右クリックで移動";
                //    yield return waitInput;
                //    break;
        }
        gameText.text = "";
        yield break;
    }
}
