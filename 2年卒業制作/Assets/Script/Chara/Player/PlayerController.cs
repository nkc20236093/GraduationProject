using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    Janken[] jankens = new Janken[3];
    /// <summary>
    /// ����񂯂�̑I������
    /// 0 = �O�[
    /// 1 = �`���L
    /// 2 = �p�[
    /// </summary>
    int select = 2;
    [SerializeField] GameDirector gameDirector;
    [SerializeField] MeshRenderer cylinder;
    [Header("�����̎p�̃I���I�t�p")] [SerializeField] MeshRenderer myMesh;
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;

    LineRenderer lineRenderer;
    Rigidbody rigid;

    float damageHealingTime = 0;
    bool performance = false;
    public int hitCount = 0;
    public static bool stop = false;

    public class Janken
    {
        public virtual void HandEffect() { }
        public virtual void Animation() { }
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
            Debug.Log("�O�[");
            // ����񂯂񔭓��L�[�����N���b�N���Ɖ��肵��
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
            Debug.Log("�p�[");


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
        MeshRenderer myRender;
        float timer = 0;
        public Scissors(LineRenderer line, GameObject FInger, MeshRenderer mesh)
        {
            lineRenderer = line;
            finger = FInger;
            myRender = mesh;
        }
        public override void HandEffect()
        {
            Debug.Log("�`���L");
            if (stop) 
            {
                gimmickCon.LightHit();
                lineRenderer.enabled = false;
                myRender.enabled = false;
                return;
            }
            // ����񂯂񔭓��L�[��j���Ɖ��肵��
            if (Input.GetMouseButton(0))
            {
                lineRenderer.enabled = true;
                timer += Time.deltaTime;
                if (timer > 0.5f) 
                {
                    myRender.enabled = false;
                }
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
                            Debug.Log("�ΏۂɃq�b�g");
                            // �q�b�g������쓮
                            gimmickCon = hit.collider.gameObject.GetComponent<GimmickCon>();
                            stop = true;
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
        lineRenderer = Finger.GetComponent<LineRenderer>();
        jankens[0] = new Rock(audioSource,transform);
        jankens[1] = new Scissors(lineRenderer, Finger, myMesh);
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
            // ���Ƀ`���L��0�A�O�[��1�A�p�[��2�Ƃ����Ƃ�
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput > 0) // �}�E�X�z�C�[������ɃX�N���[�������ꍇ
            {
                select = (select + 1) % 3; // 0, 1, 2 �̊Ԃ����[�v
            }
            else if (scrollInput < 0) // �}�E�X�z�C�[�������ɃX�N���[�������ꍇ
            {
                select = (select - 1 + 3) % 3; // 0, 1, 2 �̊Ԃ����[�v
            }
        }
        jankens[select].HandEffect();

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
            // �����Ɏ��S���o
            stop = true;
            rigid.useGravity = false;
            virtualCamera.enabled = false;
            timer += Time.deltaTime;
            // ��苗���܂ŋ߂Â�����莞�Ԍo�߂���ƃu���b�N�A�E�g
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
                // �����ŉ�ʂ��u���b�N�A�E�g
                Debug.Log("�u���b�N�A�E�g");
                // ���S����
                gameDirector.GameOver();
            }
            yield return null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Exit"))
        {
            // �����ŃN���A�̌Ăяo��
            gameDirector.GameClear();
        }
    }
    /// <summary>
    /// ����񂯂�̑I������
    /// 0 = �O�[
    /// 1 = �`���L
    /// 2 = �p�[
    /// </summary>
    /// <returns></returns>
    public int GetSelect()
    {
        return select;
    }
}
