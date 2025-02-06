using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCon : MonoBehaviour
{
    GameDirector gameDirector;
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("ヒット");
            // ダメージOR遅延効果
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.velocity *= 0.7f;
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player.DamageHitCount() >= 10)
            {
                gameDirector.Dead();
            }
        }
    }
}
