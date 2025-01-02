using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] float outline = 1f;
    Transform Player;
    [SerializeField] Vector3[] point;
    [Header("0 = É_ÉuÉãÉwÉbÉh \n1 = à„é“\n2 = ÉAÉCÉAÉìÉ{ÉbÉNÉX\n3 = ÉmÅ[É}ÉãìG")] public int EnemyNumber;
    Enemy[] enemies = new Enemy[4];
    NavMeshAgent agent;
    GameObject Flask;
    public class Enemy
    {
        protected float chaseTimer = 0;
        protected bool searchHit = false;
        protected int NowPoint = 0;
        protected NavMeshAgent agent;
        protected Vector3[] Patrolpoint = new Vector3[4];
        public bool GetBool() { return searchHit; }
        public virtual void SetPoint(Vector3[] transforms) { }
        public virtual void Search(Vector3 targetpos, Transform mytrans) { }
        public virtual void Chase(Vector3 target) { }
        public virtual void Attack() { }
        protected virtual void Animation() { }
    }

    public class DoubleHead : Enemy
    {
        public DoubleHead(NavMeshAgent navi)
        {
            agent = navi;
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
                if (hit.collider.CompareTag("Player"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
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
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    NowPoint = (NowPoint + 1) % Patrolpoint.Length;
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
                    break;
                }
            }
        }
    }

    public class PlagueDoctor : Enemy
    {
        GameObject flask;
        Transform transform;
        public PlagueDoctor(NavMeshAgent navi, GameObject Flask, Transform trans)
        {
            agent = navi;
            flask = Flask;
            transform = trans;
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
                if (hit.collider.CompareTag("Player"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
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
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    NowPoint = (NowPoint + 1) % Patrolpoint.Length;
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
                if (Mathf.Approximately(chaseTimer % 1.0f, 0f))
                {
                    Debug.Log("ìäù±");
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
    }

    public class IronBox : Enemy
    {
        private GameObject gameObject;
        public IronBox(NavMeshAgent navi, GameObject game)
        {
            agent = navi;
            gameObject = game;
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
                if (hit.collider.CompareTag("Player"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
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
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    NowPoint = (NowPoint + 1) % Patrolpoint.Length;
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
    }

    public class NormalEnemy : Enemy
    {
        public NormalEnemy(NavMeshAgent navi)
        {
            agent = navi;
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
                if (hit.collider.CompareTag("Player"))
                {
                    float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
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
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    NowPoint = (NowPoint + 1) % Patrolpoint.Length;
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
                    break;
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (EnemyNumber == 1)
        {
            Flask = transform.GetChild(0).GetChild(0).gameObject;
        }
        enemies[0] = new DoubleHead(agent);
        enemies[1] = new PlagueDoctor(agent, Flask, transform);
        enemies[2] = new IronBox(agent, gameObject);
        enemies[3] = new NormalEnemy(agent);
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
