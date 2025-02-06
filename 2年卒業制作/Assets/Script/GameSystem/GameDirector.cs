using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
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
    public void GimmickEvent()
    {
        Debug.Log("ギミッククリアによるイベント");
    }
    public void Dead()
    {
        uIDirector.DeadUI();
    }
}
