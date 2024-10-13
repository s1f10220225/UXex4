using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform targetObject;     // 追跡する対象オブジェクト
    public float moveSpeed = 5f;       // 前方への移動速度
    public float rotationScale = 0.1f; // 旋回速度のスケール

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 前方への一定速度での移動
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // 対象オブジェクトのZ軸の回転角度を取得
        float targetZRotation = targetObject.eulerAngles.z;

        // UnityのeulerAnglesは0 ~ 360の範囲で返すため、-180 ~ 180で計算するように調整
        if (targetZRotation > 180)
        {
            targetZRotation -= 360;
        }

        // 角度に応じた旋回速度を計算
        float rotationSpeed = targetZRotation * rotationScale;

        // 旋回を適用
        transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }
}
