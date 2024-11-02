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

    // ゲーム内のすべてのオブジェクトの移動速度を制御する変数
    public static float globalSpeed = 1.0f;

    public static float currentSpeed = 5.0f;

    private void Start()
    {
        StartCoroutine(SpawnObjects());
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
        float randomX = Random.Range(-6.0f, 6.0f);
        float randomY = Random.Range(-3.0f, 4.0f);
        float z = 60.0f;
        Vector3 randomPosition = new Vector3(randomX, randomY, z);

        GameObject instance = Instantiate(prefabs[prefabIndex], randomPosition, Quaternion.identity);

        // 移動用スクリプトをアタッチ
        ItemMove ItemMove = instance.AddComponent<ItemMove>();
        // ItemMove.destroyZPosition = destroyPoint.position.z - 1f;
    }
}
