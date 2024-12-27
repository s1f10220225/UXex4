using UnityEngine;

public class Gliding : MonoBehaviour
{
    public Transform leftHand, rightHand, leftFoot, rightFoot, bodyCenter, root;
    public float forceMultiplier = 10f; // 力の倍率
    public Vector3 maxVelocity = new Vector3(5f, 5f, 5f); // 各方向の最大速度
    public float constantGravityForce = 9.8f; // 重力加速度

    private Vector3 windDirection = Vector3.up; // 風の方向
    private float windStrength = 0f; // 風の強さ
    private Rigidbody rb;

    void Start()
    {
        rb = bodyCenter.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // グライディングの力を計算
        Vector3 liftForce = CalculateLiftForce();

        // 重力を追加
        liftForce += Vector3.down * constantGravityForce;

        // 力を加える
        rb.AddForce(liftForce);

        // 各方向の速度を制限
        LimitVelocity();

        // プレイヤーの位置を制限
        LimitPosition();
    }

    Vector3 CalculateLiftForce()
    {
        // 手足の位置を利用して法線と面積を計算
        Vector3 force1 = CalculateTriangleForce(leftHand.position, rightHand.position, bodyCenter.position);
        Vector3 force2 = CalculateTriangleForce(rightHand.position, rightFoot.position, bodyCenter.position);
        Vector3 force3 = CalculateTriangleForce(leftHand.position, leftFoot.position, bodyCenter.position);
        Vector3 force4 = CalculateTriangleForce(leftFoot.position, rightFoot.position, bodyCenter.position);

        // 合計の力を返す
        return force1 + force2 + force3 + force4;
    }

    Vector3 CalculateTriangleForce(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // 三角形を構成する2辺のベクトルを計算
        Vector3 side1 = pointB - pointA;
        Vector3 side2 = pointC - pointA;

        // 法線を計算
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        // 風の向きをプレイヤーのローカル座標に変換
        Vector3 localWindDirection = root.InverseTransformDirection(windDirection);

        // 面積を計算
        float area = 0.5f * Vector3.Cross(side1, side2).magnitude;

        // 風の向きに垂直な力（リフト力）
        float liftFactor = Vector3.Dot(normal, localWindDirection);
        Vector3 liftForce = normal * area * windStrength * liftFactor * forceMultiplier;

        // 風向きに平行な力（ドラッグ力）
        Vector3 dragForce = Vector3.ProjectOnPlane(localWindDirection, normal) * area * windStrength * forceMultiplier * Vector3 new(1, 0, 1);
        Debug.Log("" + liftForce + "" + dragForce);

        // 総合的な力を返す
        return liftForce + dragForce;
    }


    void LimitVelocity()
    {
        Vector3 velocity = rb.velocity;

        // 各軸の速度を制限
        velocity.x = Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y);
        velocity.z = Mathf.Clamp(velocity.z, -maxVelocity.z, maxVelocity.z);

        rb.velocity = velocity;
    }
    void LimitPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -6.0f, 6.0f);
        position.y = Mathf.Clamp(position.y, -5.0f, 5.0f);
        transform.position = position;
    }

    private void OnTriggerStay(Collider other)
    {
        // 上昇気流の影響を受ける
        Updraft updraft = other.GetComponent<Updraft>();
        if (updraft != null)
        {
            windDirection = updraft.transform.up;
            windStrength = updraft.windStrength;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 上昇気流の影響を解除
        if (other.GetComponent<Updraft>() != null)
        {
            windDirection = Vector3.zero;
            windStrength = 0f;
        }
    }
}
