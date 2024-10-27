using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarControl : MonoBehaviour
{
    void Start()
    {

    }

    [System.Serializable]
    public class RotationPair
    {
        // ペアで参照するターゲットオブジェクトと、その回転を適用するオブジェクト
        public Transform targetObject;
        public Transform followObject;
    }

    // ペアのリストを管理
    public List<RotationPair> rotationPairs = new List<RotationPair>();

    void Update()
    {
        // 各ペアについて処理
        foreach (var pair in rotationPairs)
        {
            if (pair.targetObject != null && pair.followObject != null)
            {
                // ターゲットのワールドローテーションを取得
                Quaternion targetRotation = pair.targetObject.rotation;

                // 動かす方のワールドローテーション
                Quaternion currentRotation = pair.followObject.rotation;

                // X軸とZ軸の回転のみを連動させる
                pair.followObject.rotation = targetRotation;
            }
        }
    }
}
