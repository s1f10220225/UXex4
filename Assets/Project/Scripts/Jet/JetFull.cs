using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JetFull : MonoBehaviour
{
    public float Force = 5f;
    public static int score = 0;
    public static int life = 10;
    public float maxVelocityX = 1f;
    public float maxVelocityY = 1f;
    public float maxVelocityZ = 1f;
    public float timeToIncDownForce = 0.1f; // 増加させる間隔
    public float TmpPower = 1.5f;
    public Transform hand_left; // 左手スラスターのトランスフォーム
    public Transform hand_right; // 右手スラスターのトランスフォーム
    private Rigidbody rb; // 動かすオブジェクト(自分自身)のリジットボディ
    private Vector3 direction; // 推進方向
    private float handLeftPower = 1.5f; // 左手の推進力強化
    private float handRightPower = 1.5f; // 右手の推進力強化
    private float elapsedTime = 0f; // 経過時間を記録
    private float gameTime = 0f; // ゲームの経過時間を記録
    private bool isPowerBoostActive = false; // 力の増加を追跡するフラグ
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        // 経過時間の計算
        elapsedTime += Time.deltaTime;
        gameTime += Time.deltaTime;
        // フラグが立っているか、一定時間経過した場合に落下速度を増加
        if (elapsedTime >= timeToIncDownForce)
        {
            RandomSpawner.globalSpeed += 0.1f;
            elapsedTime = 0f; // 経過時間をリセット
        }
        // 各部位の力はアイテムボーナス分を加算
        direction = (hand_left.right * handLeftPower) + -(hand_right.right * handRightPower);
        Vector3 totalForce = direction * Force;
        // 力を加える
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
        position.y = Mathf.Clamp(position.y, -5.0f, 5.0f);
        transform.position = position;
        // ライフがなくなったらゲームオーバー
        if (life < 1)
        {
            GameOver();
        }
    }
    // アイテムを取得した際に各部位の推進力を増加
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("" + other.gameObject.tag);
        switch (other.gameObject.tag)
        {
            case "l_hand":
                if (!isPowerBoostActive)
                {
                    score += 1;
                    StartCoroutine(BoostHandPowerCoroutine(true));
                }
                break;
            case "r_hand":
                if (!isPowerBoostActive)
                {
                    score += 1;
                    StartCoroutine(BoostHandPowerCoroutine(false));
                }
                break;
            case "jewelry":
                score += 2;
                break;
        }
        // 触れたオブジェクトを削除
        Destroy(other.gameObject);
    }
    // 一時的に手の力とグローバル速度を増加させるコルーチン
    private IEnumerator BoostHandPowerCoroutine(bool isLeftHand)
    {
        isPowerBoostActive = true;
        float originalPower = isLeftHand ? handLeftPower : handRightPower;
        if (isLeftHand)
        {
            handLeftPower *= TmpPower;
        }
        else
        {
            handRightPower *= TmpPower;
        }
        float originalSpeed = RandomSpawner.globalSpeed;
        RandomSpawner.globalSpeed *= TmpPower;
        yield return new WaitForSeconds(2f);

        if (isLeftHand)
        {
            handLeftPower = originalPower;
        }
        else
        {
            handRightPower = originalPower;
        }
        RandomSpawner.globalSpeed = originalSpeed;
        isPowerBoostActive = false;
    }
    void GameOver()
    {
        // スコアと経過時間の計算
        float finalScore = score * 2 + (int)gameTime;
        // スコアデータを次のシーンに渡すための方法として、PlayerPrefsを使用
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetFloat("GameTime", gameTime);
        PlayerPrefs.SetFloat("FinalScore", finalScore);
        // スコアシーンに移動
        SceneManager.LoadScene("ResultScene");
    }
}