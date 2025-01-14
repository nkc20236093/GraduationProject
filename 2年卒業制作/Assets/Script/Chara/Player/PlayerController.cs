using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    Janken[] jankens = new Janken[3];
    int select = 0;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachinePOV mPov;
    LineRenderer lineRenderer;

    public class Janken
    {
        public virtual void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer) { }
    }

    public class Rock : Janken
    {
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
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
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("パー");

            lineRenderer.enabled = false;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (moveDirection != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                rigid.AddForce(moveDir * 10f);
            }
        }
    }

    public class Scissors : Janken
    {
        RaycastHit hit;
        float lazerDistance = 10f;
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("チョキ");
            // じゃんけん発動キーがjだと仮定して
            if (Input.GetKey(KeyCode.J)) 
            {
                lineRenderer.enabled = true;
                Vector3 startPos = lineRenderer.GetPosition(0);
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
                            hit.collider.gameObject.GetComponent<GimmickCon>().LightHit();
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
        jankens[0] = new Rock();
        jankens[1] = new Scissors();
        jankens[2] = new Paper();
        lineRenderer = Finger.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraCon();
        Action();
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
        jankens[select].HandEffect(audioSource, rigid, transform, Finger, lineRenderer);
    }

    void CameraCon()
    {
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
}
