using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public bool[] gimmickClearFlags = new bool[4];
    public bool chase = false;
    [SerializeField] AudioSource audioSource;
    [Header("0 = 通常\n1 = 追跡中")]
    [SerializeField] AudioClip[] clips;
    [SerializeField] UIDirector uIDirector;
    [SerializeField] TransitionPostEffect effect;
    [SerializeField] Image[] gimmickImages;
    [SerializeField] GameObject Exit;
    [SerializeField] GameObject TutorialExit;
    public Image gimmickImage;
    bool oneAction = false;
    // Start is called before the first frame update
    void Start()
    {
        //int i = Random.Range(0, 2);
        //gimmickImage = gimmickImages[i];
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = GameManager.instance.audiovolumes[1];
        if (chase && audioSource.clip != clips[1])
        {
            audioSource.clip = clips[1];
            audioSource.Play();
        }
        else if (audioSource.clip != clips[0]) 
        {
            audioSource.clip = clips[0];
            audioSource.Play();
        }
        for(int i = 0; i < gimmickClearFlags.Length; i++)
        {
            switch(i)
            {
                case 0:
                    Destroy(TutorialExit);
                    break;
                case 1:
                    Exit.tag = "Exit";
                    break;
            }
        }
    }
    public void GimmickEvent(int number)
    {
        Debug.Log("ギミッククリアによるイベント");
        gimmickClearFlags[number] = true;
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
