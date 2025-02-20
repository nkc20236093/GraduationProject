using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("0 = SE�{�����[��\n1 = BGM�{�����[��")]
    /// <summary>
    /// 0 = SE�{�����[��
    /// 1 = BGM�{�����[��
    /// </summary>
    public float[] audiovolumes = new float[2];
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �V�[���J�ڎ��ɃI�u�W�F�N�g���j������Ȃ��悤�ɐݒ�
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
