using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuation : MonoBehaviour
{
    public float maxVelocity; // 最大速度
    public float desiredVelocity; // 期望速度
    public float panicCoefficient; // 恐慌系数
    public float relaxationTime; // 松弛时间
    public LayerMask pedestrianLayer; // 行人层
    public LayerMask obstacleLayer; // 障碍物层
    public LayerMask dangerSourceLayer; // 危险源层
    private Vector3 currentVelocity; // 当前速度
    public Vector3 desiredDirection ; // 期望方向

    void Start()
    {
        // 初始化参数
        currentVelocity = Vector3.zero;
        desiredDirection = CalculateDangerForce().normalized;
    }

    void Update()
    {
        // 计算自驱力和排斥力
        Vector3 selfDrivenForce = CalculateSelfDrivenForce();
        Vector3 repulsiveForce = CalculateRepulsiveForce();
        // 更新速度
        currentVelocity += (selfDrivenForce + repulsiveForce) * Time.deltaTime;
        Debug.Log(currentVelocity.magnitude);
        // 如果当前速度大于最大速度，保留方向，但大小改为最大速度
        if (currentVelocity.magnitude > maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }
        currentVelocity.y = 0;
        // 更新位置
        transform.position += currentVelocity * Time.deltaTime;
    }
 
    Vector3 CalculateSelfDrivenForce()
    {
        // 计算期望速度
        Vector3 desiredVelocityVector = desiredVelocity * desiredDirection * 0.8f;

        // 计算危险源的影响
        Vector3 dangerForce = CalculateDangerForce();

        // 计算自驱力，包括危险源的影响
        Vector3 selfDrivenForce =  (desiredVelocityVector + dangerForce - currentVelocity) / relaxationTime;

        return selfDrivenForce;
    }

    Vector3 CalculateRepulsiveForce()
    {
        Vector3 repulsiveForce = Vector3.zero;

        // 获取当前行人周围的所有行人
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, pedestrianLayer);
        foreach (var hitCollider in hitColliders)
        {
            // 排除主角自己
            if (hitCollider.gameObject != gameObject)
            {
                // 计算与其他行人的排斥力
                Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                repulsiveForce += direction / (distance * distance); // 排斥力与距离的平方成反比
            }
        }

        // 计算与障碍物的排斥力

        Collider[] obstacles = Physics.OverlapSphere(transform.position, 5f, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector3 direction = (transform.position - obstacle.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            repulsiveForce += direction / (Mathf.Pow(distance,10)); // 排斥力与距离的平方成反比

        }
        return repulsiveForce;
    }



    public Vector3 CalculateDangerForce()
    {
        Vector3 dangerForce = Vector3.zero;

        // 获取当前行人周围的所有危险源
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, dangerSourceLayer);
        foreach (var hitCollider in hitColliders)
        {
            // 计算与危险源的距离
            float distanceToDanger = Vector3.Distance(transform.position, hitCollider.transform.position);

            // 计算危险源的影响（排斥力）
            // 这里我们假设恐慌系数panicCoefficient代表了公式中的A参数，松弛时间relaxationTime代表了公式中的B参数
            float forceMagnitude = panicCoefficient * Mathf.Exp((1 - distanceToDanger) / relaxationTime);
            Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
            dangerForce += direction * forceMagnitude;
        }

        return dangerForce;
    }


}
