using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("スライダーの参照")]
    [SerializeField] private Slider volumeSlider;

    private const string VolumePrefKey = "GameVolume"; // 音量を保存するキー
    public float currentVolume = 0.3f; // デフォルト音量

    private void Start()
    {
        // スライダーの初期値をロード
        if (volumeSlider != null)
        {
            // 保存された音量を取得
            currentVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.3f); // デフォルトは0.3
            volumeSlider.value = currentVolume;

            // スライダーの値が変化したときのコールバックを登録
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 初期音量を適用
        SetVolume(currentVolume);
    }

    private void OnDestroy()
    {
        // スライダーのリスナーを解除
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }

    /// <summary>
    /// 音量を設定
    /// </summary>
    /// <param name="volume">新しい音量 (0.0〜1.0)</param>
    public void SetVolume(float volume)
    {
        currentVolume = volume;

        // AudioListenerの音量を変更
        AudioListener.volume = currentVolume;

        // 音量を保存
        PlayerPrefs.SetFloat(VolumePrefKey, currentVolume);
        PlayerPrefs.Save();
    }
}
