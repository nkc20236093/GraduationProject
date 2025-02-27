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
        StartCoroutine(Tutorial(corStop));
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
    IEnumerator Tutorial(bool stop)
    {
        gameText.enabled = true;
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
        switch (num)
        {
            case 6:
                gameText.enabled = true;
                gameText.text = "鍵がかかってる";
                yield return new WaitForSeconds(1.5f);
                gameText.text = "";
                gameText.enabled = false;
                break;
            case 1:
                gameText.enabled = true;
                gameText.text = "どこかのドアが開いた";
                yield return new WaitForSeconds(1.5f);
                gameText.text = "";
                gameText.enabled = false;
                break;
            case 2:
                gameText.enabled = true;
                gameText.text = "線を指定の形にしましょう\nマウスホイールで線を選択\n左クリック・右クリックで移動";
                yield return new WaitUntil(() => !PlayerController.stop);
                gameText.text = "";
                gameText.enabled = false;
                break;
        }
        yield break;
    }
}
