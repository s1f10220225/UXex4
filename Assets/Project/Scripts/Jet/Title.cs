using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            // スコアシーンに移動
            SceneManager.LoadScene("JetFull");
        }
    }
}
