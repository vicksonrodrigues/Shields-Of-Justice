using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{

    private bool canOpen;

    public string[] ItemsForSale = new string[40];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractWithPlayer();
    }


    public void InteractWithPlayer()
    {
        //if (canOpen && Input.GetButtonDown("Fire1") && PlayerController.instance.canMove && !Shop.instance.shopMenu.activeInHierarchy)
        if (canOpen && PlayerController.instance.OnInteractButtonDown() && PlayerController.instance.canMove && !Shop.instance.shopMenu.activeInHierarchy)
        {
            Shop.instance.itemsForSale = ItemsForSale;
            Shop.instance.OpenShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canOpen = true;
            DialogManager.instance.ShowToolTip();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canOpen = false;
            DialogManager.instance.CloseToolTip();
        }
    }
}
