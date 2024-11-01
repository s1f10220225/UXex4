using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetFull : MonoBehaviour
{
    public float maxVelocityX = 1f;
    public float maxVelocityY = 1f;
    public float maxVelocityZ = 1f;
    public float downForce = 0.8f; // 初期の倍率
    public float downForceIncrement = 0.1f; // 増加させる値
    public float timeToIncDownForce = 5f; // 増加させる間隔
    public bool isFalling = false; // 特定のフラグ
    public Transform hand_left; // 左手スラスターのトランスフォーム
    public Transform hand_right; // 右手スラスターのトランスフォーム
    public Transform foot_left; // 左足スラスターのトランスフォーム
    public Transform foot_right; // 右足スラスターのトランスフォーム
    private Vector3 direction;

    private Rigidbody rb; // 動かすオブジェクト(自分自身)のリジットボディ

    // 各部位に対応したアイテム取得による手足の力の強化値
    private float handLeftPower = 0.5f;
    private float handRightPower = 0.5f;
    private float footLeftPower = 1f;
    private float footRightPower = 1f;
    private float initialDownForce;
    private float elapsedTime = 0f; // 経過時間を記録

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialDownForce = (handLeftPower + handRightPower + footLeftPower + footRightPower) * downForce;
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間の計算
        elapsedTime += Time.deltaTime;

        // フラグが立っているか、一定時間経過した場合に落下速度を増加
        if (elapsedTime >= timeToIncDownForce || isFalling)
        {
            initialDownForce += downForceIncrement;
            elapsedTime = 0f; // 経過時間をリセット
        }

        // 各部位の力はアイテムボーナス分を加算
        direction = (hand_left.right * handLeftPower)
                  - (hand_right.right * handRightPower)
                  + (foot_left.up * footLeftPower)
                  + (foot_right.up * footRightPower);

        // 落下力の適用
        Vector3 totalForce = direction + Vector3.down * initialDownForce;

        rb.AddForce(totalForce);
        Vector3 velocity = rb.velocity;

        // 速度の制限
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
    }

    // アイテムを取得した際に各部位の推進力を増加
    public void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "l_hand":
                handLeftPower += 0.1f;
                break;
            case "r_and":
                handRightPower += 0.1f;
                break;
            case "l_foot":
                footLeftPower += 0.1f;
                break;
            case "r_foot":
                footRightPower += 0.1f;
                break;
        }
        Debug.Log(collision.gameObject.tag);
        Debug.Log("" + handLeftPower + handRightPower + footLeftPower + footRightPower);
    }
}