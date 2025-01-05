using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MountainGameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text countdownText; // カウントダウン表示
    [SerializeField] private float startCountdownTime = 3f; // カウントダウン秒数
    [Header("Terrain衝突でゲームオーバーになる回数")]
    [SerializeField] private int terrainCollisionLimit = 2;

    private int terrainCollisionCount = 0;
    private float countdownTimer = 0f;
    private bool isCountdown = false;
    private bool isDeviceDisconnected = false; // 操作機器の接続切れフラグ

    [Header("プレイヤー参照")]
    [SerializeField] private Transform playerTransform;

    private Rigidbody playerRigidbody;

    private void Start()
    {
        if (playerTransform)
        {
            playerRigidbody = playerTransform.GetComponent<Rigidbody>();
            GlobalGameManager.Instance.SetStartPosition(playerTransform.position);
        }
        terrainCollisionCount = 0;

        StartCountdown(); // 初回カウントダウン
    }

    private void Update()
    {
        // カウントダウン中の処理
        if (isCountdown)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                countdownTimer = 0f;
                EndCountdown();
                // カウントダウンが終わったときに CollisionHandler のフラグをOFFにする(衝突判定ON)
                var handler = FindObjectOfType<WingSuitCollisionHandler>();
                if (handler != null)
                {
                    handler.EndCollisionHandling();
                }
            }
            UpdateCountdownUI();
            return;
        }

        // ゲーム進行中の処理
        if (GlobalGameManager.Instance.IsGamePlaying && playerTransform)
        {
            GlobalGameManager.Instance.UpdateDistance(playerTransform.position);
        }
    }

    /// <summary>
    /// カウントダウンを開始
    /// </summary>
    private void StartCountdown()
    {
        isCountdown = true;
        countdownTimer = startCountdownTime;
        if (countdownText) countdownText.gameObject.SetActive(true);
        UpdateCountdownUI();

        // 一時停止 (プレイヤーの動きを止める)
        GlobalGameManager.Instance.IsGamePlaying = false;
        if (playerRigidbody)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// カウントダウン終了
    /// </summary>
    private void EndCountdown()
    {
        isCountdown = false;
        if (countdownText) countdownText.gameObject.SetActive(false);

        // 再開
        GlobalGameManager.Instance.IsGamePlaying = true;
    }

    private void UpdateCountdownUI()
    {
        if (countdownText)
        {
            int t = Mathf.CeilToInt(countdownTimer);
            countdownText.text = t.ToString();
        }
    }

    /// <summary>
    /// 地面衝突
    /// </summary>
    public void OnTerrainCollision()
    {
        terrainCollisionCount++; // 衝突回数をカウント

        if (terrainCollisionCount >= terrainCollisionLimit)
        {
            // ゲームオーバー条件を満たしたらリザルト画面へ
            GameOver();
        }
        else
        {
            // ゲームオーバー条件未満ならチェックポイントから再スタート
            RestartMidGame();
        }
    }


    private void GameOver()
    {
        GlobalGameManager.Instance.SetGameOver();
        SceneManager.LoadScene("Result");
    }

    public void GameClear()
    {
        GlobalGameManager.Instance.SetGameClear();
        SceneManager.LoadScene("Result");
    }

    /// <summary>
    /// リスタート (チェックポイントから再開)
    /// </summary>
    public void RestartMidGame()
    {
        if (GlobalGameManager.Instance.GameEndType != GameEndReason.None) return;

        // 1. チェックポイントから位置情報を取得
        Vector3 checkpointPosition = GlobalGameManager.Instance.GetCheckpointPosition();

        // 2. プレイヤーをその位置に移動
        if (playerTransform)
        {
            playerTransform.position = checkpointPosition;

            // 3. プレイヤーの物理演算をリセット
            if (playerRigidbody)
            {
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero;
            }
            Debug.Log("" + checkpointPosition);
            Debug.Log("" + playerTransform.position);
        }

        // 5. 再開準備: カウントダウンを開始
        StartCountdown();
    }

    /// <summary>
    /// 操作機器の接続が切れた場合に外部から呼び出す
    /// </summary>
    public void OnDeviceDisconnected()
    {
        if (isDeviceDisconnected) return;

        isDeviceDisconnected = true; // フラグを立てる
        GlobalGameManager.Instance.IsGamePlaying = false; // ゲーム停止

        // プレイヤーの動きを止める
        if (playerRigidbody)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        Debug.Log("操作機器の接続が切断されました。再接続を待機中...");
    }

    /// <summary>
    /// 操作機器が再接続された場合に外部から呼び出す
    /// </summary>
    public void OnDeviceReconnected()
    {
        if (!isDeviceDisconnected) return;

        isDeviceDisconnected = false; // フラグ解除
        Debug.Log("操作機器が再接続されました。ゲーム再開の準備中...");

        StartCountdown(); // その場でカウントダウンを開始
    }
}
