using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public string battleName;
    public BattleType[] potentialBattles;

    public bool activateOnEnter, activateOnStay, activateOnExit; // different way a battle can be activated

    private bool inArea;
    public float timeBetweenBattles = 10f;
    private float betweenBattleCounter;

    public bool deactivateAfterStarting;

    public bool cannotFlee;


    // Use this for initialization
    void Start()
    {   
        betweenBattleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 2.5f);
        CheckIfEnemyIsAlive();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementValue = PlayerController.instance.actions.FindAction("Movement").ReadValue<Vector2>();
        if (inArea && PlayerController.instance.canMove)
        {
            /*if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0  )
            {
                betweenBattleCounter -= Time.deltaTime;
            }*/

            if(movementValue.x !=0 || movementValue.y !=0)
            {
                betweenBattleCounter -= Time.deltaTime;
            }

            if (betweenBattleCounter <= 0)
            {
                betweenBattleCounter = Random.Range(timeBetweenBattles * .5f, timeBetweenBattles * 1.5f);

                StartCoroutine(StartBattleCo());
            }
        }
    }

    private void CheckIfEnemyIsAlive ()
    {
        bool enemyfound = false;
        Debug.Log("Enemy Count " + GameManager.instance.deadEnemies.Count);
        for (int i = 0; i < GameManager.instance.deadEnemies.Count; i++)
        {
            if (GameManager.instance.deadEnemies[i] == battleName)
            {
                enemyfound = true;
                break;
            }
        }

        if (enemyfound)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // activate StartBattleCo
    {
        if (other.tag == "Player")
        {
            if (activateOnEnter)
            {
                StartCoroutine(StartBattleCo());
            }
            else
            {
                inArea = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) // deactivate StartBattleCo
    {
        if (other.tag == "Player")
        {
            if (activateOnExit)
            {
                StartCoroutine(StartBattleCo());
            }
            else
            {
                inArea = false;
            }
        }
    }

    public IEnumerator StartBattleCo()// set environment for battle
    {

        UIFade.instance.SetFadeToBlack();
        GameManager.instance.battleActive = true;

        int selectedBattle = Random.Range(0, potentialBattles.Length);

        BattleManager.instance.battleName = battleName;
        BattleManager.instance.rewardItems = potentialBattles[selectedBattle].rewardItems;
        BattleManager.instance.rewardXP = potentialBattles[selectedBattle].rewardXP;

        yield return new WaitForSeconds(1.5f);

        BattleManager.instance.BattleStart(potentialBattles[selectedBattle].enemies, cannotFlee);
        UIFade.instance.SetFadeToClear();

        if (deactivateAfterStarting)
        {
            gameObject.SetActive(false);
        }

    }
}
