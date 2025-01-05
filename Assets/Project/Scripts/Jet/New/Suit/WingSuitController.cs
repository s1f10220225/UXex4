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

    // Rigidbody (移動用)
    private Rigidbody rb;

    // 現在の落下速度・前進速度
    private float currentFallSpeed;
    private float currentForwardSpeed;

    // 風エリア (Wind) のリスト (複数重なったら全部足し合わせ)
    private List<Wind> activeWinds = new List<Wind>();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentFallSpeed = baseFallSpeed;
        currentForwardSpeed = baseForwardSpeed;
    }

    private void Update()
    {
        // ゲームがプレイ中でない場合は動かない (カウントダウン中など)
        if (!GlobalGameManager.Instance.IsGamePlaying)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // 手足の開き具合で 落下速度 / 前進速度 を決定
        AdjustSpeedsBasedOnLimbs();

        // 風の影響を計算
        Vector3 windMovement = CalculateWindMovement();

        // 前進ベクトル & 落下ベクトル & 左右移動
        Vector3 forwardMovement = transform.up * currentForwardSpeed;
        Vector3 downwardMovement = Vector3.down * currentFallSpeed;
        Vector3 lateralMovement = CalculateLateralMovement();

        // 総合速度
        rb.velocity = forwardMovement + downwardMovement + lateralMovement + windMovement;
    }

    /// <summary>
    /// 手足の開き具合によって落下速度・前進速度を調整する
    /// </summary>
    private void AdjustSpeedsBasedOnLimbs()
    {
        // --- 落下速度 ---
        if (leftHandTransform && rightHandTransform)
        {
            float handDistance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
            // 手を広げるほど落下が遅くなる (baseFallSpeed - 距離*係数)
            currentFallSpeed = baseFallSpeed - handDistance * handEffectMultiplier;
        }

        // --- 前進速度 ---
        if (leftFootTransform && rightFootTransform)
        {
            float footDistance = Vector3.Distance(leftFootTransform.position, rightFootTransform.position);
            // 足を広げるほど前進速度が上がる (baseForwardSpeed + 距離*係数)
            currentForwardSpeed = baseForwardSpeed + footDistance * footEffectMultiplier;
        }

        // クランプ適用
        currentFallSpeed = Mathf.Clamp(currentFallSpeed, minFallSpeed, maxFallSpeed);
        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minForwardSpeed, maxForwardSpeed);
    }

    /// <summary>
    /// 左右移動 (手の左右差で計算)
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
    /// 風エリア (Wind) の影響を合算する
    /// ・useHandDistanceForWind が true なら、手の広げ具合を風の影響にも反映
    /// </summary>
    private Vector3 CalculateWindMovement()
    {
        Vector3 totalWind = Vector3.zero;

        // 手の広げ具合 (オプション)
        float handFactor = 1f;
        if (useHandDistanceForWind && leftHandTransform && rightHandTransform)
        {
            float handDistance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
            // たとえば「1 + (手の距離 * windHandMultiplier)」で倍率を上げる
            handFactor = 1f + handDistance * windHandMultiplier;
            // 上限を設定
            handFactor = Mathf.Clamp(handFactor, 1f, maxWindFactor);
        }

        foreach (var wind in activeWinds)
        {
            // 風の基本ベクトル
            Vector3 windVec = wind.WindDirection * wind.WindStrength;

            // 手の広げ具合を乗算する (オプション)
            windVec *= handFactor;

            totalWind += windVec;
        }

        return totalWind;
    }

    /// <summary>
    /// 風エリアに入った時
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
    /// 風エリアから出た時
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
