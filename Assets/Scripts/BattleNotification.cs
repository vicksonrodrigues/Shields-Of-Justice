using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleNotification : MonoBehaviour
{
    public float awakeTime;
    private float awakeCounter;
    public TextMeshProUGUI theText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (awakeCounter > 0)
        {
            awakeCounter -= Time.deltaTime;
            if (awakeCounter <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        awakeCounter = awakeTime;
    }
}
