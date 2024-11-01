using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private Text scoreText;

    void Start()
    {
        // 初期スコアを反映
        scoreText.text = "スコア: " + score.ToString();
    }

    void Update()
    {
        // scoreの値を毎フレーム更新
        scoreText.text = "スコア: " + score.ToString();
    }
}
