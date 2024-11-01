using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public float destroyZPosition;  // 削除するZ位置

    void Start()
    {
        destroyZPosition = RandomSpawner.destroyPoint2;
        Debug.Log(destroyZPosition);
    }

    void Update()
    {
        // グローバル速度を適用してZマイナス方向に移動
        transform.Translate(Vector3.back * RandomSpawner.currentSpeed * Time.deltaTime);
        Debug.Log(destroyZPosition);

        // 一定位置を超えたらオブジェクトを削除
        if (transform.position.z < destroyZPosition)
        {
            Destroy(gameObject);
        }
    }
}
