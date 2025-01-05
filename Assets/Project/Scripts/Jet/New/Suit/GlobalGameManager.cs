using UnityEngine;

public enum GameEndReason
{
    None,
    GameOver,
    GameClear
}

/// <summary>
/// シーンを越えて持続するゲーム全体の進行状況を管理
/// (スコア、距離、チェックポイントなど)
/// </summary>
public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }

    // --- ゲーム状態 ---
    public bool IsGamePlaying { get; set; } = false; // カウントダウン終了後にtrue
    public GameEndReason GameEndType { get; private set; } = GameEndReason.None;

    // --- スコア関連 (リング1点, 5点) ---
    public int ring1Count { get; private set; }
    public int ring5Count { get; private set; }

    // --- 距離 ---
    // 「スタート地点からの合計移動距離」
    private Vector3 initialPosition;
    public float distanceTraveled { get; private set; }

    // --- チェックポイント ---
    // 今回: Checkpointスクリプトで指定した「restartPosition」を受け取る
    private Vector3 latestCheckpointPos;
    private int checkpointRing1Count;
    private int checkpointRing5Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetAllData();
    }

    /// <summary>
    /// 全データを初期化
    /// </summary>
    public void ResetAllData()
    {
        IsGamePlaying = false;
        GameEndType = GameEndReason.None;
        ring1Count = 0;
        ring5Count = 0;
        distanceTraveled = 0f;
        initialPosition = Vector3.zero;
        latestCheckpointPos = Vector3.zero;
        checkpointRing1Count = 0;
        checkpointRing5Count = 0;
    }

    /// <summary>
    /// Mountainシーン開始時に、プレイヤーの最初の座標を覚える
    /// </summary>
    public void SetStartPosition(Vector3 pos)
    {
        initialPosition = pos;
        latestCheckpointPos = pos;
        distanceTraveled = 0f;
    }

    /// <summary>
    /// 毎フレーム呼び出しで、スタート地点からの距離を更新
    /// </summary>
    public void UpdateDistance(Vector3 currentPos)
    {
        if (!IsGamePlaying) return;
        distanceTraveled = Vector3.Distance(initialPosition, currentPos);
    }

    /// <summary>
    /// リングを取得
    /// </summary>
    public void AddRing(int value)
    {
        if (value == 1) ring1Count++;
        else if (value == 5) ring5Count++;
    }

    /// <summary>
    /// ゲームクリア
    /// </summary>
    public void SetGameClear()
    {
        GameEndType = GameEndReason.GameClear;
        IsGamePlaying = false;
    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void SetGameOver()
    {
        GameEndType = GameEndReason.GameOver;
        IsGamePlaying = false;
    }

    /// <summary>
    /// チェックポイントを記録(Checkpointスクリプトから呼ばれる)
    /// posはインスペクタで指定された"restartPosition"を受け取る
    /// </summary>
    public void SetCheckpoint(Vector3 pos)
    {
        latestCheckpointPos = pos;
        checkpointRing1Count = ring1Count;
        checkpointRing5Count = ring5Count;
    }

    /// <summary>
    /// チェックポイント座標を返す
    /// </summary>
    public Vector3 GetCheckpointPosition()
    {
        return latestCheckpointPos;
    }

    /// <summary>
    /// ゲームオーバーになっていない再スタート時の処理(スコア調整, 位置リセット)
    /// </summary>
    public void RestartFromCheckpoint()
    {
        // リングの差分を0.1倍にして再計算
        int diff1 = ring1Count - checkpointRing1Count;
        int diff5 = ring5Count - checkpointRing5Count;

        // 整数化の方法はお好みで (切り捨て, 四捨五入など)
        int final1 = checkpointRing1Count + Mathf.FloorToInt(diff1 * 0.1f);
        int final5 = checkpointRing5Count + Mathf.FloorToInt(diff5 * 0.1f);

        ring1Count = final1;
        ring5Count = final5;

        // 位置はチェックポイントへ
        // ただし、distanceTraveledは"スタート地点"基準のまま
        // (チェックポイントを踏んでも距離は下がらない)
        // ここではいじらない
    }
}
