using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[3];

    [Header("見た目用モデル")]
    [SerializeField] MeshRenderer modelMesh;

    [Header("自分が何番目の攻略される番号か")] 
    [SerializeField] int myGimmickNumber = 1;

    [Header("配電盤用linerenderer\n赤、緑、紫の順番")]
    [SerializeField] GameObject[] pillers;
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;

    [Header("最初の位置、リセットするたびに設定\n下の位置とする")]
    [SerializeField] Vector3[] firstSetPosition;

    [Header("正解の位置\n真ん中、上")]
    [SerializeField] Vector3[] CorrectCenter;
    [SerializeField] Vector3[] CorrectUp;

    int colorNumber = 0;
    bool lightHit = false;
    bool OneAction = false;
    bool[] gimmickClears = new bool[3] { false, false, false };
    public class originGimmick
    {
        //以下は個別に設定
        protected int myNumber;
        protected Vector3 correctPosCenter;
        protected Vector3 correctPosUp;
        protected Vector3[] firstPosition;
        protected GameObject[] cables;
        protected LineRenderer colorLineRenderer;
        protected GameObject[] pillers;
        // 以下は最初から共通設定
        protected GimmickCon gimmickCon;
        protected Vector3[] points = new Vector3[2];
        protected string[] colorTag = new string[3] { "Red", "Green", "Cyan" };
        protected Vector3 localHitPoint = Vector3.zero;
        protected Vector3 localEndPos = Vector3.zero;
        protected int counts = default;
        protected bool first = false;
        protected bool rayHit = false;
        protected RaycastHit hit;
        protected Ray ray;
        public virtual void Operation() { }  
    }
    public class RedCable: originGimmick
    {
        public RedCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, Vector3 correctCenter, Vector3 correctUP)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            correctPosUp = correctUP;
            correctPosCenter = correctCenter;
            gimmickCon = gimmick;
        }
        public override void Operation()
        {
            Debug.Log("あやとり" + ":" + colorTag[myNumber]);
            // 仮にReturnキーを押したら終了
            if (Input.GetKeyDown(KeyCode.Return))
            {
                colorLineRenderer.enabled = false;
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // 起動時に一回だけ実行
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        localEndPos = firstPosition[myNumber];
                    }
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    first = true;
                }

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value++;
                }
                else if (Input.GetMouseButton(0))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -900, 900);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));
                //Debug.Log(value - localHitPoint.x + ":" + rayHit);

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Rayがヒットした座標をローカル座標に変換して追加
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) && hit.collider.gameObject.CompareTag(colorTag[myNumber]) || rayHit)
                {
                    if (!rayHit)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                    }
                    rayHit = true;
                    points = new Vector3[colorLineRenderer.positionCount + 1];
                    points[0] = startPos;
                    // ヒットした座標をローカル座標に変換
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(colorTag[myNumber]))
                    {
                        Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                        localHitPoint = new Vector3(pos.x, pos.y, 0);
                    }
                    points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    points[2] = new Vector3(value, 150, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15㎝以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15㎝以下だったらfalse
                    else if (localHitPoint.x < colorLineRenderer.GetPosition(0).x)
                    {
                        if ((value - localHitPoint.x) > 15)
                        {
                            Debug.Log("小さい");
                            rayHit = false;
                        }
                    }
                }
                else
                {
                    if (localHitPoint != Vector3.zero && rayHit)
                    {
                        points = new Vector3[colorLineRenderer.positionCount + 1];
                        points[0] = startPos;
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                        points[2] = new Vector3(value, 150, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 150, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("赤OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp) > 10) 
                {
                    //Debug.Log(Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) + ":" + Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosCenter));
                    Debug.Log("赤NG");
                    gimmickCon.gimmickClears[myNumber] = false;
                }
                return;
            }
        }
    }

    public class GreenCable : originGimmick
    {
        public GreenCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, Vector3 correctCenter, Vector3 correctUP)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            correctPosCenter = correctUP;
            correctPosCenter = correctCenter;
            gimmickCon = gimmick;
        }
        public override void Operation()
        {
            Debug.Log("あやとり" + ":" + colorTag[myNumber]);
            // 仮にReturnキーを押したら終了
            if (Input.GetKeyDown(KeyCode.Return))
            {
                colorLineRenderer.enabled = false;
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // 起動時に一回だけ実行
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        localEndPos = firstPosition[myNumber];
                    }
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    first = true;
                }

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value++;
                }
                else if (Input.GetMouseButton(0))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));
                //Debug.Log(value - localHitPoint.x + ":" + rayHit);

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Rayがヒットした座標をローカル座標に変換して追加
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) && hit.collider.gameObject.CompareTag(colorTag[myNumber]) || rayHit)
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer.positionCount + 1];
                    points[0] = startPos;
                    // ヒットした座標をローカル座標に変換
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(colorTag[myNumber]))
                    {
                        Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                        localHitPoint = new Vector3(pos.x, pos.y, 0);
                    }
                    points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    points[2] = new Vector3(value, 150, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15㎝以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15㎝以下だったらfalse
                    else if (localHitPoint.x < colorLineRenderer.GetPosition(0).x)
                    {
                        if ((value - localHitPoint.x) > 15)
                        {
                            Debug.Log("小さい");
                            rayHit = false;
                        }
                    }
                }
                else
                {
                    if (localHitPoint != Vector3.zero && rayHit)
                    {
                        points = new Vector3[colorLineRenderer.positionCount + 1];
                        points[0] = startPos;
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                        points[2] = new Vector3(value, 150, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 150, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("緑OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp) > 10)
                {
                    //Debug.Log(Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) + ":" + Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosCenter));
                    Debug.Log("緑NG");
                    gimmickCon.gimmickClears[myNumber] = false;
                }
                return;
            }
        }
    }
    public class CyanCable : originGimmick
    {
        public CyanCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, Vector3 correctCenter, Vector3 correctUP)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            correctPosCenter = correctUP;
            correctPosCenter = correctCenter;
            gimmickCon = gimmick;
        }
        public override void Operation()
        {
            Debug.Log("あやとり" + ":" + colorTag[myNumber]);
            // 仮にReturnキーを押したら終了
            if (Input.GetKeyDown(KeyCode.Return))
            {
                colorLineRenderer.enabled = false;
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // 起動時に一回だけ実行
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        localEndPos = firstPosition[myNumber];
                    }
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    first = true;
                }

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value++;
                }
                else if (Input.GetMouseButton(0))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));
                //Debug.Log(value - localHitPoint.x + ":" + rayHit);

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Rayがヒットした座標をローカル座標に変換して追加
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) && hit.collider.gameObject.CompareTag(colorTag[myNumber]) || rayHit)
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer.positionCount + 1];
                    points[0] = startPos;
                    // ヒットした座標をローカル座標に変換
                    if (hit.collider != null && hit.collider.gameObject.CompareTag(colorTag[myNumber]))
                    {
                        Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                        localHitPoint = new Vector3(pos.x, pos.y, 0);
                    }
                    points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    points[2] = new Vector3(value, 150, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15㎝以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15㎝以下だったらfalse
                    else if (localHitPoint.x < colorLineRenderer.GetPosition(0).x)
                    {
                        if ((value - localHitPoint.x) > 15)
                        {
                            Debug.Log("小さい");
                            rayHit = false;
                        }
                    }
                }
                else
                {
                    if (localHitPoint != Vector3.zero && rayHit)
                    {
                        points = new Vector3[colorLineRenderer.positionCount + 1];
                        points[0] = startPos;
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                        points[2] = new Vector3(value, 150, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 150, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("紫OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp) > 10)
                {
                    //Debug.Log(Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter) + ":" + Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosCenter));
                    Debug.Log("紫NG");
                    gimmickCon.gimmickClears[myNumber] = false;
                }
                return;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GimmickCon gimmickCon = gameObject.GetComponent<GimmickCon>();
        originGimmicks[0] = new RedCable(0, gimmickCon, cables[0], cableObj, firstSetPosition, pillers, CorrectCenter[0], CorrectUp[0]);
        originGimmicks[1] = new GreenCable(1, gimmickCon, cables[1], cableObj, firstSetPosition, pillers, CorrectCenter[1], CorrectUp[1]);
        originGimmicks[2] = new CyanCable(2, gimmickCon, cables[2], cableObj, firstSetPosition, pillers, CorrectCenter[2], CorrectUp[2]);
    }

    // Update is called once per frame
    void Update()
    {
        if (gimmickClears[0] && gimmickClears[1] && gimmickClears[2])
        {
            // ギミッククリアによる変化を実行
            Debug.Log("クリア");
            for (int i = 0; i < cables.Length; i++)
            {
                cables[i].enabled = false;
            }
            lightHit = false;
            GameDirector gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
            gameDirector.GimmickEvent(myGimmickNumber);
        }
        if (lightHit)
        {
            if (!OneAction)
            {
                modelMesh.enabled = true;
                for (int i = 0; i < cables.Length; i++)
                {
                    cables[i].enabled = true;
                }
                for (int i = 0; i < pillers.Length; i++)
                {
                    pillers[i].SetActive(true);
                }
                OneAction = true;
            }
            // マウスのホイールで変更
            if (Input.mouseScrollDelta.y > 0)
            {
                colorNumber++;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                colorNumber--;
            }
            colorNumber = Mathf.Clamp(colorNumber, 0, 2);
            originGimmicks[colorNumber].Operation();
        }
        else
        {
            if (OneAction)
            {
                modelMesh.enabled = false;
                for (int i = 0; i < cables.Length; i++)
                {
                    cables[i].enabled = false;
                }
                for (int i = 0; i < pillers.Length; i++)
                {
                    pillers[i].SetActive(false);
                }
                // プレイヤーも操作可能に戻す
                PlayerController.stop = false;
                OneAction = false;
            }
        }
    }
    public void LightHit()
    {
        lightHit = true;
    }
}
