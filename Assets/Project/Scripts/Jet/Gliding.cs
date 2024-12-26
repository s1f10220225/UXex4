using UnityEngine;

public class Gliding : MonoBehaviour
{
    public Transform leftHand, rightHand, leftFoot, rightFoot, bodyCenter;
    public float dragCoefficient = 0.5f; // 空気抵抗係数

    private Vector3 additionalLiftForce = Vector3.zero;
    Rigidbody rb;
    void Start()
    {
        rb = bodyCenter.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 force = CalculateLiftForce() + additionalLiftForce;
        rb.AddForce(force);
    }

    Vector3 CalculateLiftForce()
    {
        // 4つの三角形を作成
        Vector3 force1 = CalculateTriangleLiftForce(leftHand.position, rightHand.position, bodyCenter.position);
        Vector3 force2 = CalculateTriangleLiftForce(rightHand.position, rightFoot.position, bodyCenter.position);
        Vector3 force3 = CalculateTriangleLiftForce(leftHand.position, leftFoot.position, bodyCenter.position);
        Vector3 force4 = CalculateTriangleLiftForce(rightFoot.position, leftFoot.position, bodyCenter.position);

        // 合力を計算
        return force1 + force2 + force3 + force4;
    }

    Vector3 CalculateTriangleLiftForce(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // 三角形を構成する2辺のベクトル計算
        Vector3 side1 = pointB - pointA;
        Vector3 side2 = pointC - pointA;
        Vector3 normal = Vector3.Cross(side1, side2).normalized; // 力の向きを法線方向に

        float area = 0.5f * Vector3.Cross(side1, side2).magnitude; // 三角形の面積。掛け算で気持ち早く

        // 抵抗力の計算
        Vector3 relativeVelocity = rb.velocity;
        float velocityMagnitude = relativeVelocity.magnitude;
        Vector3 resistanceForce = -dragCoefficient * area * velocityMagnitude * Vector3.Dot(relativeVelocity.normalized, normal) * normal; // 進行方向正面との角度で大きさを変更。速度とかは計算簡略化のため一部省略(しすぎ？)

        return resistanceForce;
    }


    // 上昇気流に留まっている間に呼ばれる
    private void OnTriggerStay(Collider other)
    {
        Updraft updraft = other.GetComponent<Updraft>();
        if (updraft != null)
        {
            additionalLiftForce = updraft.GetWindForce(bodyCenter.position);
        }
    }

    // 上昇気流から出たとき
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Updraft>() != null)
        {
            additionalLiftForce = Vector3.zero;
        }
    }
}