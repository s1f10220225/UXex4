using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform targetObject;     // 追跡する対象オブジェクト
    public float moveSpeed = 5f;       // 前方への移動速度

    // 旋回速度のスケール
    public float rotationScale_X = 1f;
    public float rotationScale_Z = 0.5f;

    public Rigidbody rb;              // Rigidbodyコンポーネントへの参照

    void FixedUpdate()
    {
        // 前方へ一定速度で移動
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);

        // 対象オブジェクトのローカルから見た回転角度を取得
        Vector3 targetEulerAngles = targetObject.eulerAngles;

        // 値範囲を制限
        float targetXRotation = NormalizeAngle(targetEulerAngles.x);
        float targetZRotation = NormalizeAngle(targetEulerAngles.z);

        // 角度に応じた旋回速度を計算
        float xRotationSpeed = targetXRotation * rotationScale_X;
        float zRotationSpeed = targetZRotation * rotationScale_Z;


        // 旋回
        Quaternion deltaRotationX = Quaternion.Euler(Vector3.right * xRotationSpeed * Time.fixedDeltaTime);
        Quaternion deltaRotationZ = Quaternion.Euler(Vector3.up * -zRotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotationZ * deltaRotationX);
    }

    // 角度を-180から180の範囲に補正する(上下反転対策)
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        return angle;
    }
}