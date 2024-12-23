using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    Janken[] jankens = new Janken[3];
    int select = 0;

    public enum HandType
    {
        None,
        Rock,
        Paper,
        Scissors
    }

    public class Janken
    {
        public HandType hand;
        public Janken(HandType selecthand)
        {
            hand = selecthand;
        }
        public virtual void HandEffect() { }
    }

    public class Rock : Janken
    {
        public Rock() : base(HandType.Rock)
        {

        }
        public override void HandEffect()
        {
            Debug.Log("グー");
        }
    }

    public class Paper : Janken
    {
        public Paper() : base(HandType.Paper)
        {

        }
        public override void HandEffect()
        {
            Debug.Log("パー");
        }
    }

    public class Scissors : Janken
    {
        public Scissors() : base(HandType.Scissors)
        {

        }
        public override void HandEffect()
        {
            Debug.Log("チョキ");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.localEulerAngles = Vector3.zero;
        rigid = GetComponent<Rigidbody>();

        jankens[0] = new Rock();
        jankens[1] = new Scissors();
        jankens[2] = new Paper();
    }

    // Update is called once per frame
    void Update()
    {
        Action();
        Move();
    }

    void Action()
    {
        // 仮にチョキをi、グーをu、パーをpとしたとき
        if (Input.GetKeyDown(KeyCode.U))
        {
            select = 0;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            select = 1;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            select = 2;
        }
        // じゃんけん発動キーがjだと仮定して
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            jankens[select].HandEffect();
        }
    }

    void Move()
    {
        CameraCon();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rigid.AddForce(moveDir * 5f);
        }

        // プレイヤーの向きをカメラの向きに合わせる
        Vector3 lookDirection = new Vector3(horizontalInput, 0, verticalInput);
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 3f * Time.deltaTime);
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }
    void CameraCon()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            Vector3 direction = Camera.main.transform.localEulerAngles;
            direction.x -= Input.GetAxis("Mouse Y") * 2.0f;
            direction.y += Input.GetAxis("Mouse X") * 2.0f;

            if (direction.x > 180)
            {
                direction.x -= 360;
            }

            direction.x = Mathf.Clamp(direction.x, -15, 45);
            direction.z = 0;
            Camera.main.transform.localEulerAngles = direction;
        }
    }
}
