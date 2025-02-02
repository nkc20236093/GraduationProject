using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] float outline = 1f;
    Transform Player;
    [SerializeField] Vector3[] point;
    [SerializeField] float WalkSpeed = 5.0f;
    [SerializeField] float RunSpeed = 7.5f;
    [Header("�G�̔ԍ�\n0 = �_�u���w�b�h \n1 = ���" +
        "\n2 = �A�C�A���{�b�N�X\n3 = �m�[�}���G")]
    public int EnemyNumber;
    Enemy[] enemies = new Enemy[4];
    NavMeshAgent agent;
    [SerializeField] Animator animator;
    GameObject Flask;
    public class Enemy
    {
        protected float chaseTimer = 0;
        protected PlayerController player;
        protected bool Stop = false;
        protected bool goalpoint = false;
        protected float stopTimer = 0;
        protected bool searchHit = false;
        protected int NowPoint = 0;
        protected NavMeshAgent agent;
        protected Vector3[] Patrolpoint = new Vector3[4];
        public bool GetBool() { return searchHit; }
        public virtual void SetPoint(Vector3[] transforms) { }
        public virtual void Search(Vector3 targetpos, Transform mytrans) { }
        public virtual void Chase(Vector3 target) { }
        public virtual void Attack() { }
        public virtual void Animation(Animator animator) { }
    }

    public class DoubleHead : Enemy
    {
        bool movingUp = true;
        Transform chilltrans;
        public DoubleHead(NavMeshAgent navi, Transform trans, PlayerController player)
        {
            chilltrans = trans;
            agent = navi;
            this.player = player;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(Vector3 targetpos, Transform mytrans)
        {
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - mytrans.position;
            if (Physics.Raycast(mytrans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("����");
                            searchHit = true;
                        }
                        else
                        {
                            NextPoint();
                        }
                    }
                    else
                    {
                        NextPoint();
                    }
                }
                else
                {
                    NextPoint();
                }
            }

            void NextPoint()
            {
                if (Patrolpoint.Length == 0 && searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("�T��");
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f && !searchHit) 
                    {
                        agent.isStopped = true;
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.isStopped = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target)
        {
            while (chaseTimer < 5.0f)
            {
                Debug.Log("�ǐ�");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    break;
                }
            }
        }
        public override void Animation(Animator animator)
        {
            float targetY = movingUp ? 2.0f : 1.25f;
            Vector3 targetPosition = chilltrans.parent.TransformPoint(new Vector3(0, targetY, 0));
            chilltrans.position = Vector3.MoveTowards(chilltrans.position, targetPosition, 0.5f * Time.deltaTime);
            if ((chilltrans.position - targetPosition).sqrMagnitude < 0.001f)
            {
                movingUp = !movingUp;
            }
        }
    }

    public class PlagueDoctor : Enemy
    {
        GameObject flask;
        Transform transform;
        public PlagueDoctor(NavMeshAgent navi, GameObject Flask, Transform trans, PlayerController controller)
        {
            agent = navi;
            flask = Flask;
            transform = trans;
            player = controller;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(Vector3 targetpos, Transform mytrans)
        {
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - mytrans.position;
            if (Physics.Raycast(mytrans.position, directionToPlayer / 2, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund")) 
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("����");
                            searchHit = true;
                        }
                        else
                        {
                            NextPoint();
                        }
                    }
                    else
                    {
                        NextPoint();
                    }
                }
                else
                {
                    NextPoint();
                }
            }

            void NextPoint()
            {
                if (Patrolpoint.Length == 0 && searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("�T��");
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f)
                    {
                        agent.isStopped = true;
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.isStopped = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target)
        {
            while (chaseTimer < 5.0f)
            {
                Debug.Log("�ǐ�");
                agent.SetDestination(target);
                if (Mathf.Approximately(chaseTimer % 1.0f, 0f))
                {
                    Debug.Log("����");
                    Vector3 pos = transform.position;
                    pos.y += 0.5f;
                    GameObject instance = Instantiate(flask, pos, Quaternion.identity);
                    instance.SetActive(true);
                    Vector3 throwVec = (target - transform.position).normalized;
                    instance.GetComponent<Rigidbody>().AddForce(throwVec * 15, ForceMode.Impulse);
                }
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    break;
                }
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint)
            {
                animator.SetBool("Move", false);
                animator.SetBool("Run", false);
            }
            else
            {
                animator.SetBool("Move", true);
                if (searchHit)
                {
                    animator.SetBool("Run", true);
                }
                else
                {
                    animator.SetBool("Run", false);
                }
            }
        }
    }

    public class IronBox : Enemy
    {
        private GameObject gameObject;
        public IronBox(NavMeshAgent navi, GameObject game, PlayerController controller)
        {
            agent = navi;
            gameObject = game;
            player = controller;
        }
        private Vector3 playerPos;
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(Vector3 targetpos, Transform mytrans)
        {
            playerPos = targetpos;
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - mytrans.position;
            if (Physics.Raycast(mytrans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("����");
                            searchHit = true;
                        }
                        else
                        {
                            NextPoint();
                        }
                    }
                    else
                    {
                        NextPoint();
                    }
                }
                else
                {
                    NextPoint();
                }
            }

            void NextPoint()
            {
                if (Patrolpoint.Length == 0 && searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("�T��");
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f)
                    {
                        agent.isStopped = true;
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.isStopped = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target)
        {
            if (chaseTimer == 0)
            {
                CallEnemy();
            }
            while (chaseTimer < 5.0f)
            {
                Debug.Log("�ǐ�");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f) 
                {
                    chaseTimer = 0;
                    break;
                }
            }
            void CallEnemy()
            {
                List<GameObject> ene = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy")); ;
                foreach (GameObject obj in ene)   
                {
                    float distance = Vector3.Distance(new Vector3(obj.transform.position.x, 0, obj.transform.position.z), new Vector3(playerPos.x, 0, playerPos.z)) ;
                    if (distance <= 10 && obj != gameObject) 
                    {
                        obj.GetComponent<EnemyCon>().enemies[obj.GetComponent<EnemyCon>().EnemyNumber].Chase(playerPos);
                    }
                }
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint)
            {
                animator.SetBool("Move", false);
                animator.SetBool("Run", false);
            }
            else
            {
                animator.SetBool("Move", true);
                if (searchHit)
                {
                    animator.SetBool("Run", true);
                }
                else
                {
                    animator.SetBool("Run", false);
                }
            }
        }
    }

    public class NormalEnemy : Enemy
    {
        Transform trans;
        public NormalEnemy(NavMeshAgent navi, PlayerController controller)
        {
            agent = navi;
            player = controller;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(Vector3 targetpos, Transform mytrans)
        {
            if (searchHit || Stop) return;
            trans = mytrans;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - mytrans.position;
            if (Physics.Raycast(mytrans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("����");
                            searchHit = true;
                        }
                        else
                        {
                            NextPoint();
                        }
                    }
                    else
                    {
                        NextPoint();
                    }
                }
                else
                {
                    NextPoint();
                }
            }

            void NextPoint()
            {
                if (Patrolpoint.Length == 0 && searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    //Debug.Log("�T��");
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f)
                    {
                        agent.isStopped = true;
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.isStopped = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            if (Stop) return;
            Debug.Log("PlayerHit");
            Stop = true;
            player.Death();
        }
        public override void Chase(Vector3 target)
        {
            while (chaseTimer < 5.0f)
            {
                if (Stop) break;
                Debug.Log("�ǐ�");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    break;
                }
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint || Stop) 
            {
                animator.SetBool("Move", false);
                animator.SetBool("Run", false);
            }
            else
            {
                animator.SetBool("Move", true);
                if (searchHit)
                {
                    animator.SetBool("Run", true);
                }
                else
                {
                    animator.SetBool("Run", false);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = WalkSpeed;
        agent.acceleration = RunSpeed;
        if (EnemyNumber == 1)
        {
            Flask = transform.GetChild(0).GetChild(0).gameObject;
        }
        enemies[0] = new DoubleHead(agent, transform.GetChild(0), player);
        enemies[1] = new PlagueDoctor(agent, Flask, transform, player);
        enemies[2] = new IronBox(agent, gameObject, player);
        enemies[3] = new NormalEnemy(agent, player);
        enemies[EnemyNumber].SetPoint(point);
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies[EnemyNumber].GetBool())
        {
            enemies[EnemyNumber].Chase(Player.position);
        }
        else
        {
            enemies[EnemyNumber].Search(Player.position, transform);
        }
        enemies[EnemyNumber].Animation(animator);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && EnemyNumber != 0) 
        {
            enemies[EnemyNumber].Attack();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && EnemyNumber == 0) 
        {
            enemies[0].Attack();
        }
    }
    public void Induction(Vector3 pos, float distance)
    {
        if (distance < outline) return;
        enemies[EnemyNumber].Chase(pos);
    }
}
