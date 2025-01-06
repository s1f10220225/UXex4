using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultUI : MonoBehaviour
{
    [Header("ゲーム終了理由 (GameOver / GameClear)")]
    [SerializeField] private Text endTypeText;

    [Header("リング100点の表示 (例 '100pt Ring x 0 = 0')")]
    [SerializeField] private Text ring1Text;

    [Header("リング500点の表示 (例 '500pt Ring x 0 = 0')")]
    [SerializeField] private Text ring5Text;

    [Header("移動距離 (例 'Distance: 1234.5m')")]
    [SerializeField] private Text distanceText;

    [Header("合計スコア (リングスコア + 距離)")]
    [SerializeField] private Text totalScoreText;

    [Header("演出間隔(秒)")]
    [SerializeField] private float delayBetweenItems = 0.5f;

    private ResultSceneManager sceneManager;

    private void Start()
    {
        sceneManager = FindObjectOfType<ResultSceneManager>();

        // まず全部非表示にしておく
        if (endTypeText) endTypeText.gameObject.SetActive(false);
        if (ring1Text) ring1Text.gameObject.SetActive(false);
        if (ring5Text) ring5Text.gameObject.SetActive(false);
        if (distanceText) distanceText.gameObject.SetActive(false);
        if (totalScoreText) totalScoreText.gameObject.SetActive(false);

        // コルーチン開始
        StartCoroutine(ShowResultItems());
    }

    private IEnumerator ShowResultItems()
    {
        // 1) 終了理由: GameOver / GameClear
        GameEndReason reason = GlobalGameManager.Instance.GameEndType;

        // 2) リングの点数
        int ring1Count = GlobalGameManager.Instance.ring1Count;   // 100点リングの個数
        int ring5Count = GlobalGameManager.Instance.ring5Count;   // 500点リングの個数
        int ring1Score = ring1Count * 100;             // (100点リング×個数)
        int ring5Score = ring5Count * 500;         // (500点リング×個数)
        int sumRingScore = ring1Score + ring5Score;

        // 3) 移動距離
        float dist = GlobalGameManager.Instance.distanceTraveled; // スタート地点からの合計距離
        // 合計スコアに距離をどう加算するかは仕様次第だけど、ここでは単純に「距離を整数化して加算」する例を示す
        int distScore = Mathf.FloorToInt(dist);

        // 4) 最終合計: (リングスコア + 距離)
        int total = sumRingScore + distScore;

        // --- ステップごとに演出 ---

        // (A) ゲーム終了理由
        yield return new WaitForSeconds(delayBetweenItems);
        if (endTypeText)
        {
            endTypeText.gameObject.SetActive(true);
            if (reason == GameEndReason.GameOver)
            {
                endTypeText.text = "GAME OVER";
            }
            else if (reason == GameEndReason.GameClear)
            {
                endTypeText.text = "GAME CLEAR!";
            }
            else
            {
                endTypeText.text = "RESULT";
            }
            AudioManager.Instance.PlayResultItemSE(); // 項目表示SE
        }

        // (B) 100点リング
        yield return new WaitForSeconds(delayBetweenItems);
        if (ring1Text)
        {
            ring1Text.gameObject.SetActive(true);
            ring1Text.text = $"100ptリング x {ring1Count} = {ring1Score}";
            AudioManager.Instance.PlayResultItemSE();
        }

        // (C) 500点リング
        yield return new WaitForSeconds(delayBetweenItems);
        if (ring5Text)
        {
            ring5Text.gameObject.SetActive(true);
            ring5Text.text = $"500ptリング x {ring5Count} = {ring5Score}";
            AudioManager.Instance.PlayResultItemSE();
        }

        // (D) 距離
        yield return new WaitForSeconds(delayBetweenItems);
        if (distanceText)
        {
            distanceText.gameObject.SetActive(true);
            distanceText.text = $"距離: {dist:F1}m";
            AudioManager.Instance.PlayResultItemSE();
        }

        // (E) 最終合計スコア
        yield return new WaitForSeconds(delayBetweenItems);
        if (totalScoreText)
        {
            totalScoreText.gameObject.SetActive(true);
            totalScoreText.text = $"総合スコア: {total}";
            // 総合スコアだけ別SE
            AudioManager.Instance.PlayResultTotalSE();
        }

        // 全表示が完了したらクリックでタイトルへ戻れるように
        if (sceneManager)
        {
            sceneManager.AllowReturnToTitle();
        }
    }
}
