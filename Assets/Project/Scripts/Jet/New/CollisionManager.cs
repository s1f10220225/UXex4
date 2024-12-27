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
                // 触れたオブジェクトを削除
                Destroy(other.gameObject);
                break;
            case "jewelry":
                score += 2;
                // 触れたオブジェクトを削除
                Destroy(other.gameObject);
                break;
            case "Wind":
                Debug.Log("Wind");
                break;
        }
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
