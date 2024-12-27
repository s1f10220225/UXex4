using UnityEngine;

public class Updraft : MonoBehaviour
{
    public float windStrength = 10f; // 上昇気流の強さ（調整可能）

    public Vector3 GetWindForce(Vector3 objectPosition)
    {
        // 上昇気流の向きを現在のオブジェクトのup方向で取る
        Vector3 windDirection = transform.up;
        return windDirection * windStrength;
    }
}