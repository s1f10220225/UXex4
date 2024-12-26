using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
    public AudioClip speedUpSound; // 効果音
    private AudioSource audioSource;
    public static int score;
    public static int life;
    private float gameTime = 0f; // ゲームの経過時間を記録

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        score = 0;
        life = 10;
    }

    void Update()
    {
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
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("" + other.gameObject.tag);
        switch (other.gameObject.tag)
        {
            case "l_hand":
            case "r_hand":
                score += 1;
                audioSource.PlayOneShot(speedUpSound);
                break;
            case "jewelry":
                score += 2;
                break;
        }
        // 触れたオブジェクトを削除
        Destroy(other.gameObject);
    }
    void GameOver()
    {
        // スコアと経過時間の計算
        float finalScore = score * 2 + (int)gameTime;
        // スコアデータを次のシーンに渡すための方法として、PlayerPrefsを使用
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetFloat("GameTime", gameTime);
        PlayerPrefs.SetFloat("FinalScore", finalScore);
        BGM bgm = FindObjectOfType<BGM>();
        if (bgm != null)
        {
            bgm.DestroyThisObject();
        }

        // スコアシーンに移動
        SceneManager.LoadScene("ResultScene");
    }
}
