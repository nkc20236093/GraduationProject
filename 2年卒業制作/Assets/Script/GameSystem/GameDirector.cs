using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject DeadUI;
    [SerializeField] GameObject ClearUI;
    // Start is called before the first frame update
    void Start()
    {
        DeadUI.SetActive(false);
        ClearUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeadPerformance()
    {
        DeadUI.SetActive(true);
    }
    public void Clear()
    {
        ClearUI.SetActive(true);
    }
    public void GimmickEvent()
    {
        Debug.Log("ギミッククリアによるイベント");
    }
}
