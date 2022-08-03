using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager instance;

    public bool battleActive;
    public string battleName;

    public GameObject battleScene;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();// all the char in the battle(player + enemy)

    public int currentTurn;// current char turn
    public bool turnWaiting;

    public GameObject uiButtonsHolder;//ActionPanel

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;

    public DamageNumber theDamageNumber;

    public TextMeshProUGUI[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    public BattleNotification battleNotice;

    public int chanceToFlee = 35;
    private bool fleeing;

    public string gameOverScene;

    public int rewardXP;
    public string[] rewardItems;

    public bool cannotFlee;
    private bool isBossBattle = false;

    public TextMeshProUGUI currentBattlerName;
    public GameObject enemyDetailPanel;
    public TextMeshProUGUI enemyHP, enemyMP;

    [Header("Item Menu")]
    public GameObject itemMenu;
    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public TextMeshProUGUI itemName;
    public Image itemImage;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI useButtonText;
    private GameObject currentBattlerNamePanel;

    // Use this for initialization
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Awake()
    {
        currentBattlerNamePanel = currentBattlerName.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            BattleStart(new string[] { "Skeleton" , "Turtle Snake" }, false);
        }*/

        if (battleActive)
        {
            if(!currentBattlerNamePanel.activeInHierarchy)
            {
                currentBattlerNamePanel.SetActive(true);
            }

            currentBattlerName.text = activeBattlers[currentTurn].charName;
            if (turnWaiting)
            {
                if (activeBattlers[currentTurn].isPlayer)
                {
                    if (itemMenu.activeInHierarchy)
                    {
                        uiButtonsHolder.SetActive(false);
                        currentBattlerNamePanel.SetActive(false);
                    }
                    else
                    {
                        currentBattlerNamePanel.SetActive(true);
                        enemyDetailPanel.SetActive(false);
                        uiButtonsHolder.SetActive(true);
                    }

                }
                else
                {
                    uiButtonsHolder.SetActive(false);
                    enemyDetailPanel.SetActive(true);

                    //enemy should attack
                    StartCoroutine(EnemyMoveCo());
                }
            }
            /* // Testing
            if (Input.GetKeyDown(KeyCode.N))
            {
                
                NextTurn();
            }*/
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee)// initial battle environment
    {
        if (!battleActive)
        {
            cannotFlee = setCannotFlee;

            battleActive = true;

            GameManager.instance.battleActive = true;

            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);
            battleScene.SetActive(true);



            for (int i = 0; i < playerPositions.Length; i++)
            {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy)
                {
                    for (int j = 0; j < playerPrefabs.Length; j++)
                    {
                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName)
                        {
                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);


                            CharStats thePlayer = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHp = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].wpnPower = thePlayer.weaponPower;
                            activeBattlers[i].armrPower = thePlayer.armorPower;
                        }
                    }


                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                if (enemiesToSpawn[i] != "")
                {
                    for (int j = 0; j < enemyPrefabs.Length; j++)
                    {
                        if (enemyPrefabs[j].charName == enemiesToSpawn[i])
                        {
                            if (enemyPrefabs[j].isBoss)
                            {
                                isBossBattle = true;
                                BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[6].position, enemyPositions[6].rotation);
                                newEnemy.transform.parent = enemyPositions[6];
                                activeBattlers.Add(newEnemy);
                            }
                            else
                            {
                                BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                                newEnemy.transform.parent = enemyPositions[i];
                                activeBattlers.Add(newEnemy);

                            }
                        }
                    }
                }
            }

            if (isBossBattle)
            {
                AudioManager.instance.PlayBGM(5);
            }
            else
            {
                AudioManager.instance.PlayBGM(6);
            }

            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count);
            UpdateBattle();
            UpdateUIStats();
        }
    }

    public void NextTurn()//turn function 
    {
        currentTurn++;
        if (currentTurn >= activeBattlers.Count)
        {
            currentTurn = 0;
        }

        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle()// check if chars are alive during a battle
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++)// check character health and set appropriate sprite
        {
            if (activeBattlers[i].currentHp < 0)
            {
                activeBattlers[i].currentHp = 0;
            }

            if (activeBattlers[i].currentHp == 0)
            {
                //Handle dead battler
                if (activeBattlers[i].isPlayer)
                {
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
                }
                else// for enemy
                {
                    activeBattlers[i].EnemyFade();
                }

            }
            else
            {
                if (activeBattlers[i].isPlayer)
                {
                    allPlayersDead = false;
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSprite;
                }
                else
                {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead)
        {
            if (allEnemiesDead)
            {
                //end battle in victory
                
                StartCoroutine(EndBattleCo());
            }
            else
            {
                //end battle in failure
                StartCoroutine(GameOverCo());
            }
            /*
             battleScene.SetActive(false);
            GameManager.instance.battleActive = false;
            battleActive = false; */
        }
        else
        {
            while (activeBattlers[currentTurn].currentHp == 0)//increment currentTurn value if the next char is dead
            {
                currentTurn++;
                if (currentTurn >= activeBattlers.Count)
                {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo()//automate the enemies attack 
    {
        UpdateEnemyUIStats();
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack()//Enemy attack environment

    {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)// Add all the active player
        {
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHp > 0)
            {
                players.Add(i);
            }
        }
        int selectedTarget = players[Random.Range(0, players.Count)];

        //activeBattlers[selectedTarget].currentHp -= 30;

        int selectAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectAttack])
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)// calculate damage and display damage no.
    {
        float atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].wpnPower;
        float defPwr = activeBattlers[target].defence + activeBattlers[target].armrPower;

        float damageCalc = (atkPwr / defPwr) * movePower * Random.Range(.9f, 1.1f);
        int damageToGive = Mathf.RoundToInt(damageCalc);

        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToGive + ") damage to " + activeBattlers[target].charName);

        activeBattlers[target].currentHp -= damageToGive;

        Instantiate(theDamageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damageToGive);

        UpdateUIStats();
    }

    public void UpdateEnemyUIStats()
    {
        if (!activeBattlers[currentTurn].isPlayer)
        {
            BattleChar enemy = activeBattlers[currentTurn];
            enemyHP.text = Mathf.Clamp(enemy.currentHp, 0, int.MaxValue) + "/" + enemy.maxHP;
            enemyMP.text = Mathf.Clamp(enemy.currentMP, 0, int.MaxValue) + "/" + enemy.maxMP;
            //update enemy hp and mp
        }
    }

    public void UpdateUIStats()//update the player battle stats
    {
        for (int i = 0; i < playerName.Length; i++)
        {
            if (activeBattlers.Count > i)//condition to not check out of bound element
            {
                if (activeBattlers[i].isPlayer)//display player basic stats for the battle
                {
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHp, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                    GameManager.instance.playerStats[i].currentHP = activeBattlers[i].currentHp;//update the main player stats
                    GameManager.instance.playerStats[i].currentMP = activeBattlers[i].currentMP;

                }
                else// if enemy dont display stats
                {
                    playerName[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerName[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)//set player attack power
    {

        int movePower = 0;
        for (int i = 0; i < movesList.Length; i++)
        {
            if (movesList[i].moveName == moveName)
            {
                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);

        NextTurn();

    }

    public void OpenTargetMenu(string moveName)//open target list and initial the target buttons
    {
        targetMenu.SetActive(true);

        List<int> Enemies = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (!activeBattlers[i].isPlayer)
            {
                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++)
        {
            if (Enemies.Count > i && activeBattlers[Enemies[i]].currentHp > 0)
            {
                targetButtons[i].gameObject.SetActive(true);

                targetButtons[i].moveName = moveName;
                targetButtons[i].activeBattlerTarget = Enemies[i];
                targetButtons[i].targetName.text = activeBattlers[Enemies[i]].charName;
            }
            else
            {
                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu()//open magic list and initial the target buttons
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++)
        {
            if (activeBattlers[currentTurn].movesAvailable.Length > i)
            {
                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                for (int j = 0; j < movesList.Length; j++)
                {
                    if (movesList[j].moveName == magicButtons[i].spellName)
                    {
                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }

            }
            else
            {
                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenBattleItemMenu()// Battle Item Button Toggle
    {
        if (!itemMenu.activeInHierarchy)
        {

            itemMenu.SetActive(true);
        }
    }

    public void ShowBattleItems()//Display inventory items
    {

        GameManager.instance.ShowItems(itemButtons);

    }

    public void SelectBattleItem(Item selectedItem)//select item and fetch item details
    {
        activeItem = selectedItem;
        if (selectedItem.isItem)
        {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmour)
        {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemImage.sprite = activeItem.itemSprite;
        itemDescription.text = activeItem.description;
    }

    public void UseBattleItem()
    {

        if (activeBattlers[currentTurn].isPlayer)
        {
            Debug.Log("item used on " + activeBattlers[currentTurn].charName);
            activeItem.Use(currentTurn);
        }
        GameManager.instance.SortItems();
        UpdateUIStats();
        ShowBattleItems();

    }

    public void Flee()// flee function
    {
        if (cannotFlee)
        {
            battleNotice.theText.text = "Can not flee this battle!";
            battleNotice.Activate();
        }
        else
        {
            int fleeSuccess = Random.Range(0, 100);
            if (fleeSuccess < chanceToFlee)
            {
                //end the battle
                //battleActive = false;
                //battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            }
            else
            {
                NextTurn();
                battleNotice.theText.text = "Couldn't escape!";
                battleNotice.Activate();
            }
        }

    }

    public IEnumerator EndBattleCo()// Ending the battle and reseting values
    {
        battleActive = false;
        isBossBattle = false;
        currentBattlerName.transform.parent.gameObject.SetActive(false);
        uiButtonsHolder.SetActive(false);
        enemyDetailPanel.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);
        itemMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);

        UIFade.instance.SetFadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattlers.Count; i++)
        {
            if (activeBattlers[i].isPlayer)
            {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHp;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        UIFade.instance.SetFadeToClear();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;
        //GameManager.instance.battleActive = false;
        if (fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
        }
        else
        {
            GameManager.instance.deadEnemies.Add(battleName);
            BattleReward.instance.OpenRewardScreen(rewardXP, rewardItems);
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

    public IEnumerator GameOverCo()// GameOver if battle lost
    {


        battleActive = false;
        UIFade.instance.SetFadeToBlack();
        for (int i = 0; i < activeBattlers.Count; i++)
        {
            Destroy(activeBattlers[i].gameObject);

        }
        yield return new WaitForSeconds(1.5f);
        activeBattlers.Clear();
        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}
