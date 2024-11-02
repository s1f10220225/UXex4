using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    // public float destroyZPosition;  // 削除するZ位置

    // void Start()
    // {
    //     destroyZPosition = RandomSpawner.destroyPoint2;
    // }

    void Update()
    {
        // グローバル速度を適用してZマイナス方向に移動
        transform.Translate(Vector3.back * RandomSpawner.currentSpeed * Time.deltaTime);

        // 一定位置を超えたらオブジェクトを削除
        if (transform.position.z < -5)
        {
            JetFull.life -= 1;
            Destroy(gameObject);
        }
    }
}
