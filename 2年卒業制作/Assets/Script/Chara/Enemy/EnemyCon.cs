using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] Vector3[] point;
    [Header("0=ƒ_ƒuƒ‹ƒwƒbƒh")] [SerializeField] int EnemyNumber;
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
        public virtual void Chase(Vector3 target, NavMeshAgent agent) { }
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
            Vector3 directionToPlayer = targetpos - mytrans.position;
            float angleToPlayer = Vector3.Angle(mytrans.forward, directionToPlayer);
            if (angleToPlayer <= 90f / 2f)
            {           
                float distanceToPlayer = directionToPlayer.magnitude;
                if (distanceToPlayer <= 10f)
                {
                    Debug.Log("”­Œ©");
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
            void NextPoint()
            {
                if (Patrolpoint.Length == 0 && searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    Debug.Log("’Tõ");
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
        public override void Chase(Vector3 target, NavMeshAgent agent)
        {
            Debug.Log("’ÇÕ");
            agent.SetDestination(target);
        }
    }

    public class PlagueDoctor : Enemy
    {
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
        {

        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target, NavMeshAgent agent)
        {
            Debug.Log("’ÇÕ");
        }
    }

    public class IronBox : Enemy
    {
        public override void Search(NavMeshAgent agent, Vector3 targetpos, Transform mytrans)
        {

        }

        public override void Attack()
        {
            Debug.Log("PlayerHit");
        }
        public override void Chase(Vector3 target, NavMeshAgent agent)
        {
            Debug.Log("’ÇÕ");
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
        public override void Chase(Vector3 target, NavMeshAgent agent)
        {
            Debug.Log("’ÇÕ");
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
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies[EnemyNumber].GetBool())
        {
            enemies[EnemyNumber].Chase(Player.position, agent);
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
}
