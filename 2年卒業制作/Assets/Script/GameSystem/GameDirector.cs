using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public bool[] gimmickClearFlags = new bool[4];
    [SerializeField] UIDirector uIDirector;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            uIDirector.Damaged();
        }
    }
    public void GimmickEvent(int number)
    {
        Debug.Log("ギミッククリアによるイベント");
        switch (number)
        {
            case 0:
                gimmickClearFlags[number] = true;
                break;
        }
    }
    public void Dead()
    {
        uIDirector.DeadUI();
    }
}
