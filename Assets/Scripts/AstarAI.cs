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
    public AstarAI Astar;
    public Evacuation Evacuation;
    public int dangerHappened = 0;
    public float speed = 3.0f;
    public float turnSpeed = 5f;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    private HumanController humanController;
    private Vector3 lastPos;
    private float unmoveTimer;
    private float maxUnmoveTime = 1.5f;
    private int stuck = 0;
    private Animator animator;
    void Start()
    {
        lastPos = transform.position;
        humanController = GetComponent<HumanController>();
        animator = GetComponent<Animator>();
    }

    public void StartPathFinding()
    {
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        targetPosition = targetObject.transform.position;
        seeker.StartPath(transform.position, targetPosition);
    }
    void FixedUpdate()
    {        
        if (path == null)
            return;
        // 判断人物当前状态
        string state = humanController.GetState();
        if (state == "Scared" || state == "Injured_Run")
        {
            // 修改危险状态
            dangerHappened = 1;
        }
        if (dangerHappened != 0)
        {
            Debug.Log("change");
            Evacuation.enabled = true;
            dangerHappened = 0;
            Astar.enabled = false;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.25f);
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
        if (Vector3.Distance(transform.position, targetObject.transform.position) < 5)
        {
            Debug.Log(Vector3.Distance(transform.position, targetObject.transform.position));
            speed = 0;
        }
        SeekWhenUnmove();
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
        humanController.SetFacingPosition(path.vectorPath[currentWaypoint]);
        // 玩家当前位置与当前的航向点距离小于一个给定值后，转向下一个航向点
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance && Vector3.Distance(transform.position, targetObject.transform.position) > 5)
        {
            if (stuck == 1)
            {
                Vector3 A = path.vectorPath[currentWaypoint];
                Vector3 B = path.vectorPath[currentWaypoint + 1];
                int WallFirst = 0;
                // 计算另外两个夹点的坐标
                Vector3 C = new Vector3(A.x, A.y, B.z); 
                Vector3 D = new Vector3(B.x, B.y, A.z);
              //  Debug.Log("C:" + C);
            //    Debug.Log("D:" +  D);
                // Perform raycast to check objects at new positions
                Collider[] hitColliders1 = Physics.OverlapSphere(C, 0.1f); 
             //   Collider[] hitColliders2 = Physics.OverlapSphere(D, 0.2f);

               
                foreach (var hitCollider in hitColliders1)
                {
                    //  Debug.Log("1:" + hitCollider.gameObject.layer);
                    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        WallFirst = 1;
                }
                foreach (var hitCollider in hitColliders1)
                {
                   //   Debug.Log("2:" + hitCollider.gameObject.layer);
                    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        WallFirst = 1;
                }
                if (WallFirst == 1)
                {
                    path.vectorPath[currentWaypoint] = D;
                }
                else path.vectorPath[currentWaypoint] = C;
                stuck = 0;
            }
            else
            {
             //   Debug.Log(path.vectorPath[currentWaypoint]);
              //  Debug.Log(path.vectorPath[currentWaypoint + 1]);
                currentWaypoint++;
                return;
            }
        }
    }

    private void SeekWhenUnmove()
    {
          //Debug.Log(Vector3.Distance(transform.position, lastPos));
        //Debug.Log(Vector3.Distance(transform.position, lastPos) > 1e-3);
        if (Vector3.Distance(transform.position, lastPos) > 1e-3)
        {
            unmoveTimer = 0;
        }
        else
        {
            unmoveTimer += Time.deltaTime;
        }
        lastPos = transform.position;
        if (unmoveTimer > maxUnmoveTime)
        {
            unmoveTimer = 0;
            stuck = 1;
            seeker.StartPath(transform.position, targetPosition);
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

