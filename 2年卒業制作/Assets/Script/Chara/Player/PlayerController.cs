using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.localEulerAngles = Vector3.zero;
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        Vector3 movedirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rigid.AddForce(0.5f * movedirection, ForceMode.Impulse);
        CameraCon();
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

            if (direction.x > 180)
            {
                direction.x -= 360;
            }

            direction.x = Mathf.Clamp(direction.x, -15, 45);
            direction.z = 0;
            Camera.main.transform.localEulerAngles = direction;
        }
    }
}
