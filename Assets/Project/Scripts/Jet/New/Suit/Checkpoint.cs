using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("このチェックポイントの再スタート地点 (インスペクタで調整)")]
    [SerializeField] private Vector3 restartPosition = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GlobalGameManagerにセット
            GlobalGameManager.Instance.SetCheckpoint(restartPosition);
        }
    }
}
