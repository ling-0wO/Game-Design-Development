using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject explosionPrefab; // ��ը�����Ԥ����
    public float explosionRadius = 30f; // ��ը�뾶

    void OnCollisionEnter(Collision collision)
    {
        // �ڵ���λ�����ɱ�ը����
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // ��ⱬը�뾶�ڵ���������
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                // ɾ����������
                Destroy(hitCollider.gameObject);
            }
        }

        // ɾ����������
        Destroy(gameObject);
     
    }
}
