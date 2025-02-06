using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDirector : MonoBehaviour
{
    [SerializeField] Image DamageImg;
    void Start()
    {
        DamageImg.color = Color.clear;
    }


    void Update()
    {
        if(DamageImg.color != Color.clear)
        {
            DamageImg.color = Color.Lerp(DamageImg.color, Color.clear, Time.deltaTime);
        }
    }

    public void Damaged()
    {
        DamageImg.color = new Color(0.7f, 0, 0, 0.7f);
    }
    public void DeadUI()
    {

    }
}
