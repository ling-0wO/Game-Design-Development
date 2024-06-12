using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject prefab; // 要实例化的预制体  
    public LayerMask layerMask; // 要检测的Layer的掩码  
    public float spawnTime;
    public bool canSpawn;
    public GameObject spawnedPeopleParent;
    public int maxPeople;
    private bool isDragging = false; // 是否正在拖动  
    private float spawnTimer = 0f;

    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
            spawnTimer += Time.deltaTime;
        IsDragging();

        if (canSpawn)
            SpawnPeople();
    }

    private void IsDragging()
    {
        if (Input.GetMouseButtonDown(0)) // 当鼠标左键按下时  
        {
            spawnTimer = spawnTime;
            isDragging = true;

        }  
        else if (Input.GetMouseButtonUp(0))
        {
            spawnTimer = 0;
            isDragging = false;
        }
    }

    private void SpawnPeople()
    {
        if (isDragging && spawnTimer > spawnTime)
        {
            spawnTimer = 0;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从摄像机发出射线  
            
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
            if (hits.Length != 0)
            {
                if (((1 << hits[0].collider.gameObject.layer) & layerMask) != 0 && HumanManager.instance.childGameObject.Count < maxPeople)
                {
                    // 在碰撞点的位置实例化物体  
                    GameObject people = Instantiate(prefab, hits[0].point, Quaternion.identity);
                    // 这里不需要isDragging变量，因为我们只在点击时创建物体  
                    AstarAI ai = people.GetComponent<AstarAI>();
                    Evacuation evacuation = people.GetComponent<Evacuation>();
                    ai.targetObject = target;
                    evacuation.AstarAI = ai;
                    people.transform.SetParent(spawnedPeopleParent.transform);
                    HumanManager.instance.childGameObject.Add(people);
                }
            }
        }
    }

    public void SetSpawnTime(int count)
    {
        spawnTime = (float) (1.0 / count);
    }
}
