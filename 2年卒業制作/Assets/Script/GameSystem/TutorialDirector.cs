using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TutorialDirector : MonoBehaviour
{
    tutorialOrigin[] tutorialOrigins=new tutorialOrigin[4];
    public bool[] tutorialFlags = new bool[4];
    bool tutorialEnd = false;
    bool tutorialRunning = false;
    public string text;
    [SerializeField] PlayerController playerController;
    [SerializeField] UIDirector uIDirector;
    public class tutorialOrigin
    {
        protected PlayerController controller;
        protected int myNumber = 0;
        protected UIDirector uiDirector;
        protected TutorialDirector tutorialDirector;
        public tutorialOrigin(int num, UIDirector ui, TutorialDirector director)
        {
            myNumber = num;
            uiDirector = ui;
            tutorialDirector = director;
        }
        public virtual void tutorial() { }
    }
    public class jankenTutorialScissors : tutorialOrigin
    {
        public jankenTutorialScissors(int num, UIDirector ui, TutorialDirector director, PlayerController player) : base(num, ui, director)
        {
            controller = player;
        }
        public override void tutorial()
        {
            tutorialDirector.text = "手の形をチョキに変えてレーザーを出してみましょう";
            if (controller.GetSelect() == 1 && Input.GetMouseButton(0))  
            {
                tutorialDirector.tutorialFlags[myNumber] = true;
            }
        }
    }
    public class jankenTutorialPaper : tutorialOrigin
    {
        public jankenTutorialPaper(int num, UIDirector ui, TutorialDirector director, PlayerController player) : base(num, ui, director)
        {
            controller = player;
        }
        public override void tutorial()
        {
            tutorialDirector.text = "手の形をパーに変えて動いてみましょう";
            if (controller.GetSelect() == 2 && Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")) 
            {
                tutorialDirector.tutorialFlags[myNumber] = true;
            }
        }
    }
    public class jankenTutorialRock : tutorialOrigin
    {
        public jankenTutorialRock(int num, UIDirector ui, TutorialDirector director, PlayerController player) : base(num, ui, director)
        {
            controller = player;
        }
        public override void tutorial()
        {
            tutorialDirector.text = "手の形をグーに変えて叩いてみましょう";
            if (controller.GetSelect() == 0 && Input.GetMouseButtonDown(0)) 
            {
                tutorialDirector.tutorialFlags[myNumber] = true;
            }
        }
    }

    public class LightTutorial : tutorialOrigin
    {
        GameDirector gameDirector;
        public LightTutorial(int num, UIDirector ui, TutorialDirector director, GameDirector game) : base(num, ui, director)
        {
            gameDirector = game;
        }
        public override void tutorial()
        {
            tutorialDirector.text = "手の形をチョキに変えてレーザーを配電盤に当てましょう";
            if (PlayerController.stop) 
            {
                tutorialDirector.text = "配電盤の線を動かして指定の形にしましょう\nマウスの左右のクリックで線を移動できます\nマウスカーソルを回すと移動させる線を変えれます\nescキーで終了";
                if (gameDirector.gimmickClearFlags[0])
                {
                    tutorialDirector.tutorialFlags[myNumber] = true;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameDirector gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        tutorialOrigins[0] = new jankenTutorialRock(0, uIDirector, this, playerController);
        tutorialOrigins[1] = new jankenTutorialPaper(1, uIDirector, this, playerController);
        tutorialOrigins[2] = new jankenTutorialScissors(2, uIDirector, this, playerController);
        tutorialOrigins[3] = new LightTutorial(3, uIDirector, this, gameDirector);
        StartCoroutine(TutorialCor(0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(playerController.GetSelect());
        }
        if (tutorialEnd)
        {
            // ここで最初のエリアの扉を開ける
        }
    }
    IEnumerator TutorialCor(int count)
    {
        if (tutorialEnd) yield break;
        while (!tutorialFlags[count])
        {
            if (!tutorialRunning)
            {
                tutorialRunning = true;
                tutorialOrigins[count].tutorial();
                yield return null;
                if (tutorialFlags[count])
                {
                    tutorialRunning = false;
                    if (AllConditionsTrue())
                    {
                        tutorialEnd = true;
                        yield break;
                    }
                    else
                    {
                        count++;
                        yield return StartCoroutine(TutorialCor(count));
                    }
                }
                else
                {
                    tutorialRunning = false;
                    yield return StartCoroutine(TutorialCor(count));
                }
            }
        }
    }
    public bool AllConditionsTrue()
    {
        foreach (bool flag in tutorialFlags)
        {
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }
}
