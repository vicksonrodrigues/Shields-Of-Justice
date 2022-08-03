using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestComplete : MonoBehaviour
{
   
    public string questName;
    public GameObject objectToActivate;
    public bool activateObjectIfComplete;
    public bool deactivateCurrentObjectIfComplete;

    public bool isEnemy;
    public bool isObject;
    public bool isNPC;

    public bool activateCompleteOnEnter;
    private bool activateCompleteOnInteract;

    private bool initialCheckDone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialCheckDone)
        {
            initialCheckDone = true;

            QuestObjectActivator();
        }

        if (activateCompleteOnInteract && PlayerController.instance.OnInteractButtonDown())
        {
            activateCompleteOnInteract = false;
            CompletedQuest();
        }


    }

    public void CompletedQuest ()
    {

        if (isObject)
        {
            QuestManager.instance.MarkQuestComplete(questName);
            
        }

        if (isNPC)
        {
            DialogManager.instance.ShouldCompleteQuestAtEndOfDialog(questName);
        }

        if (isEnemy)
        {
            BattleReward.instance.markQuestComplete = true;
            BattleReward.instance.questToMarkName = questName;
        }
        QuestObjectActivator();
        

    }
   

    public void QuestObjectActivator ()
    {
        if(QuestManager.instance.questProgress[QuestManager.instance.GetMarkQuestNumber(questName)].isCompleted)
        {
            if(activateObjectIfComplete)
            {
                objectToActivate.SetActive(activateObjectIfComplete);
            }

            if (deactivateCurrentObjectIfComplete)
            {
                gameObject.SetActive(false);
            }


        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            if (activateCompleteOnEnter)
            {
                CompletedQuest();
            }
            else
            {
                activateCompleteOnInteract = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            activateCompleteOnInteract = false;
        }
    }


}
