using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickCon : MonoBehaviour
{
    originGimmick[] originGimmicks = new originGimmick[1];
    [SerializeField] int gimickNumber;
    public class originGimmick
    {
        protected int myNumber = 0;
        public originGimmick(int setNumber)
        {
            myNumber = setNumber;
        }
        public virtual void Operation() { }
    }
    public class Gimmick01 : originGimmick
    {
        public Gimmick01(int setNumber) : base(setNumber)
        {
            myNumber = setNumber;
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
        public Gimmick02(int setNumber, RaycastHit hit, LineRenderer lineRenderer) : base(setNumber)
        {
            myNumber = setNumber;
            this.hit = hit;
            this.lineRenderer = lineRenderer;
        }
        public override void Operation()
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originGimmicks[0] = new Gimmick01(0);
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
