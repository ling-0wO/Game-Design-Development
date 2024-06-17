using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    public LayerMask peopleLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameUI.instance.gameStarted)
        {
            if (((1 << other.gameObject.layer) & peopleLayerMask) != 0)
            {
                Destroy(other.gameObject);
                HumanManager.instance.childGameObject.Remove(other.gameObject);
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (GameUI.instance.gameStarted)
        {
            if (((1 << other.gameObject.layer) & peopleLayerMask) != 0)
            {
                Destroy(other.gameObject);
                HumanManager.instance.childGameObject.Remove(other.gameObject);
            }
        }
    }
}
