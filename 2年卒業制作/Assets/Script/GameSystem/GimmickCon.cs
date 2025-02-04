using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[4];
    [SerializeField] int gimickNumber;

    [Header("配電盤用linerenderer\n赤、緑、紫の順番")]
    [SerializeField] GameObject[] pillers;
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
    [Header("最初の位置、リセットするたびに設定\n下の位置とする")]
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
        Vector3[] points;
        int[] bothWays = new int[2] { 1, -1 };

        Vector3 localHitPoint;
        int counts = default;
        bool first = false;
        bool rayHit = false;
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
                    pillers[i].SetActive(false);
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
                        if (!colorLineRenderer[i].enabled)
                        {
                            colorLineRenderer[i].enabled = true;
                        }
                        colorLineRenderer[i].SetPosition(colorLineRenderer[i].positionCount - 1, firstPosition[i]);
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

                float value = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1).x;
                // 仮にMとBで線を動かすとしたら
                if (Input.GetKey(KeyCode.M))
                {
                    value ++;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value --;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer[colorNumber].SetPosition(colorLineRenderer[colorNumber].positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer[colorNumber].GetPosition(0);
                Vector3 localStartPos = colorLineRenderer[colorNumber].transform.TransformPoint(colorLineRenderer[colorNumber].GetPosition(0));
                Vector3 localEndPos = colorLineRenderer[colorNumber].transform.TransformPoint(colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;

                Ray ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Rayがヒットした座標をローカル座標に変換して追加
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) || rayHit && hit.collider != null) 
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer[colorNumber].positionCount + 1];
                    points[0] = startPos;
                    // ヒットした座標をローカル座標に変換
                    localHitPoint = hit.transform.InverseTransformPoint(hit.collider.gameObject.transform.position);
                    Debug.Log(localHitPoint * 100000000 + ":" + hit.point);
                    if (localHitPoint.z > 0)
                    {
                        points[1] = new Vector3(bothWays[0] * localHitPoint.z, localHitPoint.y, 0) * 100000000;
                    }
                    else if (localHitPoint.z < 0)
                    {
                        points[1] = new Vector3(bothWays[1] * localHitPoint.z, localHitPoint.y, 0) * 100000000;
                    }
                    points[2] = new Vector3(value, 150, 0);
                    counts = 3;
                    Debug.Log(Mathf.Abs(value - localHitPoint.x));
                    if (Mathf.Abs(value - localHitPoint.x) < 15)
                    {
                        rayHit = false;
                    }
                }
                else
                {
                    points = new Vector3[2] { startPos, new Vector3(value, 150, 0) };
                    counts = 2;
                }
                colorLineRenderer[colorNumber].positionCount = counts;
                colorLineRenderer[colorNumber].SetPositions(points);
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
