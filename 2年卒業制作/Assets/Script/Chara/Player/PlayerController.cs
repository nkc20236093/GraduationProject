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
    [SerializeField] MeshRenderer cylinder;
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;

    LineRenderer lineRenderer;
    Rigidbody rigid;

    int hitCount = 0;
    float hitCoolTime = 0;
    float timer = 0;
    bool performance = false;
    public static bool stop = false;

    public class Janken
    {
        public virtual void HandEffect() { }
    }

    public class Rock : Janken
    {
        float coolTime = 0;
        AudioSource audio;
        Transform transform;
        public Rock(AudioSource audio, Transform transform)
        {
            this.audio = audio;
            this.transform = transform;
        }
        public override void HandEffect()
        {
            Debug.Log("グー");
            // じゃんけん発動キーがjだと仮定して
            if (Input.GetKeyDown(KeyCode.J) && coolTime < 0)  
            {
                coolTime = 3;
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
            coolTime -= Time.deltaTime;
        }
    }

    public class Paper : Janken
    {
        Rigidbody rigid;
        float moveSpeed;
        public Paper(Rigidbody rigidbody, float speed)
        {
            rigid = rigidbody;
            moveSpeed = speed;
        }
        public override void HandEffect()
        {
            if (stop) return;
            Debug.Log("パー");


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
        LineRenderer lineRenderer;
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
                            return;
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
        mPov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        rigid = GetComponent<Rigidbody>();
        lineRenderer = Finger.GetComponent<LineRenderer>();
        jankens[0] = new Rock(audioSource,transform);
        jankens[1] = new Scissors(lineRenderer, Finger);
        jankens[2] = new Paper(rigid, MoveSpeed);
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
        if (select != 1)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
        if (select != 0)
        {
            cylinder.enabled = false;
        }
        else
        {
            cylinder.enabled = true;
        }
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

    public IEnumerator EnemyDeath(Transform enemyMouth, float blackOutTime)
    {
        transform.position = enemyMouth.root.position + enemyMouth.root.forward * 1.5f;
        rigid.velocity = Vector3.zero;
        while (!performance)
        {
            // ここに死亡演出
            stop = true;
            rigid.useGravity = false;
            virtualCamera.enabled = false;
            timer += Time.deltaTime;
            if (timer < blackOutTime || Vector3.Distance(transform.position, enemyMouth.position) > 0.5f)
            {
                transform.rotation = Quaternion.LookRotation(enemyMouth.position - transform.position);
                Camera.main.transform.rotation = Quaternion.LookRotation(enemyMouth.position - transform.position);
                transform.position = Vector3.MoveTowards(transform.position, enemyMouth.position, 0.5f * Time.fixedUnscaledDeltaTime);
                Camera.main.transform.position = Vector3.MoveTowards(transform.position, enemyMouth.position, 0.5f * Time.fixedUnscaledDeltaTime);
            }
            else
            {
                performance = true;
                // ここで画面がブラックアウト
                Debug.Log("ブラックアウト");
            }
            yield return null;
        }
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
