using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static GameMenu instance;
    public GameObject theMenu;
    public GameObject[] windows;// All the other manin panels the menu can transform into 


    private CharStats[] playerStats;
    //Intialial Menu display values
    [Header("CharInfo Panel")]
    public TextMeshProUGUI[] nameText;
    public TextMeshProUGUI[] hpText;
    public TextMeshProUGUI[] mpText;
    public TextMeshProUGUI[] levelText;
    public TextMeshProUGUI[] expText;
    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatsHolder;

    //Stats Menu Values
    [Header("Stat Panel")]
    public GameObject[] statusButtons;
    public TextMeshProUGUI statusName;
    public TextMeshProUGUI statusHP;
    public TextMeshProUGUI statusMP;
    public TextMeshProUGUI statusStr;
    public TextMeshProUGUI statusDef;
    public TextMeshProUGUI statusWpnEqpd;
    public TextMeshProUGUI statusWpnPwr;
    public TextMeshProUGUI statusArmrEqpd;
    public TextMeshProUGUI statusArmrPwr;
    public TextMeshProUGUI statusExp;
    public Image statusImage;

    [Header("Item Panel")]
    public ItemButton[] menuItemButtons;
    public string selectedItem;
    public Item activeItem;
    public TextMeshProUGUI itemName, itemDescription, useButtonText;
    public Image itemImage;
    public GameObject itemCharChoiceMenu;
    public TextMeshProUGUI[] itemCharChoiceNames;

    public TextMeshProUGUI goldText;
    public string mainMenuName;// used to load into scene

    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        theMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DisplayMenu();
    }

    public void DisplayMenu() // Player Menu visiblity and display brief char stats
    {
        if(PlayerController.instance.OnOpenMenu() && !BattleManager.instance.battleActive)
        {
            if(theMenu.activeInHierarchy)
            {
                CloseMenu();
            }
            else
            {
                theMenu.SetActive(true);
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }
            AudioManager.instance.PlaySFX(5);
        }
    } 

    public void UpdateMainStats() // Update/intialize data for brief Player stats 
    {
        playerStats = GameManager.instance.playerStats;

        for(int i=0; i < playerStats.Length; i++)
        {
            //Debug.Log(playerStats.Length);
            if(playerStats[i].gameObject.activeInHierarchy)
            {
                charStatsHolder[i].SetActive(true);
                nameText[i].text = playerStats[i].charName;
                hpText[i].text = "HP : " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP : " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                levelText[i].text = "Lvl " + playerStats[i].playerLevel;
                expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                expSlider[i].value = playerStats[i].currentEXP;
                charImage[i].sprite = playerStats[i].charImage;
            }
            else
            {
                charStatsHolder[i].SetActive(false);
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() ;
    }

    public void ToggleWindow(int windowNumber)// Toggle between different menus option
    {
        UpdateMainStats();

        for (int i = 0; i < windows.Length; i++)
        {
            if (i == windowNumber)
            {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            }
            else
            {
                windows[i].SetActive(false);
            }
        }

        itemCharChoiceMenu.SetActive(false);
    }

    public void CloseMenu()// close the player menu
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }

        theMenu.SetActive(false);

        GameManager.instance.gameMenuOpen = false;

        itemCharChoiceMenu.SetActive(false);
    }

    public void OpenStatus() // open detailed char stats window
    {
        UpdateMainStats();

        //update the information that is shown
        StatusChar(0);

        for (int i = 0; i < statusButtons.Length; i++) //display only the active char
        {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = playerStats[i].charName;
        }
    }

    public void StatusChar(int selected)// for in depth char info in stats window
    {
        statusName.text = playerStats[selected].charName;
        statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
        statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusStr.text = playerStats[selected].strength.ToString();
        statusDef.text = playerStats[selected].defence.ToString();
        if (playerStats[selected].equippedWeapon != "")
        {
            statusWpnEqpd.text = playerStats[selected].equippedWeapon;
        }
        else

        {

            statusWpnEqpd.text = "None";

        }

        statusWpnPwr.text = playerStats[selected].weaponPower.ToString();
        if (playerStats[selected].equippedArmor != "")
        {
            statusArmrEqpd.text = playerStats[selected].equippedArmor;
        }
        else

        {

            statusArmrEqpd.text = "None";

        }
        statusArmrPwr.text = playerStats[selected].armorPower.ToString();
        statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentEXP).ToString();
        statusImage.sprite = playerStats[selected].charImage;

    }

    public void OpenItemsMenu()//open and displays items in inventory
    {
        GameManager.instance.ShowItems(menuItemButtons);
    }

    public void SelectItem(Item newItem)// fetch details of selected item
    {
        activeItem = newItem;

        if (activeItem.isItem)
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
    public void DiscardItem()// function for discard button
    {
        if (activeItem != null)
        {
            GameManager.instance.RemoveItem(activeItem.itemName);
            ResetSelection();
        }
    }

    public void UseItem(int selectChar) // function fo use/equip button
    {
        activeItem.Use(selectChar);

        CloseItemCharChoice();
        ResetSelection();


    }
    public void OpenItemCharChoice()//open choice window to select user 
    {
        itemCharChoiceMenu.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++)
        {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharChoice()// close choice window
    {
        itemCharChoiceMenu.SetActive(false);
    }


    private void ResetSelection()//reset the selected item 
    {
        SelectItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[0]));
    }

    public void SaveGame()//function for save button
    {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound()// audio for menu button
    {
       AudioManager.instance.PlaySFX(4);
    }

    public void QuitGame() // function for quit button
    {
        SceneManager.LoadScene(mainMenuName);

        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);
        Destroy(gameObject);
    }
}
