using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        // 初期スコアを反映
        scoreText.text = "スコア: " + JetFull.score.ToString();
    }

    void Update()
    {
        // scoreの値を毎フレーム更新
        scoreText.text = "スコア: " + JetFull.score.ToString();
    }
}
