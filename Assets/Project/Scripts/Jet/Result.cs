using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        int score = PlayerPrefs.GetInt("Score");
        float elapsedTime = PlayerPrefs.GetFloat("GameTime");
        float finalScore = PlayerPrefs.GetFloat("FinalScore");

        scoreText.text = $"スコア: {score}\n生き残った時間: {elapsedTime:F2} 秒\n総合成績: {finalScore:F2}";
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            // スコアシーンに移動
            SceneManager.LoadScene("Title");
        }
    }
}
