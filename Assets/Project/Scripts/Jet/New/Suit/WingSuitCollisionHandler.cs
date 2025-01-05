using UnityEngine;

[RequireComponent(typeof(Collider))]
/// <summary>
/// プレイヤーがTerrainに衝突したらMountainGameManagerに通知する。
/// </summary>
public class WingSuitCollisionHandler : MonoBehaviour
{
    private MountainGameManager gameManager;
    private bool isHandlingCollision = false; // 衝突処理中かどうか

    private void Start()
    {
        gameManager = FindObjectOfType<MountainGameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHandlingCollision) return; // 既に衝突処理中なら無視

        if (collision.collider.CompareTag("Terrain"))
        {
            isHandlingCollision = true; // 衝突処理を開始
            if (gameManager != null)
            {
                gameManager.OnTerrainCollision();
            }
        }
    }

    /// <summary>
    /// MountainGameManager側から「衝突処理が終わったよ」と呼んでもらう
    /// </summary>
    public void EndCollisionHandling()
    {
        isHandlingCollision = false;
    }
}
