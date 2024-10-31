using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetFull : MonoBehaviour
{
    public float maxVelocityX = 1f;
    public float maxVelocityY = 1f;
    public float maxVelocityZ = 1f;
    public float handPower = 0.5f;
    public float force = 1f;
    public Transform hand_left; // 左手スラスターのトランスフォーム
    public Transform hand_right; // 右手スラスターのトランスフォーム
    public Transform foot_left; // 左手スラスターのトランスフォーム
    public Transform foot_right; // 右手スラスターのトランスフォーム
    private Vector3 direction;


    private Rigidbody rb; // 動かすオブジェクト(自分自身)のリジットボディ

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = hand_left.right * handPower - hand_right.right * handPower + foot_left.up + foot_right.up + Vector3.down * 2f;
        rb.AddForce(direction * force);
        Debug.Log(direction * force);
        Vector3 velocity = rb.velocity;

        if (Mathf.Abs(velocity.x) > maxVelocityX)
        {
            velocity.x = Mathf.Sign(velocity.x) * maxVelocityX;
        }

        if (Mathf.Abs(velocity.y) > maxVelocityY)
        {
            velocity.y = Mathf.Sign(velocity.y) * maxVelocityY;
        }

        if (Mathf.Abs(velocity.z) > maxVelocityZ)
        {
            velocity.z = Mathf.Sign(velocity.z) * maxVelocityZ;
        }

        rb.velocity = velocity;
        Debug.Log(rb.velocity.magnitude);
    }
}
