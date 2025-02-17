using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public bool[] gimmickClearFlags = new bool[4];
    [SerializeField] UIDirector uIDirector;
    [SerializeField] TransitionPostEffect effect;
    bool oneAction = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GimmickEvent(int number)
    {
        Debug.Log("�M�~�b�N�N���A�ɂ��C�x���g");
        gimmickClearFlags[number] = true;
    }
    public void GameClear()
    {
        if (oneAction) return;
        oneAction = true;
        Debug.Log("�Q�[���N���A");
        effect.StartCoroutine(effect.Transition(0));
    }
    public void GameOver()
    {
        if (oneAction) return;
        oneAction = true;
        Debug.Log("�Q�[���I�[�o�[");
        effect.StartCoroutine(effect.Transition(1));
    }
}
