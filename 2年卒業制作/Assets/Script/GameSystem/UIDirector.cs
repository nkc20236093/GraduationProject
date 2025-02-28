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
                gameText.text = "�d�C���ʂ��ĂȂ�";
                yield return wait;
                break;
            case 1:
                gameText.text = "�ǂ����̃h�A���J����";
                yield return wait;
                break;
            case 2:
                gameText.text = "�ʓd��";
                yield return wait2;
                break;
                //case 2:
                //    gameText.text = "�����w��̌`�ɂ��܂��傤\n�}�E�X�z�C�[���Ő���I��\n���N���b�N�E�E�N���b�N�ňړ�";
                //    yield return waitInput;
                //    break;
        }
        gameText.text = "";
        yield break;
    }
}
