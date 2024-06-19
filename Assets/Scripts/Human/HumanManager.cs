using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class HumanManager: MonoBehaviour
    {
        public static HumanManager instance;
        public GameObject spawnedPeopleParent;
        public List<GameObject> prefabs;
        public List<GameObject> childGameObject;
        public GameObject target;
        private int peopleCount = 0;
        private int livePeopleCount = 0;
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
        public void SpawnPeopleInstance(Vector3 position)
        {
            peopleCount += 1;
            livePeopleCount += 1;
            GameUI.instance.SetLivePeopleText(livePeopleCount, peopleCount);
            int prefabIndex = Random.Range(0, prefabs.Count);
            GameObject people = Instantiate(prefabs[prefabIndex], position, Quaternion.identity);
            AstarAI ai = people.GetComponent<AstarAI>();
            HumanController humanController = people.GetComponent<HumanController>();
            Evacuation evacuation = people.GetComponent<Evacuation>();
            ai.targetObject = target;
            evacuation.AstarAI = ai;
            people.transform.SetParent(spawnedPeopleParent.transform);
            HumanManager.instance.childGameObject.Add(people);
            humanController.id = HumanManager.instance.childGameObject.Count;
        }

        public void DecreasePeopleCount(int count)
        {
            livePeopleCount -= count;
            GameUI.instance.SetLivePeopleText(livePeopleCount, peopleCount);
        }
    }
}