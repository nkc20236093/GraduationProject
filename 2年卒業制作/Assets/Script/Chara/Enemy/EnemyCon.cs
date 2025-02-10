using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] float outline = 1f;
    Transform Player;
    [SerializeField] Vector3[] point;
    [SerializeField] float WalkSpeed = 5.0f;
    [SerializeField] float RunSpeed = 7.5f;
    [Header("ìGÇÃî‘çÜ\n0 = É_ÉuÉãÉwÉbÉh \n1 = à„é“" +
        "\n2 = ÉAÉCÉAÉìÉ{ÉbÉNÉX\n3 = ÉmÅ[É}ÉãìG")]
    public int EnemyNumber;
    Enemy[] enemies = new Enemy[4];
    NavMeshAgent agent;
    [SerializeField] Animator animator;
    GameObject Flask;
    public class Enemy
    {
        protected PlayerController player;
        protected bool Stop = false;
        protected bool goalpoint = false;
        protected bool searchHit = false;
        protected float stopTimer = 0;
        protected float chaseTimer = 0;
        protected int NowPoint = 0;
        protected NavMeshAgent agent;
        protected NavMeshHit navHit;
        protected Vector3[] Patrolpoint = new Vector3[4];
        protected Transform trans;
        public bool GetBool() { return searchHit; }
        public virtual void SetPoint(Vector3[] transforms) { }
        public virtual void Search(Vector3 targetpos) { }
        public virtual void Chase(Vector3 target) { }
        public virtual void Attack() { }
        public virtual void Animation(Animator animator) { }
    }

    public class DoubleHead : Enemy
    {
        bool movingUp = true;
        Transform chilltrans;
        public DoubleHead(NavMeshAgent navi, Transform chillTrans, PlayerController player, Transform mytrans)
        {
            chilltrans = chillTrans;
            agent = navi;
            this.player = player;
            trans = mytrans;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        void FindPoint()
        {
            if (NavMesh.SamplePosition(trans.position, out navHit, 100f, NavMesh.AllAreas))
            {
                Debug.Log("åüçı");
                Vector3 closestPoint = navHit.position;
                agent.SetDestination(closestPoint);
            }
        }
        public override void Search(Vector3 targetpos)
        {
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("î≠å©");
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
                    Debug.Log("íTçı");
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
                Debug.Log("í«ê’");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    FindPoint();
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
        public PlagueDoctor(NavMeshAgent navi, GameObject Flask, Transform myTrans, PlayerController controller)
        {
            agent = navi;
            flask = Flask;
            trans = myTrans;
            player = controller;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        void FindPoint()
        {
            if (NavMesh.SamplePosition(trans.position, out navHit, 100f, NavMesh.AllAreas))
            {
                Debug.Log("åüçı");
                Vector3 closestPoint = navHit.position;
                agent.SetDestination(closestPoint);
            }
        }
        public override void Search(Vector3 targetpos)
        {
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer / 2, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund")) 
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("î≠å©");
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
                    Debug.Log("íTçı");
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
            if (chaseTimer <= 10.0f)
            {
                Debug.Log("í«ê’");
                agent.SetDestination(target);
                if (Mathf.Approximately(chaseTimer % 2, 0))
                {
                    Debug.Log("ìäù±");
                    Vector3 pos = trans.position;
                    pos.y += 0.5f;
                    GameObject instance = Instantiate(flask, pos, Quaternion.identity);
                    instance.SetActive(true);
                    Vector3 throwVec = (target - trans.position).normalized;
                    instance.GetComponent<Rigidbody>().AddForce(throwVec * 15, ForceMode.Impulse);
                }
                chaseTimer += Time.deltaTime;
                Debug.Log(chaseTimer);
            }
            else
            {
                Debug.Log("èIóπ");
                searchHit = false;
                FindPoint();
                chaseTimer = 0;
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
        public IronBox(NavMeshAgent navi, GameObject game, PlayerController controller, Transform myTrans)
        {
            agent = navi;
            gameObject = game;
            player = controller;
            trans = myTrans;
        }
        private Vector3 playerPos;
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        void FindPoint()
        {
            if (NavMesh.SamplePosition(trans.position, out navHit, 100f, NavMesh.AllAreas))
            {
                Debug.Log("åüçı");
                Vector3 closestPoint = navHit.position;
                agent.SetDestination(closestPoint);
            }
        }
        public override void Search(Vector3 targetpos)
        {
            playerPos = targetpos;
            if (searchHit) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("î≠å©");
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
                    Debug.Log("íTçı");
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
                Debug.Log("í«ê’");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f) 
                {
                    chaseTimer = 0;
                    FindPoint();
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
        public NormalEnemy(NavMeshAgent navi, PlayerController controller, Transform myTrans)
        {
            agent = navi;
            player = controller;
            trans = myTrans;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        void FindPoint()
        {
            if (NavMesh.SamplePosition(trans.position, out navHit, 100f, NavMesh.AllAreas))
            {
                Debug.Log("åüçı");
                Vector3 closestPoint = navHit.position;
                agent.SetDestination(closestPoint);
            }
        }
        public override void Search(Vector3 targetpos)
        {
            if (searchHit || Stop) return;
            RaycastHit hit;
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 90f / 2f)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
                            Debug.Log("î≠å©");
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
                    //Debug.Log("íTçı");
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
                Debug.Log("í«ê’");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    FindPoint();
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
        enemies[0] = new DoubleHead(agent, transform.GetChild(0), player, transform);
        enemies[1] = new PlagueDoctor(agent, Flask, transform, player);
        enemies[2] = new IronBox(agent, gameObject, player, transform);
        enemies[3] = new NormalEnemy(agent, player, transform);
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
            enemies[EnemyNumber].Search(Player.position);
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
