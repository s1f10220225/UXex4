using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JetFull : MonoBehaviour
{
    public float Force = 5f;
    public static int score = 0;
    public float maxVelocityX = 1f;
    public float maxVelocityY = 1f;
    public float maxVelocityZ = 1f;
    // public float downForce = 0.8f; // 初期の倍率
    // public float downForceIncrement = 0.2f; // 増加させる値
    public float timeToIncDownForce = 0.1f; // 増加させる間隔
    public bool isFalling = false; // 特定のフラグ
    public Transform hand_left; // 左手スラスターのトランスフォーム
    public Transform hand_right; // 右手スラスターのトランスフォーム
    // public Transform foot_left; // 左足スラスターのトランスフォーム
    // public Transform foot_right; // 右足スラスターのトランスフォーム
    private Vector3 direction;

    private Rigidbody rb; // 動かすオブジェクト(自分自身)のリジットボディ

    // 各部位に対応したアイテム取得による手足の力の強化値
    private float handLeftPower = 1.5f;
    private float handRightPower = 1.5f;
    // private float footLeftPower = 1f;
    // private float footRightPower = 1f;
    // private float initialDownForce;
    private float elapsedTime = 0f; // 経過時間を記録
    private float gameTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // initialDownForce = (handLeftPower + handRightPower + footLeftPower + footRightPower) * downForce;
    }

    // Update is called once per frame
    void Update()
{
    // 経過時間の計算
    elapsedTime += Time.deltaTime;
    gameTime += Time.deltaTime;

    // フラグが立っているか、一定時間経過した場合に落下速度を増加
    if (elapsedTime >= timeToIncDownForce || isFalling)
    {
        // initialDownForce += downForceIncrement;
        RandomSpawner.globalSpeed += 0.1f;
        elapsedTime = 0f; // 経過時間をリセット
        isFalling = false;
    }

    // 各部位の力はアイテムボーナス分を加算
    direction = (hand_left.right * handLeftPower) +
                -(hand_right.right * handRightPower);
                // (foot_left.up * footLeftPower) +
                // (foot_right.up * footRightPower);


    // 落下力の適用
    // Vector3 totalForce = direction * Force + Vector3.down * initialDownForce;
    Vector3 totalForce = direction * Force;

    rb.AddForce(totalForce);
    Vector3 velocity = rb.velocity;

    // 速度の制限
    if (Mathf.Abs(velocity.x) > maxVelocityX)
    {
        velocity.x = Mathf.Sign(velocity.x) * maxVelocityX;
    }

    if (Mathf.Abs(velocity.y) > maxVelocityY)
    {
        velocity.y = Mathf.Sign(velocity.y) * maxVelocityY;
    }

    if (Mathf.Abs(velocity.z) > maxVelocityZ)
    {
        velocity.z = Mathf.Sign(velocity.z) * maxVelocityZ;
    }

    rb.velocity = velocity;

    // プレイヤーの位置を制限
    Vector3 position = transform.position;
    position.x = Mathf.Clamp(position.x, -6.0f, 6.0f);
    position.y = Mathf.Clamp(position.y, -9.0f, 5.0f);
    transform.position = position;

    // Y座標が-8以下になったらゲームオーバー
    if (position.y < -8.0f)
    {
        GameOver();
    }
}

    // アイテムを取得した際に各部位の推進力を増加
    public void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "l_hand":
                handLeftPower += 0.1f;
                break;
            case "r_and":
                handRightPower += 0.1f;
                break;
            // case "l_foot":
            //     footLeftPower += 0.1f;
            //     break;
            // case "r_foot":
            //     footRightPower += 0.1f;
            //     break;
            case "jewelry":
                score += 1;
                isFalling = true;
                break;
        }
        // Debug.Log(collision.gameObject.tag);
        // Debug.Log("" + handLeftPower + handRightPower + footLeftPower + footRightPower);
    }
    void GameOver()
    {
    // スコアと経過時間の計算
    float finalScore = score * 2 + (int)gameTime;

    // スコアデータを次のシーンに渡すための方法として、PlayerPrefsを使用（一例）
    PlayerPrefs.SetInt("Score", score);
    PlayerPrefs.SetFloat("GameTime", gameTime);
    PlayerPrefs.SetFloat("FinalScore", finalScore);

    // スコアシーンに移動
    SceneManager.LoadScene("ResultScene");
    }
}