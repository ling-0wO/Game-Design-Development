using UnityEngine;

public class Laser : MonoBehaviour
{
    public float duration = 2.0f; // ���ߴ��ڵ�ʱ��

    void Start()
    {
        // ��һ��ʱ�����������
        Destroy(gameObject, duration);
    }
}
