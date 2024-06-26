using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    public GameObject inputSpawnSpeed;
    public GameObject PauseUI;
    public MissileSpawner missileSpawner;
    public TMP_InputField inputField;
    public GameObject livePeople;
    public TextMeshProUGUI livePeopleText;
    [FormerlySerializedAs("HumanInfo")] public HumanInfo humanInfo;

    public bool gameStarted = false;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPeople()
    {
        PlayerManager.instance.playerController.canSpawn = !PlayerManager.instance.playerController.canSpawn;
    }

    public void SetInputSpawnSpeed()
    {
        inputSpawnSpeed.SetActive(!inputSpawnSpeed.activeSelf);
    }

    public void OnInputSpawnEndEdit()
    {
        string str = inputField.text;
        Debug.Log(str);
        if(str != null && Regex.IsMatch(str,@"^\d+$"))
            PlayerManager.instance.playerController.SetSpawnTime(int.Parse(str) > 100 ? 100 : int.Parse(str));
        else
            Debug.Log("that is not number");
    }

    public void StartOrRestartGame()
    {
        if (!gameStarted)
        {
            livePeople.SetActive(true);
            missileSpawner.SetGameBegin(true);
            PlayerManager.instance.gameStarted = true;
            PlayerManager.instance.playerController.canSpawn = false;
            PauseUI.SetActive(false);
            // 遍历parent下的所有child  
            foreach (var childObject in HumanManager.instance.childGameObject)
            {
                AstarAI astarAI = childObject.GetComponent<AstarAI>();
                astarAI.StartPathFinding();

            }
        }
        else
        {
            livePeople.SetActive(false);
            missileSpawner.SetGameBegin(false);
            PlayerManager.instance.gameStarted = true;
            PauseUI.SetActive(true);
            foreach (var childObject in HumanManager.instance.childGameObject)
            {
                Destroy(childObject);

            }
            HumanManager.instance.childGameObject.Clear();
        }

        gameStarted = !gameStarted;
    }

    public void SetLivePeopleText(int livePeople, int totalPeople)
    {
        livePeopleText.text = "Live People Count: " + livePeople.ToString() + " / " + totalPeople.ToString();
    }
}
