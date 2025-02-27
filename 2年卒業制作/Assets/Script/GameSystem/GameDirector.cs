using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public bool[] gimmickClearFlags = new bool[5];
    [SerializeField] AudioSource audioSource;
    [SerializeField] EnemyCon[] enemyCon;
    [Header("0 = 通常\n1 = 追跡中")]
    [SerializeField] AudioClip[] clips;
    [SerializeField] UIDirector uIDirector;
    [SerializeField] TransitionPostEffect effect;
    [SerializeField] GameObject Exit;
    [SerializeField] GameObject TutorialExit;
    [SerializeField] GameObject[] GimmickDoors = new GameObject[4];
    bool oneAction = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = GameManager.instance.audiovolumes[1];
        if (CheckALLTRUE() && audioSource.clip != clips[1])
        {
            Debug.Log("chase");
            audioSource.clip = clips[1];
            audioSource.Play();
        }
        else if (audioSource.clip != clips[0])
        {
            Debug.Log("notChase");
            audioSource.clip = clips[0];
            audioSource.Play();
        }
    }
    bool CheckALLTRUE()
    {
        foreach (EnemyCon enemy in enemyCon)
        {
            if (enemy.GetBool())
            {
                return true;
            }
        }
        return false;
    }
    public void GimmickEvent(int number)
    {
        gimmickClearFlags[number] = true;
        for (int i = 0; i < gimmickClearFlags.Length; i++)
        {
            if (gimmickClearFlags[i])
            {
                switch (i)
                {
                    case 0:
                        Destroy(TutorialExit);
                        break;
                    case 1:
                        Destroy(GimmickDoors[i - 1]);
                        break;
                    case 2:
                        Destroy(GimmickDoors[i - 1]);
                        break;
                    case 3:
                        Destroy(GimmickDoors[i - 1]);
                        break;
                    case 6:
                        Exit.tag = "Exit";
                        break;
                }
            }
        }
    }
    public void GameClear()
    {
        if (oneAction) return;
        oneAction = true;
        Debug.Log("ゲームクリア");
        effect.StartCoroutine(effect.Transition(0));
    }
    public void GameOver()
    {
        if (oneAction) return;
        oneAction = true;
        Debug.Log("ゲームオーバー");
        effect.StartCoroutine(effect.Transition(1));
    }
}
