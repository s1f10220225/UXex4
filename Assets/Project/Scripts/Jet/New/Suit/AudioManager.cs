using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioMixer Groups")]
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup seMixerGroup;
    [Header("=== BGM ===")]
    [Tooltip("タイトル & Mountainシーンで共通")]
    public AudioClip bgmTitleAndGame;
    [Tooltip("ゲームオーバー時のリザルトBGM")]
    public AudioClip bgmResultGameOver;
    [Tooltip("ゲームクリア時のリザルトBGM")]
    public AudioClip bgmResultGameClear;

    [Header("=== SE ===")]
    public AudioClip seRingGet;
    public AudioClip seResultItem;
    public AudioClip seResultTotal;

    private AudioSource bgmSource;
    private AudioSource seSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;


        seSource = gameObject.AddComponent<AudioSource>();
        seSource.loop = false;
        seSource.playOnAwake = false;
        seSource.outputAudioMixerGroup = seMixerGroup;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    /// <summary>
    /// シーンが変わったらBGMを状況に応じて切り替えます。
    /// </summary>
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        string sceneName = newScene.name;

        // TitleかMountainなら「タイトル＆ゲーム共通BGM」を鳴らし続ける
        if (sceneName == "Title" || sceneName == "Mountain")
        {
            // すでに同じBGMを再生していたら何もしない
            if (bgmSource.clip == bgmTitleAndGame && bgmSource.isPlaying)
            {
                // 途切れずに継続
                return;
            }
            else
            {
                PlayBGM(bgmTitleAndGame);
            }
        }
        else if (sceneName == "Result")
        {
            // リザルトシーン: ゲームの終わり方をチェック
            if (GlobalGameManager.Instance.GameEndType == GameEndReason.GameOver)
            {
                PlayBGM(bgmResultGameOver);
            }
            else if (GlobalGameManager.Instance.GameEndType == GameEndReason.GameClear)
            {
                PlayBGM(bgmResultGameClear);
            }
            else
            {
                // 何も設定されてないなら、一旦ゲームオーバーと同じ扱いでもいいし、無音でもいい
                PlayBGM(bgmResultGameOver);
            }
        }
    }

    /// <summary>
    /// 指定したBGMを再生します(ループ)
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return; // すでに流れてるなら何もしない

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// SEを再生(OneShot)
    /// </summary>
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        seSource.PlayOneShot(clip);
    }

    // 以下、便利メソッド
    public void PlayRingGetSE() => PlaySE(seRingGet);
    public void PlayResultItemSE() => PlaySE(seResultItem);
    public void PlayResultTotalSE() => PlaySE(seResultTotal);
}
