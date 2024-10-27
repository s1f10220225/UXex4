using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public static Shot Instance { get; private set; }
    public float power = 1000f;

    private void Awake()
    {
        Instance = this;
    }

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void Fire(GameObject bullet)
    {
        GameObject newBullet = Instantiate(bullet, this.transform.position, Quaternion.identity);
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();

        // カーソルの位置に向かって弾を発射
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y; // カメラの高さを考慮
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = (targetPosition - this.transform.position).normalized;

        bulletRb.AddForce(direction * power);

        // 一定時間後に弾を削除
        Destroy(newBullet, 5f); // 5秒後に弾を削除
    }
}
