using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using DefaultNamespace;

public class AstarAI : MonoBehaviour
{
    public GameObject targetObject;
    private Vector3 targetPosition;
    private Seeker seeker;
    public Path path;
    public float speed = 3.0f;
    public float turnSpeed = 5f;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    private HumanController humanController;
    void Start()
    {
        humanController = GetComponent<HumanController>();
    }
    void FixedUpdate()
    {
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.2f);
        bool peopleNearby = false;
        foreach (var hitCollider in hitColliders)
        {
            // 排除主角自己
            if (hitCollider.gameObject != gameObject && hitCollider.gameObject.layer == LayerMask.NameToLayer("People"))
            {
                // 检查people对象与targetObject的距离
                float peopleToTargetDistance = Vector3.Distance(hitCollider.transform.position, targetObject.transform.position);
                float selfToTargetDistance = Vector3.Distance(transform.position, targetObject.transform.position);
                if (peopleToTargetDistance < selfToTargetDistance)
                {
                    peopleNearby = true;
                    break;
                }
            }
        }
        if (peopleNearby)
        {
            speed = 0;
        }
        else
        {
            speed = 3.0f;
        }
        targetPosition = targetObject.transform.position;
        seeker.StartPath(transform.position, targetPosition);
        if (path == null)
        {
            return;
        }
        // 当前搜索点编号大于等于路径存储的总点数时，路径搜索结束
        if (currentWaypoint >= path.vectorPath.Count)
        {
     //       Debug.Log("路径搜索结束");
            return;
        }
        /*Vector3 dir = (path.vectorPath[currentWaypoint + 1] - transform.position);//.normalized;
        dir *= speed * Time.fixedDeltaTime;
        dir = new Vector3(dir.x, dir.y, 0);*/
        // 玩家转向
        /*transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
        Quaternion targetRotation = Quaternion.LookRotation(dir, new Vector3(0, 0, 1));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        Debug.Log(dir);*/
        humanController.SetVelocity(speed);
        humanController.SetFacingPosition(path.vectorPath[currentWaypoint + 1]);
        // 玩家当前位置与当前的航向点距离小于一个给定值后，转向下一个航向点
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
    /// <summary>
    /// 当寻路结束后调用这个函数
    /// </summary>
    /// <param name="p"></param>
    private void OnPathComplete(Path p)
    {
       // Debug.Log("发现这个路线" + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }
}

