using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] float outline = 1f;
    Transform Player;
    [SerializeField] Vector3[] point;
    [Header("0 = ダブルヘッド \n1 = 医者\n2 = アイアンボックス\n3 = ノーマル敵")] [SerializeField] int EnemyNumber;
    Enemy[] enemies = new Enemy[4];
    NavMeshAgent agent;

    public class Enemy
    {
        protected bool searchHit = false;
        protected int NowPoint = 0;
        protected Vector3[] Patrolpoint = new Vector3[4];
        public bool GetBool() { return searchHit; }
        public virtual void SetPoint(Vector3[] transforms) { }
        public virtual void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans) { }
        public virtual void Chase(Vector3 target, NavMeshAgent agent, float chaseTimer) { }
        public virtual void Attack() { }
    }

    public class DoubleHead : Enemy
    {
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
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
                            Debug.Log("発見");
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
                    Debug.Log("探索");
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
        public override void Chase(Vector3 target, NavMeshAgent agent, float chaseTimer)
        {
            while (chaseTimer < 5.0f)
            {
                Debug.Log("追跡");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f) break;
            }
        }
    }

    public class PlagueDoctor : Enemy
    {
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
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
                            Debug.Log("発見");
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
                    Debug.Log("探索");
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
        public override void Chase(Vector3 target, NavMeshAgent agent, float chaseTimer)
        {
            while (chaseTimer < 5.0f)
            {
                Debug.Log("追跡");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f) break;
            }
        }
    }

    public class IronBox : Enemy
    {
        public override void SetPoint(Vector3[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
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
                            Debug.Log("発見");
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
                    Debug.Log("探索");
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
        public override void Chase(Vector3 target, NavMeshAgent agent, float chaseTimer)
        {
            while (chaseTimer < 5.0f)
            {
                Debug.Log("追跡");
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f) break;
            }
        }
    }

    public class NormalEnemy : Enemy
    {
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
        {

        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target, NavMeshAgent agent, float chaseTimer)
        {
            Debug.Log("追跡");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemies[0] = new DoubleHead();
        enemies[1] = new PlagueDoctor();
        enemies[2] = new IronBox();
        enemies[3] = new NormalEnemy();
        enemies[EnemyNumber].SetPoint(point);
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies[EnemyNumber].GetBool())
        {
            enemies[EnemyNumber].Chase(Player.position, agent, 0);
        }
        else
        {
            enemies[EnemyNumber].Search(agent, Player.position, transform);
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
        Debug.Log(0);
        enemies[EnemyNumber].Chase(pos, agent, 0);
    }
}
