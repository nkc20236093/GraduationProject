using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    Janken[] jankens = new Janken[3];
    int select = 0;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject Finger;
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
                rigid.AddForce(moveDir * 5f);
            }
        }
    }

    public class Scissors : Janken
    {
        RaycastHit hit;
        float lazerDistance = 10f;
        bool LightHit = false;
        public override void HandEffect(AudioSource audio, Rigidbody rigid, Transform transform, GameObject Finger, LineRenderer lineRenderer)
        {
            Debug.Log("チョキ");
            // じゃんけん発動キーがjだと仮定して
            if (Input.GetKey(KeyCode.J)) 
            {
                lineRenderer.enabled = true;
                Vector3 direction = Finger.transform.InverseTransformDirection(Camera.main.transform.forward); // Fingerのローカル座標系に変換
                Ray ray = new Ray(Finger.transform.position, direction);
                //Debug.DrawRay(ray.origin, ray.direction * lazerDistance, Color.red);

                lineRenderer.SetPosition(1, Finger.transform.InverseTransformPoint(Finger.transform.position + direction * lazerDistance)); // Fingerのローカル座標系で設定                
                if (Physics.Raycast(ray, out hit, lazerDistance))
                {
                    Debug.Log("ヒット");
                    float distance = Vector3.Distance(Finger.transform.position, hit.point);
                    lazerDistance = distance;
                    if (hit.collider.gameObject.CompareTag("LightGimmick") && Mathf.Approximately(distance, lazerDistance))
                    {
                        LightHit = true;
                        if (LightHit)
                        {
                            // ヒットしたら作動
                            hit.collider.gameObject.GetComponent<LightGimmick>().LightHit();
                        }
                        Debug.Log("ヒット");
                    }
                    else
                    {
                        lazerDistance = 10.0f;
                        LightHit = false;
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
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
            Camera.main.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            Vector3 direction = Camera.main.transform.localEulerAngles;
            direction.x -= Input.GetAxis("Mouse Y") * 2.0f;
            direction.y += Input.GetAxis("Mouse X") * 2.0f;

            // 修正前のY軸の動きを保存
            float CameraY = direction.y;

            if (direction.x > 180)
            {
                direction.x -= 360;
            }

            direction.x = Mathf.Clamp(direction.x, -15, 45);
            direction.z = 0;
            Camera.main.transform.localEulerAngles = direction;

            // プレイヤーの向きをカメラの向きに合わせる
            transform.eulerAngles = new Vector3(0, CameraY, 0);
        }
    }
}
