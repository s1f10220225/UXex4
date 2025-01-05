using UnityEngine;

/// <summary>
/// プレイヤーがTerrainに衝突したらMountainGameManagerに通知する。
/// </summary>
[RequireComponent(typeof(Collider))]
public class WingSuitCollisionHandler : MonoBehaviour
{
    private MountainGameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<MountainGameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
        {
            if (gameManager != null)
            {
                gameManager.OnTerrainCollision();
            }
        }
    }
}
