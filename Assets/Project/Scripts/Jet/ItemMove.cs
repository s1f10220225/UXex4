using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    public AudioClip faild; // 効果音
    private AudioSource audioSource;

    void Awake()
    {
        // "MocopiAvatar"タグが設定されたオブジェクトを探す
        GameObject mocopiAvatar = GameObject.FindWithTag("Mocopi");

        if (mocopiAvatar != null)
        {
            // オーディオソースを取得
            audioSource = mocopiAvatar.GetComponent<AudioSource>();

            // AudioSourceがアタッチされていない場合の処理
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component not found on MocopiAvatar.");
            }
        }
        else
        {
            Debug.LogError("\"MocopiAvatar\" with tag \"MocopiTag\" not found.");
        }
    }

    void Update()
    {
        // グローバル速度を適用してZマイナス方向に移動
        transform.Translate(Vector3.back * RandomSpawner.currentSpeed * Time.deltaTime);

        // 一定位置を超えたらオブジェクトを削除
        if (transform.position.z < -5)
        {
            // faildが設定されていてAudioSourceが正しく取得されている場合のみ音を再生
            if (faild != null && audioSource != null)
            {
                audioSource.PlayOneShot(faild);
            }
            else
            {
                Debug.LogWarning("Faild AudioClip or AudioSource is not set.");
            }

            JetFull.life -= 1;
            Destroy(gameObject);
        }
    }
}
