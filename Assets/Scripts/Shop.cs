using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{

    public static Shop instance;

    public GameObject shopMenu;
    public GameObject buyMenu;
    public GameObject sellMenu;

    public TextMeshProUGUI goldText;

    public string[] itemsForSale;

    public ItemButton[] buyItemButtons;
    public ItemButton[] sellItemButtons;

    public Item selectedItem;
    public TextMeshProUGUI buyItemName, buyItemDescription, buyItemValue;
    public Image buyItemImage;
    public TextMeshProUGUI sellItemName, sellItemDescription, sellItemValue;
    public Image sellItemImage;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.K) && !shopMenu.activeInHierarchy)
        {
            OpenShop();
        }*/
    }

    public void OpenShop()
    {
       
        DialogManager.instance.CloseToolTip();
    
        shopMenu.SetActive(true);
        OpenBuyMenu();

        GameManager.instance.shopActive = true;

        goldText.text = GameManager.instance.currentGold.ToString();

    }

    public void CloseShop()
    {
        shopMenu.SetActive(false);
        GameManager.instance.shopActive = false;

    }

    public void OpenBuyMenu()
    {
        buyItemButtons[0].Press();
        buyMenu.SetActive(true);
        sellMenu.SetActive(false);

        for (int i = 0; i < buyItemButtons.Length; i++)
        {
            buyItemButtons[i].buttonValue = i;

            if (itemsForSale[i] != "")
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(true);
                buyItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(itemsForSale[i]).itemSprite;
                buyItemButtons[i].amountText.text = "";
            }
            else
            {
                buyItemButtons[i].buttonImage.gameObject.SetActive(false);
                buyItemButtons[i].amountText.text = "";
            }
        }

    }

    public void OpenSellMenu()
    {
        sellItemButtons[0].Press();

        buyMenu.SetActive(false);
        sellMenu.SetActive(true);

        ShowSellItems();

    }

    private void ShowSellItems()
    {
        GameManager.instance.SortItems();
        for (int i = 0; i < sellItemButtons.Length; i++)
        {
            sellItemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "")
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(true);
                sellItemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                sellItemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            }
            else
            {
                sellItemButtons[i].buttonImage.gameObject.SetActive(false);
                sellItemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectBuyItem(Item buyItem)
    {
        if(buyItem != null)
        {
            selectedItem = buyItem;
            buyItemName.text = selectedItem.itemName;
            buyItemDescription.text = selectedItem.description;
            buyItemImage.sprite = selectedItem.itemSprite;
            buyItemValue.text = "<sprite=41>" + selectedItem.value.ToString();
        }
       
    }

    public void SelectSellItem(Item sellItem)
    {
        if(sellItem != null)
        {
            selectedItem = sellItem;
            sellItemName.text = selectedItem.itemName;
            sellItemDescription.text = selectedItem.description;
            sellItemImage.sprite = selectedItem.itemSprite;
            sellItemValue.text = "<sprite=41>" + Mathf.FloorToInt(selectedItem.value * .5f).ToString();
        }
        
    }

    public void BuyItem()
    {
        if (selectedItem != null)
        {
            if (GameManager.instance.currentGold >= selectedItem.value)
            {
                GameManager.instance.currentGold -= selectedItem.value;

                GameManager.instance.AddItem(selectedItem.itemName);
            }
        }

        goldText.text = GameManager.instance.currentGold.ToString() ;
        ResetSelection(true);
    }

    public void SellItem()
    {
        if (selectedItem != null)
        {
            GameManager.instance.currentGold += Mathf.FloorToInt(selectedItem.value * .5f);

            GameManager.instance.RemoveItem(selectedItem.itemName);
            GameManager.instance.SortItems();
            selectedItem = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[0]);
        }

        goldText.text = GameManager.instance.currentGold.ToString() ;

        ShowSellItems();
        ResetSelection(false);
    }

    private void ResetSelection(bool BuyItem)//reset the selected item 
    {
        if(BuyItem)
        {
            SelectBuyItem(GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[0]));
        }
        
        if(!BuyItem)
        {
            SelectSellItem(GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[0]));
        }
        
    }

}
