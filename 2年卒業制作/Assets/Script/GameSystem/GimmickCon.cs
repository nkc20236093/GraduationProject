using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[3];
    [SerializeField] int colorNumber = 0;

    [Header("�z�d�՗plinerenderer\n�ԁA�΁A���̏���")]
    [SerializeField] GameObject[] pillers;
    [SerializeField] GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
    [Header("�ŏ��̈ʒu�A���Z�b�g���邽�тɐݒ�\n���̈ʒu�Ƃ���")]
    [SerializeField] Vector3[] firstSetPosition;
    [Header("�����̈ʒu\n�^�񒆁A��")]
    [SerializeField] Vector3[] CorrectCenter;
    [SerializeField] Vector3[] CorrectUp;
    bool lightHit = false;
    bool[] gimmickClears = new bool[3] { false, false, false };
    public class originGimmick
    {
        //�ȉ��͌ʂɐݒ�
        protected int myNumber;
        protected Vector3 correctPosCenter;
        protected Vector3 correctPosUp;
        protected Vector3[] firstPosition;
        protected GameObject[] cables;
        protected LineRenderer colorLineRenderer;
        protected GameObject[] pillers;
        // �ȉ��͍ŏ����狤�ʐݒ�
        protected GimmickCon gimmickCon;
        protected Vector3[] points;
        protected int[] bothWays = new int[2] { 1, -1 };
        protected Vector3 localHitPoint = Vector3.zero;
        protected int counts = default;
        protected bool first = false;
        protected bool rayHit = false;
        protected RaycastHit hit;
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
            correctPosCenter = correctUP;
            correctPosCenter = correctCenter;
            gimmickCon = gimmick;
        }
        public override void Operation()
        {
            Debug.Log("����Ƃ�");
            // ����Escape�L�[����������I��
            if (Input.GetKeyDown(KeyCode.Escape) || gimmickCon.gimmickClears[myNumber]) 
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    pillers[i].SetActive(false);
                }
                colorLineRenderer.enabled = false;
                // �v���C���[������\�ɖ߂�
                PlayerController.stop = false;
                GimmickCon gimmickCon = (GimmickCon)GameObject.FindObjectOfType(typeof(GimmickCon));
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // �N�����Ɉ�񂾂����s
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        pillers[i].SetActive(true);
                    }
                    first = true;
                }


                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // ����M��B�Ő��𓮂����Ƃ�����
                if (Input.GetKey(KeyCode.M))
                {
                    value++;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                Vector3 localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;

                Ray ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Ray���q�b�g�������W�����[�J�����W�ɕϊ����Ēǉ�
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) || rayHit && hit.collider != null)
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer  .positionCount + 1];
                    points[0] = startPos;
                    // �q�b�g�������W�����[�J�����W�ɕϊ�
                    Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                    localHitPoint = new Vector3(pos.x, pos.y, 0);

                    if (localHitPoint.x > 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    }
                    else if (localHitPoint.x < 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
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
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                if (colorLineRenderer.GetPosition(1).Equals(correctPosCenter) && colorLineRenderer.GetPosition(2).Equals(correctPosCenter))
                {
                    gimmickCon.gimmickClears[myNumber] = true;
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
            Debug.Log("����Ƃ�");
            // ����Escape�L�[����������I��
            if (Input.GetKeyDown(KeyCode.Escape) || gimmickCon.gimmickClears[myNumber]) 
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    pillers[i].SetActive(false);
                }
                colorLineRenderer.enabled = false;
                // �v���C���[������\�ɖ߂�
                PlayerController.stop = false;
                GimmickCon gimmickCon = (GimmickCon)GameObject.FindObjectOfType(typeof(GimmickCon));
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // �N�����Ɉ�񂾂����s
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        pillers[i].SetActive(true);
                    }
                    first = true;
                }

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // ����M��B�Ő��𓮂����Ƃ�����
                if (Input.GetKey(KeyCode.M))
                {
                    value++;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                Vector3 localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;

                Ray ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Ray���q�b�g�������W�����[�J�����W�ɕϊ����Ēǉ�
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) || rayHit && hit.collider != null)
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer.positionCount + 1];
                    points[0] = startPos;
                    // �q�b�g�������W�����[�J�����W�ɕϊ�
                    Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                    localHitPoint = new Vector3(pos.x, pos.y, 0);

                    if (localHitPoint.x > 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    }
                    else if (localHitPoint.x < 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
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
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                if (colorLineRenderer.GetPosition(1).Equals(correctPosCenter) && colorLineRenderer.GetPosition(2).Equals(correctPosCenter))
                {
                    gimmickCon.gimmickClears[myNumber] = true;
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
            Debug.Log("����Ƃ�");
            // ����Escape�L�[����������I��
            if (Input.GetKeyDown(KeyCode.Escape) || gimmickCon.gimmickClears[myNumber]) 
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    pillers[i].SetActive(false);
                }
                colorLineRenderer.enabled = false;
                // �v���C���[������\�ɖ߂�
                PlayerController.stop = false;
                GimmickCon gimmickCon = (GimmickCon)GameObject.FindObjectOfType(typeof(GimmickCon));
                gimmickCon.lightHit = false;
                first = false;
            }
            else
            {
                // �N�����Ɉ�񂾂����s
                if (!first)
                {
                    for (int i = 0; i < cables.Length; i++)
                    {
                        if (!colorLineRenderer.enabled)
                        {
                            colorLineRenderer.enabled = true;
                        }
                        colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, firstPosition[myNumber]);
                        pillers[i].SetActive(true);
                    }
                    first = true;
                }

                float value = colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1).x;
                // ����M��B�Ő��𓮂����Ƃ�����
                if (Input.GetKey(KeyCode.M))
                {
                    value++;
                }
                else if (Input.GetKey(KeyCode.B))
                {
                    value--;
                }
                value = Mathf.Clamp(value, -100, 100);
                colorLineRenderer.SetPosition(colorLineRenderer.positionCount - 1, new Vector3(value, 150, 0));

                Vector3 startPos = colorLineRenderer.GetPosition(0);
                Vector3 localStartPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(0));
                Vector3 localEndPos = colorLineRenderer.transform.TransformPoint(colorLineRenderer.GetPosition(colorLineRenderer.positionCount - 1));
                Vector3 direction = (localEndPos - localStartPos).normalized;

                Ray ray = new Ray(localStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.blue);
                // Ray���q�b�g�������W�����[�J�����W�ɕϊ����Ēǉ�
                if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Pillar")) || rayHit && hit.collider != null)
                {
                    rayHit = true;
                    points = new Vector3[colorLineRenderer.positionCount + 1];
                    points[0] = startPos;
                    // �q�b�g�������W�����[�J�����W�ɕϊ�
                    Vector3 pos = hit.collider.GetComponent<RectTransform>().anchoredPosition;
                    localHitPoint = new Vector3(pos.x, pos.y, 0);

                    if (localHitPoint.x > 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
                    }
                    else if (localHitPoint.x < 0)
                    {
                        points[1] = new Vector3(localHitPoint.x, localHitPoint.y, 0);
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
                colorLineRenderer.positionCount = counts;
                colorLineRenderer.SetPositions(points);
                if (colorLineRenderer.GetPosition(1).Equals(correctPosCenter) && colorLineRenderer.GetPosition(2).Equals(correctPosCenter))
                {
                    gimmickCon.gimmickClears[myNumber] = true;
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
            // �M�~�b�N�N���A�ɂ��ω������s
            Debug.Log("�N���A");
            GameDirector gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
            gameDirector.GimmickEvent();
        }
        if (lightHit)
        {
            for (int i = 0; i < cables.Length; i++)
            {
                cables[i].enabled = true;
            }
            // ����F��H�ŘM�����ς���Ƃ�����
            if (Input.GetKeyDown(KeyCode.H))
            {
                colorNumber++;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                colorNumber--;
            }
            colorNumber = Mathf.Clamp(colorNumber, 0, 2);
            originGimmicks[colorNumber].Operation();
        }
        else
        {
            for (int i = 0; i < cables.Length; i++)
            {
                cables[i].enabled = false;
            }
        }
    }
    public void LightHit()
    {
        lightHit = true;
    }
}
