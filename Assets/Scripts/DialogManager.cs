using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{

    public static DialogManager instance;

    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI tipText;

    public GameObject dialogBox;
    public GameObject nameBox;
    public GameObject tipsPanel;
    public GameObject startPanel;
    private bool isStartTimerComplete;
    public bool isStart;

    public string[] dialogLines;

    public int currentLine;
    private bool justStarted;

    private bool markQuestComplete;
    private bool markQuestStarted;
    private bool shouldActivateQuestSystem;
    private string questToMarkStarted;
    private string questToMarkComplete;

    //start dialogue

    // Start is called before the first frame update
    void Start()
    {
        isStart = true;
        if(isStart)
        {
            GameManager.instance.StartDialog = true;
            StartCoroutine(ShowStartDialogBox());   

        }
        
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.instance.OnInteractButtonDown() || isStartTimerComplete )
        {
            GameManager.instance.StartDialog = false;
            startPanel.SetActive(false);
        }
        if(dialogBox.activeInHierarchy)
        {
            ReadDialog();
        }
    }

    public IEnumerator ShowStartDialogBox()
    {
        yield return new WaitForSeconds(1f);
        if(!startPanel.activeInHierarchy)
        {
            
            startPanel.SetActive(true);

        }
        yield return new WaitForSeconds(7f);
        isStartTimerComplete = true;
        isStart = false;
        
        

    }
    
    public void ShowToolTip()
    {
        if (!tipsPanel.activeInHierarchy )
        {
                tipsPanel.SetActive(true);
                InputActionAsset actionAsset = PlayerController.instance.actions;
                InputAction action = actionAsset.FindAction("Interact");
                string InteractButton = action.GetBindingDisplayString();
                Debug.Log(InteractButton);
                tipText.text = "Press <sprite name=" + InteractButton + "> to interact";
        }
    }

    public void CloseToolTip()
    {
        if(tipsPanel.activeInHierarchy)
        {
            tipsPanel.SetActive(false);
        }
    }

    void ReadDialog()
    {
        //if (Input.GetButtonUp("Fire1"))2
        if(PlayerController.instance.OnInteractButtonUp())
        {
            if (!justStarted)
            {
                currentLine++;

                if (currentLine >= dialogLines.Length)
                {
                    dialogBox.SetActive(false);
                    GameManager.instance.dialogActive = false;

                    if (shouldActivateQuestSystem)
                    {
                        shouldActivateQuestSystem = false;
                        if (markQuestComplete)
                        {
                            
                            QuestManager.instance.MarkQuestComplete(questToMarkComplete);
                        }
                        if(markQuestStarted)
                        {
                            
                            QuestManager.instance.MarkQuestStarted(questToMarkStarted);
                        }
                        
                    }
                }
                else
                {
                    CheckIfName();
                    dialogText.text = dialogLines[currentLine];
                }
            }
            else
            {
                justStarted = false;
            }
            
        }
    }

    public void ShowDialog(string[] newLines ,bool isPerson)
    {
        if(tipsPanel.activeInHierarchy)
        {
            tipsPanel.SetActive(false);
        }
        dialogLines = newLines;

        currentLine = 0;

        CheckIfName();

        dialogText.text = dialogLines[currentLine];
        dialogBox.SetActive(true);
        justStarted = true;

        nameBox.SetActive(isPerson);

        GameManager.instance.dialogActive = true; 
    }

    public void CheckIfName()
    {
        if(dialogLines[currentLine].StartsWith("n-"))
        {
            nameText.text = dialogLines[currentLine].Replace("n-"," ");
            currentLine++;        }
    }

    public void ShouldCompleteQuestAtEndOfDialog(string questToMarkName)
    {
        questToMarkComplete = questToMarkName;
        markQuestComplete = true;
        if(!shouldActivateQuestSystem)
        {
            shouldActivateQuestSystem = true;
        }
        

    }

    public void ShouldActivateQuestAtEndOfDialog(string questToMarkName)
    {
        questToMarkStarted = questToMarkName;
        markQuestStarted = true;
        if (!shouldActivateQuestSystem)
        {
            shouldActivateQuestSystem = true;
        }


    }
}
