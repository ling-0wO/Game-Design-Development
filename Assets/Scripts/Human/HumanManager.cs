using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class HumanManager: MonoBehaviour
    {
        public static HumanManager instance;
        public List<GameObject> childGameObject;
        
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}