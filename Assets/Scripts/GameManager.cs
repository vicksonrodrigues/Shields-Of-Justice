using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CharStats[] playerStats;

    public bool gameMenuOpen;
    public bool dialogActive;
    public bool fadinfBetweenAreas;
    public bool shopActive;
    public bool battleActive;
    public bool StartDialog;

    public string[] itemsHeld;// Defines how many distinct items are present with the player
    public int[] numberOfItems;//Defines how many quantity of a particular item present with the player
    public Item[] referenceItems;// refers to items prefab for item details 
    public List<string> deadEnemies;
    public int currentGold;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        deadEnemies = new List<string>();
     

        DontDestroyOnLoad(gameObject);

        SortItems();

       
    }

    // Update is called once per frame
    void Update()
    {

        if (gameMenuOpen || dialogActive || fadinfBetweenAreas || shopActive || battleActive|| StartDialog)
        {
            PlayerController.instance.canMove = false;
        }
        else
        {
            PlayerController.instance.canMove = true;
        }
        /* //Testing
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddItem("Iron Armor");
            AddItem("Blabla");

            RemoveItem("Health Potion");
            RemoveItem("Bleep");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadData();
        }*/
    }

    public Item GetItemDetails(string itemToGrab) // Compare the parameter 'itemToGrab' with the reference Item and returns reference item details 
    {

        for (int i = 0; i < referenceItems.Length; i++)
        {
            if (referenceItems[i].itemName == itemToGrab)
            {
                return referenceItems[i];
            }
        }

        return null;
    }

    public void SortItems() // Sort itemsheld and its quantity
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace)
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Length - 1; i++)
            {
                if (itemsHeld[i] == "")
                {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void ShowItems(ItemButton[] Buttons)// fetch info for item's button
    {
        SortItems();
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "")
            {
                Buttons[i].buttonImage.gameObject.SetActive(true);
                Buttons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                Buttons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            }
            else
            {
                Buttons[i].buttonImage.gameObject.SetActive(false);
                Buttons[i].amountText.text = "";
            }
        }
    }

    public void AddItem(string itemToAdd)// Add item to itemheld or update the quantity
    {
        int newItemPosition = 0;
        bool foundSpace = false;

        SortItems();

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd)
            {
                newItemPosition = i;
                i = itemsHeld.Length;
                foundSpace = true;
            }
        }

        if (foundSpace)
        {
            bool itemExists = false;
            for (int i = 0; i < referenceItems.Length; i++)
            {
                if (referenceItems[i].itemName == itemToAdd)
                {
                    itemExists = true;

                    i = referenceItems.Length;
                }
            }

            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;
            }
            else
            {
                Debug.LogError(itemToAdd + " Does Not Exist!!");
            }
        }

        ShowItems(GameMenu.instance.menuItemButtons);
    }

    public void RemoveItem(string itemToRemove) // Remove item from itemheld or update the quantity
    {
        bool foundItem = false;
        int itemPosition = 0;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == itemToRemove)
            {
                foundItem = true;
                itemPosition = i;

                i = itemsHeld.Length;
            }
        }

        if (foundItem)
        {
            numberOfItems[itemPosition]--;

            if (numberOfItems[itemPosition] <= 0)
            {
                itemsHeld[itemPosition] = "";
            }

            ShowItems(GameMenu.instance.menuItemButtons);
        }
        else
        {
            Debug.LogError("Couldn't find " + itemToRemove);
        }
    }

    public void SaveData()// Save player data in playerpref
    {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

        //save character info
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentExp", playerStats[i].currentEXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentMP", playerStats[i].currentMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxMP", playerStats[i].maxMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Strength", playerStats[i].strength);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Defence", playerStats[i].defence);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_WpnPwr", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_ArmrPwr", playerStats[i].armorPower);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedWpn", playerStats[i].equippedWeapon);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedArmr", playerStats[i].equippedArmor);
        }

        //store inventory data
        for (int i = 0; i < itemsHeld.Length; i++)
        {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
        }

        //store dead enemy
        PlayerPrefs.SetInt("DeadEnemiesCount_" , deadEnemies.Count);
        for (int i = 0;i < deadEnemies.Count; i++)
        {
            PlayerPrefs.SetString("DeadEnemies_" + i, deadEnemies[i]);
        }
    }

    public void LoadData()// Load player data from playerpref
    {
        if (PlayerPrefs.HasKey("Current_Scene"))
        {
            PlayerController.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("Player_Position_x"), PlayerPrefs.GetFloat("Player_Position_y"), PlayerPrefs.GetFloat("Player_Position_z"));

            for (int i = 0; i < playerStats.Length; i++)
            {
                if (PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 0)
                {
                    playerStats[i].gameObject.SetActive(false);
                }
                else
                {
                    playerStats[i].gameObject.SetActive(true);
                }

                playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Level");
                playerStats[i].currentEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentExp");
                playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentHP");
                playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxHP");
                playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentMP");
                playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxMP");
                playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Strength");
                playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Defence");
                playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_WpnPwr");
                playerStats[i].armorPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_ArmrPwr");
                playerStats[i].equippedWeapon = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedWpn");
                playerStats[i].equippedArmor = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedArmr");
            }

            for (int i = 0; i < itemsHeld.Length; i++)
            {
                itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
                numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
            }

            int EnemiesCount = PlayerPrefs.GetInt("DeadEnemiesCount_");

            for (int i = 0; i < EnemiesCount; i++)
            {
                deadEnemies.Add(PlayerPrefs.GetString("DeadEnemies_" + i));
            }
        }
    }
}
