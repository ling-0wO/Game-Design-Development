using DefaultNamespace;
using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject explosionPrefab; // 爆炸物体的预制体
    public GameObject laserPrefab; // 激光的预制体
    public float explosionRadius = 30f; // 爆炸半径
    public int clickCount = 0; // 玩家点击次数
    public int maxClickCount = 3; // 最大点击次数
    public Material normalMaterial; // 正常的材质
    public Material flashMaterial; // 闪烁的材质

    void Update()
    {
        // 检测玩家的鼠标点击
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    clickCount++;

                    // 如果点击次数达到最大值，引发爆炸
                    if (clickCount >= maxClickCount)
                    {
                        // 生成一道激光
                        GameObject laser = Instantiate(laserPrefab, Camera.main.transform.position, Quaternion.identity);
                        LineRenderer lineRenderer = laser.GetComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, Camera.main.transform.position - new Vector3(0.0f, 2f,0f));
                        lineRenderer.SetPosition(1, transform.position);
                        Explode();
                    }
                    else
                    {
                        // 否则，导弹闪烁一次
                        StartCoroutine(Flash());
                    }
                }
            }
        }
    }

    IEnumerator Flash()
    {
        // 将导弹的材质设置为闪烁的材质
        GetComponent<Renderer>().material = flashMaterial;

        // 等待0.1秒
        yield return new WaitForSeconds(0.1f);

        // 将导弹的材质恢复为正常的材质
        GetComponent<Renderer>().material = normalMaterial;
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // 在导弹位置生成爆炸物体
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 检测爆炸半径内的人类物体
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius * 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                // 根据距离设置人类的状态
                HumanController humanController = hitCollider.gameObject.GetComponent<HumanController>();
                if (distance <= explosionRadius)
                {
                    // 删除人类物体
                    Destroy(hitCollider.gameObject);
                }
                else if (distance <= explosionRadius * 3)
                {
                    // 设置人类的状态为 injured
                    humanController.SetState("Injured_Run");
                }
                else
                {
                    // 设置人类的状态为 scared
                    humanController.SetState("Scared");
                }
            }
        }
        // 删除导弹物体
        Destroy(gameObject);
    }


}
