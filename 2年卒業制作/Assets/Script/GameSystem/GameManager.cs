using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("0 = SEボリューム\n1 = BGMボリューム")]
    /// <summary>
    /// 0 = SEボリューム
    /// 1 = BGMボリューム
    /// </summary>
    public float[] audiovolumes = new float[2];
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移時にオブジェクトが破棄されないように設定
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
