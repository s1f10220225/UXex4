using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] prefabs;  // 生成するプレハブの配列
    public float spawnInterval = 2.0f;  // 生成の間隔
    public float MoveSpeed = 5.0f;  // 基本のオブジェクト速度

    public Transform destroyPoint; // 削除ポイント（位置）
    public static float destroyPoint2;

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
        destroyPoint2 = destroyPoint.position.z - 1f;
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
        float randomY = Random.Range(-3.0f, 3.0f);
        float z = 60.0f;
        Vector3 randomPosition = new Vector3(randomX, randomY, z);

        GameObject instance = Instantiate(prefabs[prefabIndex], randomPosition, Quaternion.identity);

        // 移動用スクリプトをアタッチ
        ItemMove ItemMove = instance.AddComponent<ItemMove>();
        // ItemMove.destroyZPosition = destroyPoint.position.z - 1f;
    }
}
