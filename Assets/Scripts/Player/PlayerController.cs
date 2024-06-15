using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public GameObject prefab; // 要实例化的预制体  
    [FormerlySerializedAs("layerMask")] public LayerMask groundLayerMask;
    public LayerMask peopleLayerMask;// 要检测的Layer的掩码  
    public float spawnTime;
    public bool canSpawn;
    public int maxPeople;
    private bool isDragging = false; // 是否正在拖动  
    private float spawnTimer = 0f;

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
        if (PlayerManager.instance.gameStarted && Input.GetMouseButtonDown(0))
        {
            ShowHumanState();
        }
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
                if (((1 << hits[0].collider.gameObject.layer) & groundLayerMask) != 0 && HumanManager.instance.childGameObject.Count < maxPeople)
                {
                    HumanManager.instance.SpawnPeopleInstance(hits[0].point);
                    // 在碰撞点的位置实例化物体  

                }
            }
        }
    }

    private void ShowHumanState()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从摄像机发出射线  

        bool ishit = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, peopleLayerMask);
        RaycastHit humanhit = hit;
        if (ishit)
        {
            
            Debug.Log(humanhit.collider.gameObject.name);
            HumanController humanController = humanhit.collider.gameObject.GetComponent<HumanController>();
                Debug.Log(humanController);
                if (!GameUI.instance.humanInfo.gameObject.activeSelf)
                {
                    GameUI.instance.humanInfo.gameObject.SetActive(true);
                }

                GameUI.instance.humanInfo.avatar.sprite = humanController.avatar;
                GameUI.instance.humanInfo.name.text = "Human " + humanController.id;
                GameUI.instance.humanInfo.character.text = "character: ";
                GameUI.instance.humanInfo.action.text = "action: " + humanController.GetState();
            }
    }
    public void SetSpawnTime(int count)
    {
        spawnTime = (float) (1.0 / count);
    }
}