using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickCon : MonoBehaviour
{
    LineRenderer linerenderer;
    originGimmick[] originGimmicks = new originGimmick[4];
    [SerializeField] int gimickNumber;
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
        RaycastHit hit;
        LineRenderer lineRenderer;
        public Gimmick02(LineRenderer lineRenderer)
        {
            this.lineRenderer = lineRenderer;
        }
        public override void Operation()
        {
            Debug.Log("あやとり");
            Vector3 startPos = lineRenderer.GetPosition(0);
            Vector3 endPos = lineRenderer.GetPosition(1);
            Vector3 direction = endPos - startPos;
            Ray ray = new Ray(startPos, direction);
            if (Physics.Raycast(ray, out hit))  
            {
                if (hit.collider.gameObject.CompareTag("Pillar"))
                {
                    Vector3[] points = new Vector3[lineRenderer.positionCount + 1];
                    points[0] = startPos;
                    points[1] = hit.point;
                    points[2] = endPos;
                    lineRenderer.SetPositions(points);
                }
                else
                {
                    Vector3[] points = new Vector3[2] { startPos, endPos };
                    lineRenderer.SetPositions(points);
                }
            }
            else
            {
                Vector3[] points = new Vector3[2] { startPos, endPos };
                lineRenderer.SetPositions(points);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originGimmicks[0] = new Gimmick01();
        if (gimickNumber == 1)
        {
            for (int i = 1; i < 3; i++)
            {
                originGimmicks[i] = new Gimmick02(transform.GetChild(i - 1).GetComponent<LineRenderer>());
            }
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
