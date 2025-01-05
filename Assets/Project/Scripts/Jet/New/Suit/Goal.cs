using UnityEngine;

/// <summary>
/// ゴール地点に触れたらクリア扱いにする。
/// </summary>
[RequireComponent(typeof(Collider))]
public class Goal : MonoBehaviour
{
    private MountainGameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<MountainGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.GameClear();
            }
        }
    }
}
