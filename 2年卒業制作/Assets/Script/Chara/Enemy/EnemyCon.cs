using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCon : MonoBehaviour
{
    [Header("陽動、発見までの索敵範囲")] 
    [SerializeField] float outline = 1f;
    [SerializeField] GameObject mouth;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource myAudio;
    [SerializeField] Vector3[] point;
    [SerializeField] float WalkSpeed = 5.0f;
    [SerializeField] float RunSpeed = 7.5f;
    [Header("敵の番号\n0 = ダブルヘッド \n1 = 医者" +
        "\n2 = アイアンボックス\n3 = ノーマル敵")]
    [SerializeField] int EnemyNumber;
    Enemy[] enemies = new Enemy[4];
    NavMeshAgent agent;
    Transform Player;
    GameObject Flask;
    GameDirector director;
    public class Enemy
    {
        public Enemy(GameObject mouth, GameDirector gameDirector)
        {
            myMouth = mouth;
        }
        protected float walkSpeed;
        protected PlayerController player;
        protected bool actionStop = false;
        protected bool playerHit = false;
        protected bool goalpoint = false;
        protected bool searchHit = false;
        protected float stopTimer = 0;
        protected float chaseTimer = 0;
        protected float SEtimer = 0;
        protected int NowPoint = 0;
        protected GameObject myMouth;
        protected NavMeshAgent agent;
        protected Vector3[] Patrolpoint = new Vector3[4];
        protected Transform trans;
        protected RaycastHit hit;
        public bool GetBool() { return searchHit; }
        public virtual void SetPoint(Vector3[] transforms) 
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Patrolpoint[i] = transforms[i];
            }
        }
        public virtual void Search(Vector3 targetpos, AudioSource audioSource) { }
        public virtual IEnumerator Chase(Vector3 target) { yield break; }
        public virtual void Attack()
        {
            if (playerHit) return;
            playerHit = true;
            player.StartCoroutine(player.EnemyDeath(myMouth.transform, 3));
        }

        public virtual void Animation(Animator animator) { }
    }

    public class DoubleHead : Enemy
    {
        bool movingUp = true;
        Transform chilltrans;
        public DoubleHead(NavMeshAgent navi, Transform chillTrans, PlayerController player, Transform mytrans, float speed, GameObject Mouth, GameDirector gameDirector) : base(Mouth, gameDirector)
        {
            chilltrans = chillTrans;
            agent = navi;
            this.player = player;
            trans = mytrans;
            walkSpeed = speed;
            myMouth = Mouth;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            base.SetPoint(transforms);
        }
        public override void Search(Vector3 targetpos, AudioSource audioSource)
        {
            if (searchHit || actionStop) return;
            SEtimer += Time.deltaTime;
            if (SEtimer > 2.0f)
            {
                SEtimer = 0;
                audioSource.Play();
            }
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 60)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
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
                if (Patrolpoint.Length == 0 || searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f && !searchHit) 
                    {
                        agent.speed = 0;
                        agent.ResetPath();
                        goalpoint = true;
                    }
                    else
                    {
                        agent.speed = walkSpeed;
                        goalpoint = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            if (playerHit) return;
            playerHit = true;
            player.StartCoroutine(player.EnemyDeath(myMouth.transform, 3.2f));
        }
        public override IEnumerator Chase(Vector3 target)
        {
            agent.speed = 0;
            agent.ResetPath();
            if (playerHit || actionStop) yield break;
            searchHit = true;
            actionStop = true;
            while (chaseTimer < 5.0f)
            {
                agent.speed = walkSpeed;
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    chaseTimer = 0;
                    searchHit = false;
                    actionStop = false;
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    break;
                }
                yield return null;
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
        float throwTime = 0;
        GameObject flask;
        public PlagueDoctor(NavMeshAgent navi, GameObject Flask, Transform myTrans, PlayerController controller, float speed, GameObject Mouth, GameDirector gameDirector) : base(Mouth, gameDirector)
        {
            agent = navi;
            flask = Flask;
            trans = myTrans;
            player = controller;
            walkSpeed = speed;
            myMouth = Mouth;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            base.SetPoint(transforms);
        }
        public override void Search(Vector3 targetpos, AudioSource audioSource)
        {
            if (searchHit) return;
            SEtimer += Time.deltaTime;
            if (SEtimer > 2.0f)
            {
                SEtimer = 0;
                audioSource.Play();
            }
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 60)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
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
                if (Patrolpoint.Length == 0 || searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f && !searchHit)
                    {
                        agent.speed = 0;
                        agent.ResetPath();
                        goalpoint = true;
                    }
                    else
                    {
                        agent.speed = walkSpeed;
                        goalpoint = false;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            base.Attack();
        }
        public override IEnumerator Chase(Vector3 target)
        {
            agent.speed = 0;
            agent.ResetPath();
            if (searchHit || actionStop) yield break;
            actionStop = true;
            searchHit = true;
            while (chaseTimer < 10.0f)
            {
                throwTime += Time.deltaTime;
                agent.speed = walkSpeed;
                agent.SetDestination(target);
                if (throwTime > 2) 
                {
                    throwTime = 0;
                    Vector3 pos = trans.position;
                    pos.y += 0.5f;
                    GameObject instance = Instantiate(flask, pos, Quaternion.identity);
                    instance.SetActive(true);
                    Vector3 throwVec = (target - trans.position).normalized;
                    instance.GetComponent<Rigidbody>().AddForce(throwVec * 15, ForceMode.Impulse);
                }
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 10.0f) 
                {
                    actionStop = false;
                    searchHit = false;
                    throwTime = 0;
                    chaseTimer = 0;
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    break;
                }
                yield return null;
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint || playerHit) 
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
        public IronBox(NavMeshAgent navi, GameObject game, PlayerController controller, Transform myTrans, float speed, GameObject Mouth, GameDirector gameDirector) : base(Mouth, gameDirector)
        {
            agent = navi;
            gameObject = game;
            player = controller;
            trans = myTrans;
            walkSpeed = speed;
            myMouth = Mouth;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            base.SetPoint(transforms);
        }
        public override void Search(Vector3 targetpos, AudioSource audioSource)
        {
            if (searchHit) return;
            SEtimer += Time.deltaTime;
            if (SEtimer > 2.0f)
            {
                SEtimer = 0;
                audioSource.Play();
            }
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 120)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
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
                if (Patrolpoint.Length == 0 || searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f)
                    {
                        agent.speed = 0;
                        agent.ResetPath();
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.speed = walkSpeed;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            base.Attack();
        }
        public override IEnumerator Chase(Vector3 target)
        {
            agent.speed = 0;
            agent.ResetPath();
            if (searchHit || actionStop) yield break;
            actionStop = true;
            searchHit = true;
            while (chaseTimer < 6.0f)
            {
                if (Mathf.Approximately(chaseTimer % 3.0f, 0))
                {
                    agent.speed = 0;
                    agent.ResetPath();
                    CallEnemy();
                }

                agent.speed = walkSpeed;
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 6.0f) 
                {
                    chaseTimer = 0;
                    searchHit = false;
                    actionStop = false;
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    break;
                }
                yield return null;
            }
            void CallEnemy()
            {
                List<GameObject> ene = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy")); ;
                if (ene.Count == 0) return;
                foreach (GameObject obj in ene)   
                {
                    float distance = Vector3.Distance(new Vector3(obj.transform.position.x, 0, obj.transform.position.z), new Vector3(target.x, 0, target.z));
                    if (distance <= 20 && obj != gameObject)  
                    {
                        obj.GetComponent<EnemyCon>().enemies[obj.GetComponent<EnemyCon>().EnemyNumber].Chase(target);
                    }
                }
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint || playerHit) 
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
        public NormalEnemy(NavMeshAgent navi, PlayerController controller, Transform myTrans, float speed, GameObject Mouth, GameDirector gameDirector) : base(Mouth, gameDirector)
        {
            agent = navi;
            player = controller;
            trans = myTrans;
            walkSpeed = speed;
            myMouth = Mouth;
        }
        public override void SetPoint(Vector3[] transforms)
        {
            base.SetPoint(transforms);
        }
        public override void Search(Vector3 targetpos, AudioSource audioSource)
        {
            if (searchHit || playerHit) return;
            SEtimer += Time.deltaTime;
            if (SEtimer > 4f)
            {
                SEtimer = 0;
                audioSource.Play();
            }
            Vector3 directionToPlayer = targetpos - trans.position;
            if (Physics.Raycast(trans.position, directionToPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Graund"))
                {
                    float angleToPlayer = Vector3.Angle(trans.forward, directionToPlayer);
                    if (angleToPlayer <= 60)
                    {
                        float distanceToPlayer = directionToPlayer.magnitude;
                        if (distanceToPlayer <= 10f)
                        {
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
                if (Patrolpoint.Length == 0 || searchHit) return;
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    searchHit = false;
                    stopTimer += Time.deltaTime;
                    if (stopTimer < 2.5f)
                    {
                        agent.speed = 0;
                        agent.ResetPath();
                        goalpoint = true;
                    }
                    else
                    {
                        goalpoint = false;
                        agent.speed = walkSpeed;
                        agent.SetDestination(Patrolpoint[NowPoint]);
                        NowPoint = (NowPoint + 1) % Patrolpoint.Length;
                        stopTimer = 0;
                    }
                }
            }
        }

        public override void Attack()
        {
            base.Attack();
        }
        public override IEnumerator Chase(Vector3 target)
        {
            agent.ResetPath();
            agent.speed = 0;
            if (searchHit || actionStop) yield break;
            chaseTimer = 0;
            actionStop = true;
            searchHit = true;
            while (chaseTimer < 5.0f)
            {
                agent.speed = walkSpeed;
                agent.SetDestination(target);
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= 5.0f)
                {
                    actionStop = false;
                    searchHit = false;
                    chaseTimer = 0;
                    agent.SetDestination(Patrolpoint[NowPoint]);
                    break;
                }
                yield return null;
            }
        }
        public override void Animation(Animator animator)
        {
            if (goalpoint || playerHit) 
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
        director = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = WalkSpeed;
        agent.acceleration = RunSpeed;
        if (EnemyNumber == 1)
        {
            Flask = transform.GetChild(0).GetChild(0).gameObject;
        }
        enemies[0] = new DoubleHead(agent, transform.GetChild(0), player, transform, agent.speed, mouth, director);
        enemies[1] = new PlagueDoctor(agent, Flask, transform, player, agent.speed, mouth, director);
        enemies[2] = new IronBox(agent, gameObject, player, transform, agent.speed, mouth, director);
        enemies[3] = new NormalEnemy(agent, player, transform, agent.speed, mouth, director);
        enemies[EnemyNumber].SetPoint(point);
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        myAudio.volume = GameManager.instance.audiovolumes[0];
        if (enemies[EnemyNumber].GetBool())
        {
            StartCoroutine(enemies[EnemyNumber].Chase(Player.position));
        }
        else
        {
            enemies[EnemyNumber].Search(Player.position,myAudio);
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
        if (distance < outline)
        {
            StartCoroutine(enemies[EnemyNumber].Chase(Player.position));
        }
    }
    public bool GetBool()
    {
        return enemies[EnemyNumber].GetBool();
    }
}
