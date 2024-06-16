using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    public GameObject missilePrefab; // ������Ԥ����
    public float spawnInterval = 5.0f; // ���ɼ��ʱ��
    public float xMin = -50.0f; // x����Сֵ
    public float xMax = 50.0f; // x�����ֵ
    public float zRange = 50.0f; // z��Χ
    public GameObject missleParent; // ���ɵ����ĸ��ڵ�
    private float spawnTimer; // ���ɼ�ʱ��
    private bool gameBegin;

    void Start()
    {
        spawnTimer = spawnInterval; // ��ʼ�����ɼ�ʱ��
    }

    void Update()
    {
        if (gameBegin)
        {
            spawnTimer -= Time.deltaTime; // �������ɼ�ʱ��

            // ������ɼ�ʱ��С�ڵ���0������һ���µĵ���
            if (spawnTimer <= 0)
            {
                float x = Random.Range(xMin, xMax); // ���xλ��
                float z = Random.Range(-zRange, zRange); // ���zλ��
                Vector3 spawnPosition = new Vector3(x, 200.0f, z); // ����λ��

                // ������λ������һ���µĵ���
                GameObject missle = Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
                missle.transform.SetParent(missleParent.transform);

                spawnTimer = spawnInterval; // �������ɼ�ʱ��
            }
        }
    }

    public void SetGameBegin(bool begin)
    {
        gameBegin = begin;
    }
}