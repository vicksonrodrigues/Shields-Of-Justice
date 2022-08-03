using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestManager : MonoBehaviour
{


    public List<string> questNames;

    public List<QuestStatus> questProgress;

    public static QuestManager instance;

    // Use this for initialization
    void Start()
    {
        instance = this;

        questNames = new List<string>();
        questProgress = new List<QuestStatus>();
        questNames.Add("NO SUCH QUEST");
        questProgress.Add(new QuestStatus { isNotStarted = true, isStarted = false, isCompleted = false });
        QuestInitializer();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Add the listener to be called when a scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;


    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        QuestInitializer();
    }

    private void QuestInitializer ()
    {
        QuestActivator[] quests = FindObjectsOfType<QuestActivator>();

        foreach (QuestActivator quest in quests)
        {
            bool similarQuest = false;
            for (int i = 0; i < questNames.Count; i++)
            {
                if (questNames[i] == quest.questName)
                {
                    similarQuest = true;
                    break;
                }
            }
            if (!similarQuest)
            {
                questNames.Add(quest.questName);
                questProgress.Add(new QuestStatus { isNotStarted = true, isStarted = false, isCompleted = false });
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(CheckIfComplete("quest test"));
            MarkQuestComplete("quest test");
            MarkQuestIncomplete("fight the demon");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveQuestData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadQuestData();
        }*/
    }

    public void SaveQuestData()
    {
        PlayerPrefs.SetInt("QuestCount_",questNames.Count);
        for (int i = 1; i < questNames.Count; i++)
        {
            PlayerPrefs.SetString("QuestName_"+i,questNames[i]);
            if (questProgress[i].isCompleted)
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i],2);
            }
            else if (questProgress[i].isStarted)
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 1);
            }
            else
            {
                PlayerPrefs.SetInt("QuestMarker_" + questNames[i], 0);
            }
        }
    }

    public void LoadQuestData()
    {
        int questCount = PlayerPrefs.GetInt("QuestCount_");

        for(int i = 1; i <questCount; i++)
        {
            questNames.Add(PlayerPrefs.GetString("QuestName_" + i));
            questProgress.Add(new QuestStatus { isNotStarted = true, isStarted = false, isCompleted = false });
        }

        for (int i = 1; i < questCount; i++)
        {
       
            int questStatus = PlayerPrefs.GetInt("QuestMarker_" + questNames[i]);

            switch(questStatus)
            {
                case 0:
                    MarkQuestStatus("isNotStarted", questNames[i]);
                    break;
                case 1:
                    MarkQuestStatus("isStarted", questNames[i]);
                    break;
                case 2:
                    MarkQuestStatus("isCompleted", questNames[i]);
                    break;

            }
        }
    }

    //New Quest System

    public int GetMarkQuestNumber(string questToFind)
    {
        for (int i = 0; i < questNames.Count; i++)
        {
            if (questNames[i] == questToFind)
            {
                return i;
            }
        }

        Debug.LogError("Quest " + questToFind + " does not exist");
        return 0;
    }

    public void MarkQuestStatus(string questStatus, string questName)
    {
        int questNumber = GetMarkQuestNumber(questName);
        switch (questStatus)
        {
            case "isStarted":
                questProgress[questNumber].isNotStarted = false;
                questProgress[questNumber].isStarted = true;
                questProgress[questNumber].isCompleted = false;
                break;

            case "isCompleted":
                questProgress[questNumber].isNotStarted = false;
                questProgress[questNumber].isStarted = false;
                questProgress[questNumber].isCompleted = true;
                break;
            case "isNotStarted":
                questProgress[questNumber].isNotStarted = true;
                questProgress[questNumber].isStarted = false;
                questProgress[questNumber].isCompleted = false;
                break;

            default:
                Debug.Log("Wrong input given for status or Quest doesn't exist");
                break;

        }
    }

    public void MarkQuestComplete( string questName)
    {
        int questNumber = GetMarkQuestNumber(questName);

        if (!questProgress[questNumber].isNotStarted && questProgress[questNumber].isStarted)
        {
            MarkQuestStatus("isCompleted", questName);
        }

    }

    public void MarkQuestStarted(string questName)
    {
        int questNumber = GetMarkQuestNumber(questName);
        
        if (questProgress[questNumber].isNotStarted && !questProgress[questNumber].isCompleted)
        {
            MarkQuestStatus("isStarted", questName);
   
        }
    }

}
