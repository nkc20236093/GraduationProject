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
    /// 0 = グー
    /// 1 = チョキ
    /// 2 = パー
    /// </summary>
    int select = 2;
    [SerializeField] MeshRenderer cylinder;
    [Header("自分の姿のオンオフ用")] [SerializeField] SkinnedMeshRenderer myMesh;
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] AudioSource audioSource;
    [Header("中指、人差し指の順")]
    [SerializeField] GameObject[] Finger;
    [SerializeField] LineRenderer[] lineRenderer;

    [SerializeField] GameDirector gameDirector;
    [SerializeField] UIDirector uIDirector;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;

    Rigidbody rigid;

    float damageHealingTime = 0;
    bool performance = false;
    public int hitCount = 0;
    public static bool stop = false;

    public class Janken
    {
        protected float saveTime;
        protected Animator animator;
        public Janken(Animator animator)
        {
            this.animator = animator;
        }
        public virtual void HandEffect() { }
        public virtual void Animation(int num) 
        {
            switch(num)
            {
                case 0:
                    animator.SetBool("Rock", true);
                    animator.SetBool("Scissors", false);
                    animator.SetBool("Paper", false);
                    break;
                case 1:
                    animator.SetBool("Rock", false);
                    animator.SetBool("Scissors", true);
                    animator.SetBool("Paper", false);
                    break;
                case 2:
                    if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    {
                        animator.speed = saveTime;
                        animator.SetBool("Paper", true);
                        animator.SetBool("Rock", false);
                        animator.SetBool("Scissors", false);
                    }
                    else
                    {
                        animator.SetBool("Paper", true);
                        animator.SetBool("Rock", false);
                        animator.SetBool("Scissors", false);
                        saveTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        animator.speed = 0;
                    }
                    break;
            }
        }
    }

    public class Rock : Janken
    {
        float coolTime = 0;
        AudioSource audio;
        Transform transform;
        public Rock(AudioSource audio, Transform transform, Animator anim) : base(anim)
        {
            this.audio = audio;
            this.transform = transform;
        }
        public override void HandEffect()
        {
            Debug.Log("グー");
            // じゃんけん発動キーが左クリックだと仮定して
            if (Input.GetMouseButtonDown(0) && coolTime < 0)
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
        public override void Animation(int num)
        {
            base.Animation(num);
        }
    }

    public class Paper : Janken
    {
        Rigidbody rigid;
        float moveSpeed;
        public Paper(Rigidbody rigidbody, float speed, Animator animator) : base(animator)
        {
            rigid = rigidbody;
            moveSpeed = speed;
            this.animator = animator;
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
        GameObject[] finger;
        RaycastHit hit;
        float lazerDistance = 10f;
        LineRenderer[] lineRenderer;
        SkinnedMeshRenderer myRender;
        float timer = 0;
        public Scissors(LineRenderer[] line, GameObject[] FInger, SkinnedMeshRenderer mesh, Animator anim) : base(anim)
        {
            lineRenderer = line;
            finger = FInger;
            myRender = mesh;
            animator = anim;
        }
        public override void HandEffect()
        {
            Debug.Log("チョキ");
            if (stop) 
            {
                gimmickCon.LightHit();
                myRender.enabled = false;
                return;
            }
            // じゃんけん発動キーが左クリック
            if (Input.GetMouseButton(0))
            {
                lineRenderer[0].enabled = true;
                lineRenderer[1].enabled = true;
                timer += Time.deltaTime;
                if (timer > 0.5f) 
                {
                    myRender.enabled = false;
                }
                Vector3 startPos = finger[0].transform.position;
                Vector3 endPos = Camera.main.transform.forward * lazerDistance;
                Vector3 direction = endPos - startPos;

                Ray ray = new Ray(startPos, direction * lazerDistance);
                lineRenderer[0].SetPosition(0, startPos);
                lineRenderer[1].SetPosition(0, startPos);
                Debug.DrawRay(ray.origin, ray.direction * lazerDistance, Color.red);

                if (Physics.Raycast(ray, out hit, lazerDistance))
                {
                    if (!hit.collider.gameObject.CompareTag("Player"))
                    {
                        lineRenderer[0].SetPosition(1, hit.point);
                        lineRenderer[1].SetPosition(1, hit.point);
                        lazerDistance = Vector3.Distance(startPos, hit.point);
                        if (hit.collider.gameObject.CompareTag("LightGimmick"))
                        {
                            Debug.Log("対象にヒット");
                            // ヒットしたら作動
                            gimmickCon = hit.collider.gameObject.GetComponent<GimmickCon>();
                            stop = true;
                            return;
                        }
                    }
                }
                else
                {
                    lazerDistance = 10.0f;
                    lineRenderer[0].SetPosition(1, endPos);
                    lineRenderer[1].SetPosition(1, endPos);
                }
            }
            else
            {
                lazerDistance = 10.0f;
                if (!stop)
                {
                    myRender.enabled = true;
                }
                timer = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mPov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        rigid = GetComponent<Rigidbody>();
        for (int i = 0; i < lineRenderer.Length; i++)
        {
            lineRenderer[i] = Finger[i].GetComponent<LineRenderer>();
        }
        jankens[0] = new Rock(audioSource, transform, animator);
        jankens[1] = new Scissors(lineRenderer, Finger, myMesh, animator);
        jankens[2] = new Paper(rigid, MoveSpeed, animator);
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
        hitCount = Mathf.Clamp(hitCount, 0, 10);
        damageHealingTime += Time.deltaTime;
        Debug.Log(hitCount);
        if (damageHealingTime >= 5)
        {
            damageHealingTime = 0;
            hitCount--;
        }
    }
    void Action()
    {
        if (!stop)
        {
            // 仮にチョキを0、グーを1、パーを2としたとき
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0) // マウスホイールを上にスクロールした場合
            {
                select = (select + 1) % 3; // 0, 1, 2 の間をループ
            }
            else if (scrollInput < 0) // マウスホイールを下にスクロールした場合
            {
                select = (select - 1 + 3) % 3; // 0, 1, 2 の間をループ
            }
        }
        jankens[select].HandEffect();
        uIDirector.ChangeJankenUI(select);
        if (select != 1)
        {
            for (int i = 0; i < lineRenderer.Length; i++)
            {
                lineRenderer[i].enabled = false;
            }
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
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

    public void PositionDead()
    {
        stop = true;
        rigid.useGravity = false;
        virtualCamera.enabled = false;
        gameDirector.GameOver();
    }

    public IEnumerator EnemyDeath(Transform enemyMouth, float blackOutTime)
    {
        transform.position = enemyMouth.root.position + enemyMouth.root.forward * 1.75f;
        rigid.velocity = Vector3.zero;
        float timer = 0;
        while (!performance)
        {
            // ここに死亡演出
            stop = true;
            rigid.useGravity = false;
            virtualCamera.enabled = false;
            timer += Time.deltaTime;
            // 一定距離まで近づくか一定時間経過するとブラックアウト
            if (timer < blackOutTime || Mathf.Approximately(Vector3.Distance(transform.position, enemyMouth.position), 1.0f))
            {
                transform.rotation = Quaternion.LookRotation(enemyMouth.position - transform.position);
                Camera.main.transform.rotation = Quaternion.LookRotation(enemyMouth.position - transform.position);
                transform.position = Vector3.MoveTowards(transform.position, enemyMouth.position, 0.5f * Time.fixedUnscaledDeltaTime);
                Camera.main.transform.position = Vector3.MoveTowards(transform.position, enemyMouth.position, 0.5f * Time.fixedUnscaledDeltaTime);
            }
            else if (timer > blackOutTime || Mathf.Approximately(Vector3.Distance(transform.position, enemyMouth.position), 1.0f))  
            {
                performance = true;
                // ここで画面がブラックアウト
                Debug.Log("ブラックアウト");
                // 死亡処理
                gameDirector.GameOver();
            }
            yield return null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Exit"))
        {
            // ここでクリアの呼び出し
            gameDirector.GameClear();
        }
    }
    /// <summary>
    /// じゃんけんの選択識別
    /// 0 = グー
    /// 1 = チョキ
    /// 2 = パー
    /// </summary>
    /// <returns></returns>
    public int GetSelect()
    {
        return select;
    }
}
