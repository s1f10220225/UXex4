using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] prefabs;  // 生成するプレハブの配列
    public float initialSpawnInterval = 2.0f;
    public float spawnInterval = 2.0f;  // 生成の間隔
    public float MoveSpeed = 5.0f;  // 基本のオブジェクト速度

    public Transform destroyPoint; // 削除ポイント（位置）
    public AudioClip faildClip; 

    // ゲーム内のすべてのオブジェクトの移動速度を制御する変数
    public static float globalSpeed = 1.0f;

    public static float currentSpeed = 5.0f;
    
    private Coroutine spawnCoroutine;
    public Transform parent;

    public float spawnRange = 5.0f; // 前回の生成位置からの許容範囲
    private Vector3? previousSpawnPosition = null; // 前回の生成位置を記録

    private void OnEnable()
    {
        // コルーチンが再開されるようにアクティブ化時に開始
        spawnCoroutine = StartCoroutine(SpawnObjects());
    }

    private void OnDisable()
    {
        // コルーチンがアクティブではないときに停止
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    private void Start() {
        globalSpeed = 1.0f;
        currentSpeed = 5.0f;
    }

    private void Update()
    {
        currentSpeed = MoveSpeed * globalSpeed;
        spawnInterval = Mathf.Max(initialSpawnInterval / globalSpeed, 0.5f); // 最小値を0.5に制限
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObject()
    {
        int prefabIndex = Random.Range(0, prefabs.Length);

        Vector3 randomPosition;
        float z = 60.0f;

        // 初回はランダム、2回目以降は前回位置に近い場所をランダムに選択
        if (previousSpawnPosition == null)
        {
            float randomX = Random.Range(-6.0f, 6.0f);
            float randomY = Random.Range(-3.0f, 4.0f);
            randomPosition = new Vector3(randomX, randomY, z);
        }
        else
        {
            float baseX = previousSpawnPosition.Value.x;
            float baseY = previousSpawnPosition.Value.y;

            // 制約内で中心に移動する傾向を作る
            float shiftX = Mathf.Sign(baseX) * Random.Range(spawnRange / 2, spawnRange);
            float shiftY = Mathf.Sign(baseY) * Random.Range(spawnRange / 2, spawnRange);

            // 生成範囲外に出ないように制限
            float randomX = Mathf.Clamp(baseX - shiftX, -6.0f, 6.0f);
            float randomY = Mathf.Clamp(baseY - shiftY, -3.0f, 4.0f);

            randomPosition = new Vector3(randomX, randomY, z);
        }

        previousSpawnPosition = randomPosition; // 現在の位置を保存

        GameObject instance = Instantiate(prefabs[prefabIndex], randomPosition, Quaternion.identity);
        ItemMove itemMoveComponent = instance.GetComponent<ItemMove>();
        if (itemMoveComponent != null) 
        {
            itemMoveComponent.faild = faildClip;
        }
        else
        {
            Debug.LogWarning("ItemMove component not found on spawned object.");
        } 
    }
}