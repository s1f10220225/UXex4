using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneStarter : MonoBehaviour
{
    public Text countdownText;
    public GameObject gameElements;

    private void Start()
    {
        // シーン開始時にゲーム要素を非アクティブにする
        gameElements.SetActive(false);
        
        // カウントダウンのコルーチンを開始
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            // カウントダウンのテキストを表示
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }

        // 0の表示
        countdownText.text = "Start!";
        yield return new WaitForSeconds(0.5f);

        // ゲーム要素をアクティブにする
        gameElements.SetActive(true);

        // カウントダウンテキストを非表示
        countdownText.enabled = false;
    }
}
