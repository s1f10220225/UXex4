using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    private bool canReturnToTitle = false;

    private void Update()
    {
        // 表示がまだ終わっていないなら、クリックしても何もしない
        if (!canReturnToTitle) return;

        // 結果表示完了後なら、クリックでタイトルへ戻る
        if (Input.GetMouseButtonDown(0))
        {
            // 次のプレイに備えて GlobalGameManager のデータをリセット
            GlobalGameManager.Instance.ResetAllData();

            // タイトルシーンへ
            SceneManager.LoadScene("Title");
        }
    }

    /// <summary>
    /// 結果表示が終わったタイミングで ResultUI から呼ぶ想定
    /// </summary>
    public void AllowReturnToTitle()
    {
        canReturnToTitle = true;
    }
}
