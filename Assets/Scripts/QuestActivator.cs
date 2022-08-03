using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActivator : MonoBehaviour
{

    public string questName;
    public bool isObject;
    public bool isEnemy;
    public bool isNPC;

    public bool activateOnEnter;
    private bool activateOnInteract = false ;
    public bool deactivateOnQuestActivation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activateOnInteract && PlayerController.instance.OnInteractButtonDown())
        {
            activateOnInteract = false;
            ActivateQuest();
        }


        if (BattleReward.instance.rewardScreen.activeInHierarchy && isEnemy)
        {
            ActivateQuest();
        }
    }

    public void ActivateQuest()
    {
       
        
        if(isObject)
        {
            QuestManager.instance.MarkQuestStarted(questName);
            gameObject.SetActive(!deactivateOnQuestActivation);
        }

        if(isNPC)
        {
            DialogManager.instance.ShouldActivateQuestAtEndOfDialog(questName);
        }

        if(isEnemy)
        {
            QuestManager.instance.MarkQuestStarted(questName);
            BattleReward.instance.questToMarkName = questName;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            if (activateOnEnter)
            {
                ActivateQuest();
            }
            else
            {
                activateOnInteract = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            activateOnInteract = false;
        }
    }
}
