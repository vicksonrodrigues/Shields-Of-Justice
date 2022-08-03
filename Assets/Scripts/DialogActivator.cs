using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour
{
    [TextArea]
    public string[] lines;

    private bool canActivate;

    public bool isPerson = true;

    public bool isInteractable;

    public bool isStartDialogue;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isStartDialogue)
        {
            StartDialog();
        }
        ActivateDialog();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            canActivate = true;
            
            if (isInteractable)
            {
                DialogManager.instance.ShowToolTip();
            }
        }

        
       
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            canActivate = false;
            if (isInteractable)
            {
                DialogManager.instance.CloseToolTip();
            }
        }

       
    }

    private void ActivateDialog()
    {
        /*if(canActivate && Input.GetButtonDown("Interact") && !DialogManager.instance.dialogBox.activeInHierarchy)
        {
            DialogManager.instance.ShowDialog(lines,isPerson);
            DialogManager.instance.ShouldActivateQuestAtEnd(questToMark, markComplete);
        }*/

        if (canActivate && PlayerController.instance.OnInteractButtonDown() && !DialogManager.instance.dialogBox.activeInHierarchy)
        {
            
            DialogManager.instance.ShowDialog(lines, isPerson);

        }
    }

    private void StartDialog()
    {
        DialogManager.instance.ShowDialog(lines, isPerson);
        isStartDialogue = false;
    }
}
