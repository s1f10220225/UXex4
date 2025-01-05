using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム中のスコアや距離表示、および +○ ポップアップ演出を管理。
/// Mountainシーンに配置する想定。
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("スコア表示 (合計)")]
    [SerializeField] private Text scoreText;
    [Header("距離表示")]
    [SerializeField] private Text distanceText;

    // [Header("リング取得の+○ポップアップのTextプレハブ(一瞬表示して消える)")]
    // [SerializeField] private GameObject ringGetPopUpPrefab;

    // [Header("ポップアップ生成位置(例: スコアの近く)")]
    // [SerializeField] private Transform popUpSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 毎フレーム、グローバル管理からスコア・距離を取得して表示更新
        UpdateScoreAndDistance();
    }

    private void UpdateScoreAndDistance()
    {
        if (scoreText)
        {
            // リングスコア合計 = (1× ring1Count + 5× ring5Count)
            int totalScore = GlobalGameManager.Instance.ring1Count
                           + GlobalGameManager.Instance.ring5Count * 5;
            scoreText.text = "スコア: " + totalScore;
        }

        if (distanceText)
        {
            float dist = GlobalGameManager.Instance.distanceTraveled;
            distanceText.text = "距離: " + dist.ToString("F1") + "m";
        }
    }

    // /// <summary>
    // /// リング取得時に +点数 を表示するポップアップを作る
    // /// </summary>
    // public void ShowRingGetPopUp(int ringValue)
    // {
    //     if (!ringGetPopUpPrefab) return;

    //     // ポップアップを生成
    //     GameObject pop = Instantiate(ringGetPopUpPrefab, popUpSpawnPoint.position, Quaternion.identity);
    //     pop.transform.SetParent(popUpSpawnPoint.parent, worldPositionStays: true);

    //     // 中のTextコンポーネントを探して "+点数" を書く
    //     Text txt = pop.GetComponentInChildren<Text>();
    //     if (txt) txt.text = "+" + ringValue;

    //     // 少しの間で消えるように
    //     Destroy(pop, 1.0f);
    // }
}
