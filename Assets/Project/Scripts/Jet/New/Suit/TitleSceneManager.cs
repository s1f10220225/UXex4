using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// ボタンの OnClick() に設定して呼び出す
    /// </summary>
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Mountain");
    }
}
