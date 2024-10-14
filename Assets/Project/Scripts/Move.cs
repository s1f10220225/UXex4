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

    void Start()
    {
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        // 前方へ一定速度で移動
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);


        // // 対象オブジェクトのX軸の回転角度を取得
        // float targetXRotation = targetObject.eulerAngles.x;

        // // UnityのeulerAnglesは0 ~ 360の範囲で返すため、-180 ~ 180で計算するように調整
        // if (targetXRotation > 180)
        // {
        //     targetXRotation -= 360;
        // }

        // // 対象オブジェクトのZ軸の回転角度を取得
        // float targetZRotation = targetObject.eulerAngles.z;



        // // UnityのeulerAnglesは0 ~ 360の範囲で返すため、-180 ~ 180で計算するように調整
        // if (targetZRotation > 180)
        // {
        //     targetZRotation -= 360;
        // }

        // 対象オブジェクトのローカルから見た回転角度を取得
        Vector3 targetLocalEulerAngles = targetObject.localEulerAngles;


        // 値範囲を制限
        float targetXRotation = NormalizeAngle(targetLocalEulerAngles.x);
        float targetZRotation = NormalizeAngle(targetLocalEulerAngles.z);

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