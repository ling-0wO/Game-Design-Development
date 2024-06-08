using UnityEngine;
using System.Collections.Generic;

public class OrcaAgent : MonoBehaviour
{
    public float radius = 0.5f; // 碰撞半径
    public float maxSpeed = 2.0f; // 最大速度
    public Vector3 velocity; // 当前速度
    public Vector3 targetDirection; // 目标方向

    void Start()
    {
        velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * maxSpeed;
        targetDirection = velocity.normalized;
    }

    void Update()
    {
        List<OrcaAgent> neighbors = FindNeighbors();
        Vector3 avoidanceVector = ComputeOrca(neighbors);

        // 添加避让矢量到目标方向上
        Vector3 newDirection = targetDirection + avoidanceVector.normalized;
        newDirection.y = 0; // 确保没有竖直方向上的分量
        newDirection = newDirection.normalized;

        // 根据新的方向更新速度、位置、目标方向
        velocity = newDirection * maxSpeed;
        velocity.y = 0; 
        transform.position += velocity * Time.deltaTime; 
        targetDirection = newDirection;
    }

    // 查找邻居
    List<OrcaAgent> FindNeighbors()
    {
        List<OrcaAgent> neighbors = new List<OrcaAgent>();
        Collider[] hits = Physics.OverlapSphere(transform.position, radius * 2);
        foreach (var hit in hits)
        {
            OrcaAgent agent = hit.GetComponent<OrcaAgent>();
            if (agent != null && agent != this)
            {
                neighbors.Add(agent);
            }
        }
        return neighbors;
    }

    // 计算避让矢量
    Vector3 ComputeOrca(List<OrcaAgent> neighbors)
    {
        Vector3 avoidance = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            Vector3 relativePos = neighbor.transform.position - transform.position;
            relativePos.y = 0; 
            Vector3 relativeVel = neighbor.velocity - velocity;
            relativeVel.y = 0; 
            float distSq = relativePos.sqrMagnitude;
            float combinedRadius = radius + neighbor.radius;
            float combinedRadiusSq = combinedRadius * combinedRadius;

            if (distSq < combinedRadiusSq)
            {
                Vector3 w = relativeVel - relativePos / Mathf.Sqrt(distSq) * Mathf.Max(0, (Mathf.Sqrt(combinedRadiusSq) - Mathf.Sqrt(distSq)));
                avoidance += w;
            }
        }
        return avoidance;
    }
}