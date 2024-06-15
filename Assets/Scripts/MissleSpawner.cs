using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab; // 导弹的预制体
    public float spawnInterval = 5.0f; // 生成间隔时间
    public float xMin = -50.0f; // x的最小值
    public float xMax = 50.0f; // x的最大值
    public float zRange = 50.0f; // z范围
    public GameObject missleParent; // 生成导弹的父节点
    private float spawnTimer; // 生成计时器
    private bool gameBegin;

    void Start()
    {
        spawnTimer = spawnInterval; // 初始化生成计时器
    }

    void Update()
    {
        if (gameBegin)
        {
            spawnTimer -= Time.deltaTime; // 更新生成计时器

            // 如果生成计时器小于等于0，生成一个新的导弹
            if (spawnTimer <= 0)
            {
                float x = Random.Range(xMin, xMax); // 随机x位置
                float z = Random.Range(-zRange, zRange); // 随机z位置
                Vector3 spawnPosition = new Vector3(x, 200.0f, z); // 生成位置

                // 在生成位置生成一个新的导弹
                GameObject missle = Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
                missle.transform.SetParent(missleParent.transform);

                spawnTimer = spawnInterval; // 重置生成计时器
            }
        }
    }

    public void SetGameBegin(bool begin)
    {
        gameBegin = begin;
    }
}