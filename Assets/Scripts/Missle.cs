using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject explosionPrefab; // 爆炸物体的预制体
    public float explosionRadius = 30f; // 爆炸半径

    void OnCollisionEnter(Collision collision)
    {
        // 在导弹位置生成爆炸物体
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 检测爆炸半径内的人类物体
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                // 删除人类物体
                Destroy(hitCollider.gameObject);
            }
        }

        // 删除导弹物体
        Destroy(gameObject);
     
    }
}
