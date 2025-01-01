using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskCon : MonoBehaviour
{
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Graund"))
        {
            GameObject poison = transform.GetChild(0).gameObject;
            poison.transform.SetParent(null);
            Vector3 pos = poison.transform.position;
            pos.y += 1.0f;
            poison.transform.position = pos;
            poison.SetActive(true);
            Destroy(gameObject);
        }
    }
}
