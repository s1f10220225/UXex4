using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class WingSuitController : MonoBehaviour
{
    [Header("=== 基本スピード設定 ===")]
    [SerializeField] private float baseFallSpeed = 20f;    // 基本の落下速度
    [SerializeField] private float baseForwardSpeed = 10f; // 基本の前進速度
    [SerializeField] private float sideMovementFactor = 10f; // 左右移動速度の倍率

    [Header("=== 手足の影響度設定 ===")]
    [Tooltip("手を開いた際の落下速度への影響度（大きいほど手を広げると落下が遅くなる）")]
    [SerializeField] private float handEffectMultiplier = 5f;
    [Tooltip("足を開いた際の前進速度への影響度（大きいほど足を広げると前進速度が上がる）")]
    [SerializeField] private float footEffectMultiplier = 5f;

    [Header("=== クランプ範囲 ===")]
    [SerializeField] private float minFallSpeed = 5f;
    [SerializeField] private float maxFallSpeed = 40f;
    [SerializeField] private float minForwardSpeed = 5f;
    [SerializeField] private float maxForwardSpeed = 40f;
    [SerializeField] private float maxSideSpeed = 20f;

    [Header("=== 手足のTransform ===")]
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Transform leftFootTransform;
    [SerializeField] private Transform rightFootTransform;

    [Header("=== 風の影響設定 ===")]
    [SerializeField] private bool useHandDistanceForWind = false;
    [Tooltip("手をどれくらい広げているかを風の影響に乗せる場合、どれくらい倍率をかけるか")]
    [SerializeField] private float windHandMultiplier = 0.5f;
    [Tooltip("風の最大効果を制限するためのクランプ")]
    [SerializeField] private float maxWindFactor = 2f;

    // =========================== 追加したパラメータ ===========================
    [Header("=== 手動モード設定 ===")]
    [Tooltip("Escキーで手動モードとキャプチャーモードを切り替える")]
    [SerializeField] private bool isManualMode = false; 

    [Tooltip("手動モード時の前進速度を増減する量（W/S用）")]
    [SerializeField] private float manualForwardSpeedStep = 5f;

    [Tooltip("手動モード時の上下(↑/↓)で落下速度を増減する量")]
    [SerializeField] private float manualFallSpeedStep = 5f;

    [Tooltip("手動モード時の左右移動速度 (A/D)")]
    [SerializeField] private float manualSideSpeed = 10f;

    [Tooltip("手動モードでの風倍率 (一定倍率)")]
    [SerializeField] private float manualWindMultiplier = 1.0f;
    // =======================================================================

    // Rigidbody
    private Rigidbody rb;

    // 現在の落下速度・前進速度 (キャプチャーモードにおける数値)
    private float currentFallSpeed;
    private float currentForwardSpeed;

    // 手動モード専用の前進速度
    private float manualForwardCurrent;

    // 風エリア (Wind) のリスト (複数重なったら全部足し合わせ)
    private List<Wind> activeWinds = new List<Wind>();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentFallSpeed = baseFallSpeed;
        currentForwardSpeed = baseForwardSpeed;

        // 手動モード用前進速度の初期値
        manualForwardCurrent = baseForwardSpeed;
    }

    private void Update()
    {
        // ゲームがプレイ中でない場合は動かない (カウントダウン中など)
        if (!GlobalGameManager.Instance.IsGamePlaying)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // Escキーでモード切り替え (手動モード <-> キャプチャーモード)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isManualMode = !isManualMode;
        }

        // 1) モードごとの 落下速度 / 前進速度 の計算
        if (isManualMode)
        {
            // 手動モード
            ManualModeControl();
        }
        else
        {
            // キャプチャーモード (既存コード)
            AdjustSpeedsBasedOnLimbs();
        }

        // 2) 風の影響を計算
        Vector3 windMovement = CalculateWindMovement();

        // 3) 前進ベクトル & 落下ベクトル & 左右移動
        Vector3 forwardMovement;
        Vector3 downwardMovement;
        Vector3 lateralMovement;

        if (isManualMode)
        {
            // --- 手動モード ---
            // forwardMovement は manualForwardCurrent
            forwardMovement = transform.up * manualForwardCurrent;
            // 落下は currentFallSpeed
            downwardMovement = Vector3.down * currentFallSpeed;
            // 左右移動
            lateralMovement = CalculateManualLateral();
        }
        else
        {
            // --- キャプチャーモード ---
            forwardMovement = transform.up * currentForwardSpeed;
            downwardMovement = Vector3.down * currentFallSpeed;
            lateralMovement = CalculateLateralMovement(); // 既存コード
        }

        // 4) 合算して Rigidbody にセット
        rb.velocity = forwardMovement + downwardMovement + lateralMovement + windMovement;
    }

    /// <summary>
    /// キャプチャーモード (既存コード)
    /// </summary>
    private void AdjustSpeedsBasedOnLimbs()
    {
        // --- 落下速度 ---
        if (leftHandTransform && rightHandTransform)
        {
            float handDistance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
            currentFallSpeed = baseFallSpeed - handDistance * handEffectMultiplier;
        }

        // --- 前進速度 ---
        if (leftFootTransform && rightFootTransform)
        {
            float footDistance = Vector3.Distance(leftFootTransform.position, rightFootTransform.position);
            currentForwardSpeed = baseForwardSpeed + footDistance * footEffectMultiplier;
        }

        // クランプ
        currentFallSpeed   = Mathf.Clamp(currentFallSpeed,   minFallSpeed,   maxFallSpeed);
        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minForwardSpeed, maxForwardSpeed);
    }

    /// <summary>
    /// キャプチャーモード (既存コード) - 左右移動
    /// </summary>
    private Vector3 CalculateLateralMovement()
    {
        if (!leftHandTransform || !rightHandTransform) return Vector3.zero;

        float leftDist = Vector3.Distance(leftHandTransform.position, transform.position);
        float rightDist = Vector3.Distance(rightHandTransform.position, transform.position);
        float diff = leftDist - rightDist;

        float sideSpeed = diff * sideMovementFactor;
        sideSpeed = Mathf.Clamp(sideSpeed, -maxSideSpeed, maxSideSpeed);

        return transform.right * sideSpeed;
    }

    /// <summary>
    /// 風の影響を合算
    /// キャプチャーモード: useHandDistanceForWind によって手の広げ具合を掛け合わせる
    /// 手動モード: manualWindMultiplier を掛ける (常に一定)
    /// </summary>
    private Vector3 CalculateWindMovement()
    {
        Vector3 totalWind = Vector3.zero;

        float handFactor = 1f;
        if (!isManualMode)
        {
            // === キャプチャーモード ===
            if (useHandDistanceForWind && leftHandTransform && rightHandTransform)
            {
                float handDistance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
                handFactor = 1f + handDistance * windHandMultiplier;
                handFactor = Mathf.Clamp(handFactor, 1f, maxWindFactor);
            }
        }
        else
        {
            // === 手動モード ===
            handFactor = manualWindMultiplier; // 常に一定の倍率
        }

        foreach (var wind in activeWinds)
        {
            Vector3 windVec = wind.WindDirection * wind.WindStrength;
            windVec *= handFactor;
            totalWind += windVec;
        }

        return totalWind;
    }

    /// <summary>
    /// 手動モードでの制御(W,S,↑,↓)による落下速度・前進速度の調整
    /// </summary>
    private void ManualModeControl()
    {
        // --- 前進速度 (W,S) ---
        //   W: 前進速度を増やす
        //   S: 前進速度を減らす
        if (Input.GetKey(KeyCode.W))
        {
            manualForwardCurrent += manualForwardSpeedStep * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            manualForwardCurrent -= manualForwardSpeedStep * Time.deltaTime;
        }

        // クランプ (前進速度は 0～maxForwardSpeed の範囲)
        manualForwardCurrent = Mathf.Clamp(manualForwardCurrent, 0f, maxForwardSpeed);

        // --- 落下速度 (↑, ↓) ---
        //   ↑: 落下速度を減らす(より浮く)
        //   ↓: 落下速度を増やす(より落ちる)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentFallSpeed -= manualFallSpeedStep * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            currentFallSpeed += manualFallSpeedStep * Time.deltaTime;
        }

        // クランプ (落下速度)
        currentFallSpeed = Mathf.Clamp(currentFallSpeed, minFallSpeed, maxFallSpeed);
    }

    /// <summary>
    /// 手動モードでの左右移動 (A,D) 
    /// ※ 上下は落下速度で調整するので、ここでは上下移動ベクトルは付加しない
    /// </summary>
    private Vector3 CalculateManualLateral()
    {
        float side = 0f;
        if (Input.GetKey(KeyCode.A)) side -= 1f;
        if (Input.GetKey(KeyCode.D)) side += 1f;

        float sideSpeed = side * manualSideSpeed;
        sideSpeed = Mathf.Clamp(sideSpeed, -maxSideSpeed, maxSideSpeed);

        return transform.right * sideSpeed;
    }

    /// <summary>
    /// 風エリアに入った時 (既存コード)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wind>(out Wind wind))
        {
            if (!activeWinds.Contains(wind))
            {
                activeWinds.Add(wind);
            }
        }
    }

    /// <summary>
    /// 風エリアから出た時 (既存コード)
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Wind>(out Wind wind))
        {
            if (activeWinds.Contains(wind))
            {
                activeWinds.Remove(wind);
            }
        }
    }
}
