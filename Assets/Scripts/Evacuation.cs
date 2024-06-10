using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuation : MonoBehaviour
{
    public float maxVelocity; // ����ٶ�
    public float desiredVelocity; // �����ٶ�
    public float panicCoefficient; // �ֻ�ϵ��
    public float relaxationTime; // �ɳ�ʱ��
    public LayerMask pedestrianLayer; // ���˲�
    public LayerMask obstacleLayer; // �ϰ����
    public LayerMask dangerSourceLayer; // Σ��Դ��
    private Vector3 currentVelocity; // ��ǰ�ٶ�
    public Vector3 desiredDirection ; // ��������

    void Start()
    {
        // ��ʼ������
        currentVelocity = Vector3.zero;
        desiredDirection = CalculateDangerForce().normalized;
    }

    void Update()
    {
        // �������������ų���
        Vector3 selfDrivenForce = CalculateSelfDrivenForce();
        Vector3 repulsiveForce = CalculateRepulsiveForce();
        // �����ٶ�
        currentVelocity += (selfDrivenForce + repulsiveForce) * Time.deltaTime;
        Debug.Log(currentVelocity.magnitude);
        // �����ǰ�ٶȴ�������ٶȣ��������򣬵���С��Ϊ����ٶ�
        if (currentVelocity.magnitude > maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }
        currentVelocity.y = 0;
        // ����λ��
        transform.position += currentVelocity * Time.deltaTime;
    }
 
    Vector3 CalculateSelfDrivenForce()
    {
        // ���������ٶ�
        Vector3 desiredVelocityVector = desiredVelocity * desiredDirection * 0.8f;

        // ����Σ��Դ��Ӱ��
        Vector3 dangerForce = CalculateDangerForce();

        // ����������������Σ��Դ��Ӱ��
        Vector3 selfDrivenForce =  (desiredVelocityVector + dangerForce - currentVelocity) / relaxationTime;

        return selfDrivenForce;
    }

    Vector3 CalculateRepulsiveForce()
    {
        Vector3 repulsiveForce = Vector3.zero;

        // ��ȡ��ǰ������Χ����������
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, pedestrianLayer);
        foreach (var hitCollider in hitColliders)
        {
            // �ų������Լ�
            if (hitCollider.gameObject != gameObject)
            {
                // �������������˵��ų���
                Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                repulsiveForce += direction / (distance * distance); // �ų���������ƽ���ɷ���
            }
        }

        // �������ϰ�����ų���

        Collider[] obstacles = Physics.OverlapSphere(transform.position, 5f, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector3 direction = (transform.position - obstacle.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            repulsiveForce += direction / (Mathf.Pow(distance,10)); // �ų���������ƽ���ɷ���

        }
        return repulsiveForce;
    }



    public Vector3 CalculateDangerForce()
    {
        Vector3 dangerForce = Vector3.zero;

        // ��ȡ��ǰ������Χ������Σ��Դ
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, dangerSourceLayer);
        foreach (var hitCollider in hitColliders)
        {
            // ������Σ��Դ�ľ���
            float distanceToDanger = Vector3.Distance(transform.position, hitCollider.transform.position);

            // ����Σ��Դ��Ӱ�죨�ų�����
            // �������Ǽ���ֻ�ϵ��panicCoefficient�����˹�ʽ�е�A�������ɳ�ʱ��relaxationTime�����˹�ʽ�е�B����
            float forceMagnitude = panicCoefficient * Mathf.Exp((1 - distanceToDanger) / relaxationTime);
            Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
            dangerForce += direction * forceMagnitude;
        }

        return dangerForce;
    }


}
