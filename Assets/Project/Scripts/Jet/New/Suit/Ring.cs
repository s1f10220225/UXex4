using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ring : MonoBehaviour
{
    [Header("このリングの点数 (1 or 5)")]
    [SerializeField] private int ringValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalGameManager.Instance.AddRing(ringValue);

            // リング取得SE
            AudioManager.Instance.PlayRingGetSE();

            // TODO: ゲーム中UIに+○表示 (UIManager.Instance.ShowRingGetPopUp(ringValue); 等)
            // ここでは省略

            Destroy(gameObject);
        }
    }
}
