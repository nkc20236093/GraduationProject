using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    Janken[] jankens = new Janken[3];
    /// <summary>
    /// じゃんけんの選択識別
    /// 0 = チョキ
    /// 1 = グー
    /// 2 = パー
    /// </summary>
    int select = 2;
    [SerializeField] GameDirector gameDirector;
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;

    LineRenderer lineRenderer;
    Rigidbody rigid;

    int hitCount = 0;
    float hitCoolTime = 0;
    public static bool stop = false;

    public class Janken
    {
        protected LineRenderer lineRenderer;
        public virtual void HandEffect() { }
    }

    public class Rock : Janken
    {
        AudioSource audio;
        Transform transform;
        public Rock(LineRenderer line, AudioSource audio, Transform transform)
        {
            lineRenderer = line;
            this.audio = audio;
            this.transform = transform;
        }
        public override void HandEffect()
        {
            Debug.Log("グー");

            lineRenderer.enabled = false;

            // じゃんけん発動キーがjだと仮定して
            if (Input.GetKeyDown(KeyCode.J))
            {
                audio.Play();
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f);
                foreach (Collider col in hitColliders)
                {
                    if (col.CompareTag("Enemy"))
                    {
                        float distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(col.transform.position.x, 0, col.transform.position.z));
                        col.gameObject.GetComponent<EnemyCon>().Induction(transform.position, distance);
                    }
                }
            }
        }
    }

    public class Paper : Janken
    {
        Rigidbody rigid;
        float moveSpeed;
        public Paper(LineRenderer line, Rigidbody rigidbody, float speed)
        {
            lineRenderer = line;
            rigid = rigidbody;
            moveSpeed = speed;
        }
        public override void HandEffect()
        {
            if (stop) return;
            Debug.Log("パー");

            lineRenderer.enabled = false;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (moveDirection != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                rigid.velocity = moveDir * moveSpeed;
            }
        }
    }

    public class Scissors : Janken
    {
        GimmickCon gimmickCon = null;
        GameObject finger;
        RaycastHit hit;
        float lazerDistance = 10f;
        public Scissors(LineRenderer line, GameObject FInger)
        {
            lineRenderer = line;
            finger = FInger;
        }
        public override void HandEffect()
        {
            Debug.Log("チョキ");
            if (stop)
            {
                gimmickCon.LightHit();
                lineRenderer.enabled = false;
                return;
            }
            // じゃんけん発動キーがjだと仮定して
            if (Input.GetKey(KeyCode.J))   
            {
                lineRenderer.enabled = true;
                Vector3 startPos = finger.transform.position;
                Vector3 endPos = Camera.main.transform.forward * lazerDistance;
                Vector3 direction = endPos - startPos;

                Ray ray = new Ray(startPos, direction * lazerDistance);
                lineRenderer.SetPosition(0, startPos);
                Debug.DrawRay(ray.origin, ray.direction * lazerDistance, Color.red);

                if (Physics.Raycast(ray, out hit, lazerDistance))
                {
                    if (!hit.collider.gameObject.CompareTag("Player"))
                    {
                        lineRenderer.SetPosition(1, hit.point);
                        lazerDistance = Vector3.Distance(startPos, hit.point);
                        if (hit.collider.gameObject.CompareTag("LightGimmick"))
                        {
                            Debug.Log("対象にヒット");
                            // ヒットしたら作動
                            stop = true;
                            gimmickCon = hit.collider.gameObject.GetComponent<GimmickCon>();
                        }
                    }
                }
                else
                {
                    lazerDistance = 10.0f;
                    lineRenderer.SetPosition(1, endPos);
                }
            }
            else
            {
                lazerDistance = 10.0f;
                lineRenderer.enabled = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        mPov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        rigid = GetComponent<Rigidbody>();
        lineRenderer = Finger.GetComponent<LineRenderer>();
        jankens[0] = new Rock(lineRenderer,audioSource,transform);
        jankens[1] = new Scissors(lineRenderer, Finger);
        jankens[2] = new Paper(lineRenderer, rigid, MoveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        CameraCon();
        Action();
        DamaglHealing();
    }
    void DamaglHealing()
    {
        hitCoolTime += Time.deltaTime;
        if (hitCoolTime >= 3)
        {
            hitCoolTime = 0;
            hitCount--;
        }
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
        jankens[select].HandEffect();
    }

    void CameraCon()
    {
        if (stop)
        {
            mPov.m_HorizontalAxis.m_InputAxisName = "";
            mPov.m_HorizontalAxis.m_InputAxisValue = 0;
            mPov.m_VerticalAxis.m_InputAxisName = "";
            mPov.m_VerticalAxis.m_InputAxisValue = 0;
            return;
        }
        else
        {
            mPov.m_HorizontalAxis.m_InputAxisName = "Mouse X";
            mPov.m_VerticalAxis.m_InputAxisName = "Mouse Y";
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            transform.eulerAngles = Vector3.zero;
            mPov.m_VerticalAxis.Value = 0;
            mPov.m_HorizontalAxis.Value = 0;
        }
        else
        {
            Vector3 direction = new Vector3(0, transform.eulerAngles.y, 0);
            direction.y += Input.GetAxis("Mouse X") * 2.0f;

            if (direction.y > 180)
            {
                direction.y -= 360;
            }
            direction.z = 0;
            transform.eulerAngles = new Vector3(0, direction.y, 0);
            mPov.m_HorizontalAxis.Value = direction.y;
        }
    }

    public void Death()
    {
        if (stop) return;
        stop = true;
        // ここに死亡演出
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Exit"))
        {
            // ここでクリアの呼び出し
        }
    }
    public int DamageHitCount()
    {
        if (hitCoolTime >= 0) return hitCount;
        hitCount++;
        return hitCount;
    }
}
