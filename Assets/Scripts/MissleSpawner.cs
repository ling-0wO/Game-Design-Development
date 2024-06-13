using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab; // ������Ԥ����
    public float spawnInterval = 5.0f; // ���ɼ��ʱ��
    public float xMin = -50.0f; // x����Сֵ
    public float xMax = 50.0f; // x�����ֵ
    public float zRange = 50.0f; // z��Χ
    private float spawnTimer; // ���ɼ�ʱ��

    void Start()
    {
        spawnTimer = spawnInterval; // ��ʼ�����ɼ�ʱ��
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime; // �������ɼ�ʱ��

        // ������ɼ�ʱ��С�ڵ���0������һ���µĵ���
        if (spawnTimer <= 0)
        {
            float x = Random.Range(xMin, xMax); // ���xλ��
            float z = Random.Range(-zRange, zRange); // ���zλ��
            Vector3 spawnPosition = new Vector3(x, 200.0f, z); // ����λ��

            // ������λ������һ���µĵ���
            Instantiate(missilePrefab, spawnPosition, Quaternion.identity);

            spawnTimer = spawnInterval; // �������ɼ�ʱ��
        }
    }
}
