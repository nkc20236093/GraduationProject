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
            tutorialDirector.text = "��̌`���`���L�ɕς��ă��[�U�[���o���Ă݂܂��傤";
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
            tutorialDirector.text = "��̌`���p�[�ɕς��ē����Ă݂܂��傤";
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
            tutorialDirector.text = "��̌`���O�[�ɕς��Ē@���Ă݂܂��傤";
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
            tutorialDirector.text = "��̌`���`���L�ɕς��ă��[�U�[��z�d�Ղɓ��Ă܂��傤";
            if (PlayerController.stop) 
            {
                tutorialDirector.text = "�z�d�Ղ̐��𓮂����Ďw��̌`�ɂ��܂��傤\n�}�E�X�̍��E�̃N���b�N�Ő����ړ��ł��܂�\n�}�E�X�J�[�\�����񂷂ƈړ����������ς���܂�\nesc�L�[�ŏI��";
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
            // �����ōŏ��̃G���A�̔����J����
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
