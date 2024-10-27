using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    // public Rigidbody rb;
    private Shot shot;
    public GameObject bullet;

    // 一定間隔の設定（秒数）
    public float fireInterval = 1.0f;
    private float lastShotTime;

    private void Start()
    {
        shot = Shot.Instance;
        lastShotTime = -fireInterval; // 最初からすぐに撃てるように初期化
    }

    void Update()
    {
        // 移動操作
        // float x = Input.GetAxisRaw("Horizontal");
        // float z = Input.GetAxisRaw("Vertical");
        // Vector3 direction = new Vector3(x, 0, z);
        // rb.AddForce(direction * power);

        // 現在の時間
        float currentTime = Time.time;

        // 弾の発射（指定秒数以内に既に発射されていない時のみ）
        if (Input.GetMouseButton(0) && currentTime - lastShotTime >= fireInterval)
        {
            shot.Fire(bullet);
            lastShotTime = currentTime;  // 最後に発射した時間を更新
        }
    }
}