using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[4];
    [SerializeField] int gimickNumber;

    [Header("配電盤用linerenderer\n赤、緑、紫の順番")]
    [SerializeField] GameObject[] pillers;
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
    [Header("最初の位置、リセットするたびに設定\n上の位置とする")]
    [SerializeField] Vector3[] firstSetPosition;
    public class originGimmick
    {
        protected int myNumber = 0;
        public originGimmick(){}
        public virtual void Operation() { }
    }
    public class Gimmick01 : originGimmick
    {
        public Gimmick01()
        {

        }
        public override void Operation()
        {
            Debug.Log("ライトヒット!");
        }
    }
    public class Gimmick02 : originGimmick
    {
        Vector3[] firstPosition;
        GameObject[] cables;
        LineRenderer[] colorLineRenderer = new LineRenderer[3];
        GameObject[] pillers;

        bool first = false;
        int colorNumber = 0;
        RaycastHit hit;
        public Gimmick02(LineRenderer[] lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers)
        {
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
        }
        public override void Operation()
        {
            Debug.Log("あやとり");
            // 仮にEscapeキーを押したら終了
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    colorLineRenderer[i].enabled = false;
                }
                // プレイヤーも操作可能に戻す
                PlayerController.stop = false;
                first = false;
            }
            else
            {
                // 起動時に一回だけ実行
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        colorLineRenderer[i].SetPositions(firstPosition);
                    }
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer[i].enabled)
                        {
                            colorLineRenderer[i].enabled = true;
                        }
                    }
                    for (int i = 0; i < pillers.Length; i++)
                    {
                        pillers[i].SetActive(true);
                    }
                    first = true;
                }

                // 仮にFとHで弄る線を変えるとしたら
                if (Input.GetKeyDown(KeyCode.H))
                {
                    colorNumber++;
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    colorNumber--;
                }
                colorNumber = Mathf.Clamp(colorNumber, 0, 2);
                Debug.Log(colorNumber);
                //Debug.Log(colorLineRenderer[colorNumber].positionCount - 1);
                //Debug.Log(colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1));
                float value = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1).x;
                // 仮にMとBで線を動かすとしたら
                if (Input.GetKey(KeyCode.M))
                {
                    value += 5;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value -= 5;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer[colorNumber].SetPosition(colorLineRenderer[colorNumber].positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer[colorNumber].GetPosition(0);
                Vector3 endPos = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1);
                Vector3 direction = endPos - startPos;
                Ray ray = new Ray(startPos, direction);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("Pillar"))
                    {
                        Vector3[] points = new Vector3[colorLineRenderer[colorNumber].positionCount + 1];
                        points[0] = startPos;
                        points[1] = hit.point;
                        points[2] = endPos;
                        colorLineRenderer[colorNumber].SetPositions(points);
                    }
                    else
                    {
                        Vector3[] points = new Vector3[2] { startPos, endPos };
                        colorLineRenderer[colorNumber].SetPositions(points);
                    }
                }
                else
                {
                    Vector3[] points = new Vector3[2] { startPos, endPos };
                    colorLineRenderer[colorNumber].SetPositions(points);
                }
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originGimmicks[0] = new Gimmick01();
        if (gimickNumber == 1)
        {
            originGimmicks[1] = new Gimmick02(cables, cableObj, firstSetPosition, pillers);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LightHit()
    {
        originGimmicks[gimickNumber].Operation();
    }
}
