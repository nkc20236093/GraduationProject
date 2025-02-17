using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCon : MonoBehaviour
{
    float hitTimer = 0;
    float timeLimit = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeLimit += Time.deltaTime;
        if (timeLimit >= 3)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hitTimer += Time.deltaTime;
            if (hitTimer > 1.5f)
            {
                Debug.Log("ヒット");
                hitTimer = 0;
                // ダメージOR遅延効果
                Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
                rigidbody.velocity *= 0.7f;
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                player.hitCount++;
                if (player.hitCount >= 3)
                {
                    player.PositionDead();
                }
                else if(player.hitCount < 3)
                {
                    UIDirector uIDirector =GameObject.Find("UIDirector").GetComponent<UIDirector>();
                    uIDirector.Damaged(player.hitCount);
                }
            }
        }
    }
}
