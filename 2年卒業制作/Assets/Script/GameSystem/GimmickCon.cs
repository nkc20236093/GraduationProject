using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[3];

    [Header("見た目用モデル")]
    [SerializeField] MeshRenderer modelMesh;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image myImage;

    [Header("自分が何番目の攻略される番号か")] 
    [SerializeField] int myGimmickNumber = 1;

    [Header("配電盤用linerenderer\n赤、緑、紫の順番")]
    [SerializeField] GameObject[] pillers;
    [Header("各色の柱\n赤、緑、紫の順番")]
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
    [SerializeField] Material[] lineMaterialLuminescence = new Material[3];
    [SerializeField] Material[] lineMaterial = new Material[3];
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
        protected int randomInt = default;
        protected bool first = false;
        protected bool rayHit = false;
        protected RaycastHit hit;
        protected Ray ray;
        public virtual void Operation() { }  
    }
    public class RedCable: originGimmick
    {
        readonly Vector3[] correctPosCenter = new Vector3[6]
        {
            new Vector3(0, -150, 0),
            new Vector3(0, -150, 0),
            new Vector3(0, -150, 0),
            new Vector3(0, -150, 0),
            new Vector3(-60, 100, 0),
            new Vector3(60, 175, 0)
        };
        readonly Vector3[] correctPosUp = new Vector3[6]
        {
            new Vector3(300, 350, 0),
            new Vector3(-300, 350, 0),
            new Vector3(300, 350, 0),
            new Vector3(-300, 350, 0),
            new Vector3(-300, 350, 0),
            new Vector3(250, 350, 0)
        };
        private readonly Vector3[] PILLAR_POSITIONS_RED = new Vector3[6]
        {
            new Vector3(-75, 175, 0),
            new Vector3(190, 200, 0),
            new Vector3(-190, 200, 0),
            new Vector3(-190, 0, 0),
            new Vector3(-60, 100, 0),
            new Vector3(60, 175, 0)
        };
        public RedCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, int r)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            gimmickCon = gimmick;
            randomInt = r;
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
                    RectTransform rect = pillers[myNumber].GetComponent<RectTransform>();
                    rect.anchoredPosition3D = PILLAR_POSITIONS_RED[randomInt];
                    if (!colorLineRenderer.enabled)
                    {
                        colorLineRenderer.enabled = true;
                    }
                    colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                    localEndPos = firstPosition[myNumber];
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    first = true;
                }

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = localEndPos - localStartPos;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value += 5;
                }
                else if (Input.GetMouseButton(0))
                {
                    value -= 5;
                }
                value = Mathf.Clamp(value, -300, 300);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

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
                    points[2] = new Vector3(value, 350, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15�p以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15�p以下だったらfalse
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
                        points[2] = new Vector3(value, 350, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 350, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter[randomInt]);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp[randomInt]);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("赤OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter[randomInt]) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp[randomInt]) > 10) 
                {
                    Debug.Log("赤NG");
                    gimmickCon.gimmickClears[myNumber] = false;
                }
                return;
            }
        }
    }

    public class GreenCable : originGimmick
    {
        readonly Vector3[] correctPosCenter = new Vector3[6]
{
            new Vector3(300, -150, 0),
            new Vector3(300, -150, 0),
            new Vector3(300, -150, 0),
            new Vector3(300, -150, 0),
            new Vector3(200, 150, 0),
            new Vector3(300, -150, 0)
};
        readonly Vector3[] correctPosUp = new Vector3[6]
        {
            new Vector3(-300, 350, 0),
            new Vector3(300, 350, 0),
            new Vector3(0, 350, 0),
            new Vector3(0, 350, 0),
            new Vector3(-300, 350, 0),
            new Vector3(250, 350, 0)
        };
        private readonly Vector3[] PILLAR_POSITIONS_GREEN = new Vector3[6]
        {
            new Vector3(-125, 75, 0),
            new Vector3(-125,200,0),
            new Vector3(5,200,0),
            new Vector3(-145,180,0),
            new Vector3(200, 150, 0),
            new Vector3(-140, -90, 0)
        };

        public GreenCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, int r)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            gimmickCon = gimmick;
            randomInt = r;
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
                    RectTransform rect = pillers[myNumber].GetComponent<RectTransform>();
                    Debug.Log(PILLAR_POSITIONS_GREEN[randomInt] + ":" + randomInt);
                    rect.anchoredPosition3D = PILLAR_POSITIONS_GREEN[randomInt];
                    if (!colorLineRenderer.enabled)
                    {
                        colorLineRenderer.enabled = true;
                    }
                    colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                    localEndPos = firstPosition[myNumber];
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    Debug.Log(correctPosUp[randomInt] + ":" + correctPosCenter[randomInt]);
                    first = true;
                }

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value += 5;
                }
                else if (Input.GetMouseButton(0))
                {
                    value -= 5;
                }
                value = Mathf.Clamp(value, -300, 300);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

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
                    points[2] = new Vector3(value, 350, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15�p以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15�p以下だったらfalse
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
                        points[2] = new Vector3(value, 350, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 350, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter[randomInt]);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp[randomInt]);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("緑OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter[randomInt]) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp[randomInt]) > 10)
                {
                    Debug.Log("緑NG");
                    gimmickCon.gimmickClears[myNumber] = false;
                }
                return;
            }
        }
    }
    public class CyanCable : originGimmick
    {
        readonly Vector3[] correctPosCenter = new Vector3[6]
{
            new Vector3(-300, -150, 0),
            new Vector3(-300, -150, 0),
            new Vector3(-300, -150, 0),
            new Vector3(-300, -150, 0),
            new Vector3(-175, 200, 0),
            new Vector3(-180, 150, 0)
};
        readonly Vector3[] correctPosUp = new Vector3[6]
        {
            new Vector3(0, 350, 0),
            new Vector3(0, 350, 0),
            new Vector3(-300, 350, 0),
            new Vector3(300, 350, 0),
            new Vector3(30, 350, 0),
            new Vector3(250, 350, 0)
        };
        private readonly Vector3[] PILLAR_POSITIONS_CYAN = new Vector3[6]
        {
            new Vector3(90, 200, 0),
            new Vector3(90, 270, 0),
            new Vector3(-90, 270, 0),
            new Vector3(260, -40, 0),
            new Vector3(-175, 200, 0),
            new Vector3(-180, 150, 0)
        };

        public CyanCable(int num, GimmickCon gimmick, LineRenderer lineRenderer, GameObject[] cables, Vector3[] first, GameObject[] pillers, int r)
        {
            myNumber = num;
            colorLineRenderer = lineRenderer;
            this.cables = cables;
            firstPosition = first;
            this.pillers = pillers;
            gimmickCon = gimmick;
            randomInt = r;
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
                    RectTransform rect = pillers[myNumber].GetComponent<RectTransform>();
                    Debug.Log(PILLAR_POSITIONS_CYAN[randomInt] + ":" + randomInt);
                    rect.anchoredPosition3D = PILLAR_POSITIONS_CYAN[randomInt];
                    if (!colorLineRenderer.enabled)
                    {
                        colorLineRenderer.enabled = true;
                    }
                    colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                    localEndPos = firstPosition[myNumber];
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = colorLineRenderer.GetPosition(i);
                    }
                    Debug.Log(correctPosUp[randomInt] + ":" + correctPosCenter[randomInt] + ":" + randomInt);
                    first = true;
                }

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;
                ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // マウスの左右のクリックで動かす
                if (Input.GetMouseButton(1))
                {
                    value += 5;
                }
                else if (Input.GetMouseButton(0))
                {
                    value -= 5;
                }
                value = Mathf.Clamp(value, -300, 300);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

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
                    points[2] = new Vector3(value, 350, 0);
                    counts = 3;
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より大きい(右にある)時は
                    // localHitPoint.x(中点)-value(一番上)の差が15�p以下だったらfalse
                    if (localHitPoint.x > colorLineRenderer.GetPosition(0).x)
                    {
                        if ((localHitPoint.x - value) > 15)
                        {
                            Debug.Log("大きい");
                            rayHit = false;
                        }
                    }
                    // localHitPoint(中点)のx座標がcolorLineRenderer.GetPosition(0).x(一番下)より小さい(左にある)時は
                    // value(一番上)-localHitPoint.x(中点)の差が15�p以下だったらfalse
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
                        points[2] = new Vector3(value, 350, 0);
                        counts = 3;
                    }
                    else
                    {
                        points = new Vector3[2] { startPos, new Vector3(value, 350, 0) };
                        counts = 2;
                    }
                }
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                float centerDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2), correctPosCenter[randomInt]);
                float UpDistance = Vector3.Distance(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1), correctPosUp[randomInt]);
                if (centerDistance <= 10 && UpDistance <= 10)
                {
                    Debug.Log("紫OK");
                    gimmickCon.gimmickClears[myNumber] = true;
                }
                else if (Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 2) * 100), correctPosCenter[randomInt]) > 10 || Vector3.Distance(colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1) * 100), correctPosUp[randomInt]) > 10)
                {
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
        int r = Random.Range(0, sprites.Length - 1);
        myImage.sprite = sprites[r];
        GimmickCon gimmickCon = gameObject.GetComponent<GimmickCon>();
        originGimmicks[0] = new RedCable(0, gimmickCon, cables[0], cableObj, firstSetPosition, pillers, r);
        originGimmicks[1] = new GreenCable(1, gimmickCon, cables[1], cableObj, firstSetPosition, pillers, r);
        originGimmicks[2] = new CyanCable(2, gimmickCon, cables[2], cableObj, firstSetPosition, pillers, r);
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
            gameObject.SetActive(false);
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
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput > 0)
            {
                colorNumber = (colorNumber + 1) % 3;
            }
            else if (scrollInput < 0)
            {
                colorNumber = (colorNumber - 1 + 3) % 3;
            }
            cables[colorNumber].material = lineMaterialLuminescence[colorNumber];
            for (int i = 0; i < cables.Length; i++)
            {
                if (i != colorNumber) 
                {
                    cables[i].material = lineMaterial[i];
                }
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
