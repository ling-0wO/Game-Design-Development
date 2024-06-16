using DefaultNamespace;
using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject explosionPrefab; // ��ը�����Ԥ����
    public GameObject laserPrefab; // �����Ԥ����
    public float explosionRadius = 30f; // ��ը�뾶
    public int clickCount = 0; // ��ҵ������
    public int maxClickCount = 3; // ���������
    public Material normalMaterial; // �����Ĳ���
    public Material flashMaterial; // ��˸�Ĳ���

    void Update()
    {
        // �����ҵ������
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    clickCount++;

                    // �����������ﵽ���ֵ��������ը
                    if (clickCount >= maxClickCount)
                    {
                        // ����һ������
                        GameObject laser = Instantiate(laserPrefab, Camera.main.transform.position, Quaternion.identity);
                        LineRenderer lineRenderer = laser.GetComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, Camera.main.transform.position - new Vector3(0.0f, 2f,0f));
                        lineRenderer.SetPosition(1, transform.position);
                        Explode();
                    }
                    else
                    {
                        // ���򣬵�����˸һ��
                        StartCoroutine(Flash());
                    }
                }
            }
        }
    }

    IEnumerator Flash()
    {
        // �������Ĳ�������Ϊ��˸�Ĳ���
        GetComponent<Renderer>().material = flashMaterial;

        // �ȴ�0.1��
        yield return new WaitForSeconds(0.1f);

        // �������Ĳ��ʻָ�Ϊ�����Ĳ���
        GetComponent<Renderer>().material = normalMaterial;
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // �ڵ���λ�����ɱ�ը����
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // ��ⱬը�뾶�ڵ���������
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius * 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                // ���ݾ������������״̬
                HumanController humanController = hitCollider.gameObject.GetComponent<HumanController>();
                if (distance <= explosionRadius)
                {
                    // ɾ����������
                    Destroy(hitCollider.gameObject);
                }
                else if (distance <= explosionRadius * 3)
                {
                    // ���������״̬Ϊ injured
                    humanController.SetState("Injured_Run");
                }
                else
                {
                    // ���������״̬Ϊ scared
                    humanController.SetState("Scared");
                }
            }
        }
        // ɾ����������
        Destroy(gameObject);
    }


}
