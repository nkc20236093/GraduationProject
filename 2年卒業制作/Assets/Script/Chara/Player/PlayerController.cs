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
    [SerializeField] GameObject Finger;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] GameDirector gameDirector;
    [SerializeField] UIDirector uIDirector;
    [SerializeField] Animator animator;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;

    Rigidbody rigid;

    float damageHealingTime = 0;
    bool performance = false;
    bool dead = false;
    public int hitCount = 0;
    public static bool stop = false;

    public class Janken
    {
        const float fadeTime = 1 / 3;
        bool isFade = false;
        protected bool isClick = false;
        protected Animator animator;
        protected SkinnedMeshRenderer modelRender;
        protected float timer = 0;
        public Janken(Animator animator)
        {
            this.animator = animator;
        }
        protected void FadeSkinne(Material material)
        {
            Color firstColor = material.color;
            Color tarthetColor = isFade ? new Color(firstColor.r, firstColor.g, firstColor.b, 1.0f) : new Color(firstColor.r, firstColor.g, firstColor.b, 0.0f);
            float nowTime = 0;
            if (nowTime < fadeTime) 
            {
                nowTime += Time.deltaTime;
                float t = nowTime / fadeTime;
                material.color = Color.Lerp(firstColor, tarthetColor, t);
            }
            else
            {
                isFade = !isFade;
            }
        }
        public virtual void HandEffect() { }
        public virtual void Animation(int num, Material material) 
        {
            if (Input.GetMouseButton(0))
            {
                isClick = true;
            }
            else
            {
                isClick = false;
            }
            switch(num)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    if (Input.GetButton("Horizontal")|| Input.GetButton("Vertical")) 
                    {
                        if (Input.GetAxis("Vertical") > 0)
                        {
                            animator.SetFloat("speed", 1);
                        }
                        else if (Input.GetAxis("Vertical") < 0)
                        {
                            animator.SetFloat("speed", -1);
                        }
                        animator.SetBool("Paper", true);
                        animator.SetBool("Idle", false);
                    }
                    else
                    {
                        animator.SetBool("Idle", true);
                        animator.SetBool("Paper", false);
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
        public Rock(AudioSource audio, Transform transform, Animator anim, SkinnedMeshRenderer mesh) : base(anim)
        {
            this.audio = audio;
            this.transform = transform;
            modelRender = mesh;
        }
        public override void HandEffect()
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                FadeSkinne(modelRender.material);
            }
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
        public override void Animation(int num, Material material)
        {
            base.Animation(num, material);
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

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (moveDirection != Vector3.zero && !stop)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                rigid.velocity = moveDir * moveSpeed;
            }
            else
            {
                rigid.velocity = new Vector3(0, Physics.gravity.y, 0);
            }
        }
        public override void Animation(int num, Material material)
        {
            base.Animation(num, material);
        }
    }

    public class Scissors : Janken
    {
        GimmickCon gimmickCon = null;
        GameObject finger;
        RaycastHit hit;
        float lazerDistance = 10f;
        LineRenderer lineRenderer;
        public Scissors(LineRenderer line, GameObject FInger, SkinnedMeshRenderer mesh, Animator anim) : base(anim)
        {
            lineRenderer = line;
            finger = FInger;
            modelRender = mesh;
            animator = anim;
        }
        public override void HandEffect()
        {
            if (stop)
            {
                gimmickCon.LightHit();
                modelRender.enabled = false;
                lineRenderer.enabled = false;
                return;
            }
            // じゃんけん発動キーが左クリック
            if (Input.GetMouseButton(0))
            {
                lineRenderer.enabled = true;
                timer += Time.deltaTime;
                if (timer > 0.5f) 
                {
                    FadeSkinne(modelRender.material);
                }

                Vector3 startPos = finger.transform.TransformPoint(finger.transform.position);
                Vector3 endPos = Camera.main.transform.forward * lazerDistance;
                Vector3 direction = endPos - startPos;

                Ray ray = new Ray(startPos, direction * lazerDistance);
                lineRenderer.SetPosition(0, startPos);
                Debug.DrawRay(ray.origin, ray.direction * lazerDistance, Color.red);
                if (Physics.Raycast(ray, out hit, lazerDistance, LayerMask.GetMask("LightGimmick")))
                {
                    lineRenderer.SetPosition(1, hit.point);
                    lazerDistance = Vector3.Distance(startPos, hit.point);
                    // ヒットしたら作動
                    gimmickCon = hit.collider.gameObject.GetComponent<GimmickCon>();
                    stop = true;
                    return;
                }
                else
                {
                    lazerDistance = 10.0f;
                    lineRenderer.SetPosition(1, endPos);
                }
            }
            else
            {
                lineRenderer.enabled = false;
                lazerDistance = 10.0f;
                timer = 0;
            }
        }
        public override void Animation(int num, Material material)
        {
            base.Animation(num, material);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mPov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        rigid = GetComponent<Rigidbody>();
        lineRenderer = Finger.GetComponent<LineRenderer>();
        jankens[0] = new Rock(audioSource, transform, animator, myMesh);
        jankens[1] = new Scissors(lineRenderer, Finger, myMesh, animator);
        jankens[2] = new Paper(rigid, MoveSpeed, animator);
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = GameManager.instance.audiovolumes[0];
        CameraCon();
        Action();
        DamaglHealing();
    }
    void DamaglHealing()
    {
        hitCount = Mathf.Clamp(hitCount, 0, 10);
        damageHealingTime += Time.deltaTime;
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
            jankens[select].Animation(select, myMesh.material);
        }
        jankens[select].HandEffect();
        uIDirector.ChangeJankenUI(select);
        if (select != 1) 
        {
            lineRenderer.enabled = false;
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
            if (!dead)
            {
                rigid.isKinematic = true;
            }
            mPov.m_HorizontalAxis.m_InputAxisName = "";
            mPov.m_HorizontalAxis.m_InputAxisValue = 0;
            mPov.m_VerticalAxis.m_InputAxisName = "";
            mPov.m_VerticalAxis.m_InputAxisValue = 0;
            return;
        }
        else
        {
            if (!dead)
            {
                rigid.isKinematic = true;
            }
            rigid.isKinematic = false;
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
        if (stop) yield break;
        transform.position = enemyMouth.root.position + enemyMouth.root.forward * 1.75f;
        rigid.velocity = Vector3.zero;
        float timer = 0;
        stop = true;
        dead = true;
        while (!performance)
        {
            // ここに死亡演出
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
