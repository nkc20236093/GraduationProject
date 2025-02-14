using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDirector : MonoBehaviour
{
    tutorialOrigin[] tutorialOrigins=new tutorialOrigin[4];
    public bool[] tutorialFlags = new bool[4];
    bool tutorialEnd = false;
    bool tutorialRunning = false;
    public string text;
    [SerializeField] UIDirector uIDirector;
    public class tutorialOrigin
    {
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
        public jankenTutorialScissors(int num, UIDirector ui, TutorialDirector director) : base(num, ui, director) { }
        public override void tutorial()
        {
            tutorialDirector.text = "じゃんけんチュートリアル:チョキ";
            while (!tutorialDirector.tutorialFlags[myNumber])
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log("進行");
                    tutorialDirector.tutorialFlags[myNumber] = true;
                }
            }
        }
    }
    public class jankenTutorialPaper : tutorialOrigin
    {
        public jankenTutorialPaper(int num, UIDirector ui, TutorialDirector director) : base(num, ui, director) { }
        public override void tutorial()
        {
            tutorialDirector.text = "じゃんけんチュートリアル:パー";
            while (!tutorialDirector.tutorialFlags[myNumber])
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log("進行");
                    tutorialDirector.tutorialFlags[myNumber] = true;
                }
            }
        }
    }
    public class jankenTutorialRock : tutorialOrigin
    {
        public jankenTutorialRock(int num, UIDirector ui, TutorialDirector director) : base(num, ui, director)
        {
            myNumber = num;
            uiDirector = ui;
            tutorialDirector = director;
        }
        public override void tutorial()
        {
            Debug.Log("待機");

            tutorialDirector.text = "じゃんけんチュートリアル:グー";
            while (!tutorialDirector.tutorialFlags[myNumber])
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log("進行");
                    tutorialDirector.tutorialFlags[myNumber] = true;
                }
            }
        }
    }

    public class LightTutorial : tutorialOrigin
    {
        public LightTutorial(int num, UIDirector ui, TutorialDirector director) : base(num, ui, director) { }
        public override void tutorial()
        {
            Debug.Log("待機");

            tutorialDirector.text = "ライトチュートリアル";
            while (!tutorialDirector.tutorialFlags[myNumber])
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log("進行");
                    tutorialDirector.tutorialFlags[myNumber] = true;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tutorialOrigins[0] = new jankenTutorialRock(0, uIDirector, this);
        tutorialOrigins[1] = new jankenTutorialPaper(1, uIDirector, this);
        tutorialOrigins[2] = new jankenTutorialScissors(2, uIDirector, this);
        tutorialOrigins[3] = new LightTutorial(3, uIDirector, this);
        StartCoroutine(TutorialCor(0));
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator TutorialCor(int count)
    {
        if (!tutorialRunning)
        {
            tutorialRunning = true;
            tutorialOrigins[count].tutorial();
            yield return new WaitUntil(() => tutorialFlags[count]);
            tutorialRunning = false;
            if (!AllConditionsTrue())
            {
                count++;
                yield return StartCoroutine(TutorialCor(count));
            }
            else
            {
                tutorialEnd = false;
                yield break;
            }
        }
    }
    bool AllConditionsTrue()
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
