using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickCon : MonoBehaviour
{
    bool gimmickChallenge = false;
    originGimmick[] originGimmicks = new originGimmick[4];
    [SerializeField] int gimickNumber;

    [Header("配電盤用linerenderer\n赤、緑、紫の順番")]
    [SerializeField]
    GameObject[] cableObj;
    [SerializeField] LineRenderer[] cables;
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
        GameObject[] cables;
        int colorNumber = 0;
        RaycastHit hit;
        LineRenderer[] colorLineRenderer= new LineRenderer[3];
        public Gimmick02(LineRenderer[] lineRenderer, GameObject[] cables)
        {
            colorLineRenderer = lineRenderer;
            this.cables = cables;
        }
        public override void Operation()
        {
            Debug.Log("あやとり");
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                for (int i = 0; i < cables.Length; i++) 
                {
                    colorLineRenderer[i].enabled = false;
                }
                // ここにギミック終了のコード

            }
            for (int i = 0; i < cables.Length; i++)
            {
                if (!colorLineRenderer[i].enabled)
                {
                    colorLineRenderer[i].enabled = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                colorNumber++;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                colorNumber--;
            }
            Vector3 startPos = colorLineRenderer[colorNumber].GetPosition(0);
            Vector3 endPos = colorLineRenderer[colorNumber].GetPosition(colorLineRenderer[colorNumber].positionCount);
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

    // Start is called before the first frame update
    void Start()
    {
        originGimmicks[0] = new Gimmick01();
        if (gimickNumber == 1)
        {
            originGimmicks[1] = new Gimmick02(cables, cableObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LightHit()
    {
        if (gimmickChallenge) return;
        gimmickChallenge = true;
        originGimmicks[gimickNumber].Operation();
    }
}
