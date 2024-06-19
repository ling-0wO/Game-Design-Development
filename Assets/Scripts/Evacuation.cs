using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
public class Evacuation : MonoBehaviour
{
    public float maxVelocity;
    public float desiredVelocity;
    public float panicCoefficient;
    public float relaxationTime;
    public Evacuation Eva;
    public LayerMask pedestrianLayer;
    public LayerMask obstacleLayer;
    public LayerMask dangerSourceLayer;
    private Vector3 currentVelocity;
    public Vector3 desiredDirection;
    public AstarAI AstarAI;
    private int change = 0;
    private Animator animator;
    // 新增用于追踪障碍物状态的字典
    private Dictionary<Collider, Vector3> previousObstaclePositions = new Dictionary<Collider, Vector3>();
    private Dictionary<Collider, float> obstacleNoMovementTime = new Dictionary<Collider, float>();

    // 计时器
    private float scriptRunningTime = 0f;
    void Start()
    {
        currentVelocity = Vector3.zero;
        desiredDirection = CalculateDangerForce().normalized;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 更新计时器
        scriptRunningTime += Time.deltaTime;

        Vector3 selfDrivenForce = CalculateSelfDrivenForce();
        Vector3 repulsiveForce = CalculateRepulsiveForce();

        currentVelocity += (selfDrivenForce + repulsiveForce) * Time.deltaTime;

        if (currentVelocity.magnitude > maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }
        currentVelocity.y = 0;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, obstacleLayer);
        if (hitColliders.Length != 0)
        {
            HumanController humanController = gameObject.GetComponent<HumanController>();
            currentVelocity.x = 0;
            currentVelocity.z = 0;
            humanController.SetState("Scared");

        }
        transform.position += currentVelocity * Time.deltaTime;

        // 检查计时器是否超过7秒
        if (scriptRunningTime >= 3.0f)
        {
            change = 1;
            scriptRunningTime = 0f;
            Debug.Log("7 seconds elapsed. Changing script.");
        }
        if (change == 1)
        {
            // animator 
            HumanController humanController = gameObject.GetComponent<HumanController>();
            AstarAI.start = 1;
            AstarAI.enabled = true;
            AstarAI.speed = 5;
            change = 0;
            Eva.enabled = false;
        }
    }

    Vector3 CalculateSelfDrivenForce()
    {
        Vector3 desiredVelocityVector = desiredVelocity * desiredDirection * 0.8f;
        Vector3 dangerForce = CalculateDangerForce();
        Vector3 selfDrivenForce = (desiredVelocityVector + dangerForce - currentVelocity) / relaxationTime;
      //  Debug.Log("desiredVelocityVector:" + desiredVelocityVector + ", dangerForce" + dangerForce);
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
        // 超过范围了，人不再害怕
        if(hitColliders.Length == 0)
        {
            change = 1;
        }


        foreach (var hitCollider in hitColliders)
        {
            float distanceToDanger = Vector3.Distance(transform.position, hitCollider.transform.position);

            // 正则化 distanceToDanger 的范围到 (0, 1)
            float normalizedDistance = distanceToDanger / 100.0f;

            // 计算力的大小，使用正则化后的距离
            float forceMagnitude = panicCoefficient * Mathf.Exp((1 - normalizedDistance) / relaxationTime);
            // Debug.Log("panic" + panicCoefficient + ", 1-normalizedDis:" + (1 - normalizedDistance) + ", relax:" + relaxationTime);

            Vector3 direction = (transform.position - hitCollider.transform.position).normalized;
            dangerForce += direction * forceMagnitude;

            // Debug.Log("direction:" + direction + ",forceMan:" + forceMagnitude);
        }

        return dangerForce;
    }
}