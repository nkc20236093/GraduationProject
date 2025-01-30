using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject backUI;
    // Start is called before the first frame update
    void Start()
    {
        backUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeadPerformance()
    {
        backUI.SetActive(true);
    }
}
