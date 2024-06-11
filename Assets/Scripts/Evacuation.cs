using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuation : MonoBehaviour
{
    public float maxVelocity;
    public float desiredVelocity;
    public float panicCoefficient;
    public float relaxationTime;
    public LayerMask pedestrianLayer;
    public LayerMask obstacleLayer;
    public LayerMask dangerSourceLayer;
    private Vector3 currentVelocity;
    public Vector3 desiredDirection;
    public AstarAI AstarAI;
    private int change = 0;
    // 新增用于追踪障碍物状态的字典
    private Dictionary<Collider, Vector3> previousObstaclePositions = new Dictionary<Collider, Vector3>();
    private Dictionary<Collider, float> obstacleNoMovementTime = new Dictionary<Collider, float>();

    void Start()
    {
        currentVelocity = Vector3.zero;
        desiredDirection = CalculateDangerForce().normalized;
    }

    void Update()
    {
        Vector3 selfDrivenForce = CalculateSelfDrivenForce();
        Vector3 repulsiveForce = CalculateRepulsiveForce();
        if (change == 1 )
        {
            AstarAI.enabled = true;
            this.enabled = false;
        }
        currentVelocity += (selfDrivenForce + repulsiveForce) * Time.deltaTime;

        if (currentVelocity.magnitude > maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }
        currentVelocity.y = 0;

        transform.position += currentVelocity * Time.deltaTime;
    }

    Vector3 CalculateSelfDrivenForce()
    {
        Vector3 desiredVelocityVector = desiredVelocity * desiredDirection * 0.8f;
        Vector3 dangerForce = CalculateDangerForce();
        Vector3 selfDrivenForce = (desiredVelocityVector + dangerForce - currentVelocity) / relaxationTime;

        return selfDrivenForce;
    }

    Vector3 CalculateRepulsiveForce()
    {
        Vector3 repulsiveForce = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, pedestrianLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject)
            {
                Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                repulsiveForce += direction / (distance * distance);
            }
        }

        Collider[] obstacles = Physics.OverlapSphere(transform.position, 1f, obstacleLayer);
        foreach (var obstacle in obstacles)
        {
            Vector3 direction = (transform.position - obstacle.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            distance = distance > 1 ? distance - 0.8f : 0.01f;
            repulsiveForce += direction / Mathf.Pow(distance, 10);

            // 检查障碍物是否移动
            if (previousObstaclePositions.ContainsKey(obstacle))
            {
                if (previousObstaclePositions[obstacle] == obstacle.transform.position)
                {
                    // 障碍物位置未改变，增加计时
                    obstacleNoMovementTime[obstacle] += Time.deltaTime;
                    if (obstacleNoMovementTime[obstacle] >= 2.0f)
                    {
                        change = 1;
                        Debug.Log("1");
                        obstacleNoMovementTime[obstacle] = 0; // 重新计时
                    }
                }
                else
                {
                    // 障碍物位置改变，重置计时
                    obstacleNoMovementTime[obstacle] = 0;
                }
            }
            else
            {
                // 初次记录障碍物位置
                obstacleNoMovementTime[obstacle] = 0;
            }

            // 更新记录的障碍物位置
            previousObstaclePositions[obstacle] = obstacle.transform.position;
        }

        return repulsiveForce;
    }

    public Vector3 CalculateDangerForce()
    {
        Vector3 dangerForce = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, dangerSourceLayer);
        foreach (var hitCollider in hitColliders)
        {
            float distanceToDanger = Vector3.Distance(transform.position, hitCollider.transform.position);
            float forceMagnitude = panicCoefficient * Mathf.Exp((1 - distanceToDanger) / relaxationTime);
            Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
            dangerForce += direction * forceMagnitude;
        }

        return dangerForce;
    }
}