using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    public string charName;
    public int playerLevel = 1;
    public int currentEXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int baseEXP = 1000;



    public int currentHP;
    public int maxHP = 100;
    public int currentMP;
    public int maxMP =30 ;
    public int[] mpLvlBonus ;
    public int strength;
    public int defence;
    public int weaponPower;
    public int armorPower;

    public string equippedWeapon;
    public string equippedArmor;

    public Sprite charImage;

    // Start is called before the first frame update
    void Start()
    {
        SetExperienceForEachLevel();

    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.K))
        {
            AddEXP(1000);
        }*/
    }

    private void SetExperienceForEachLevel()
    {
        expToNextLevel = new int[maxLevel];
        expToNextLevel[1] = baseEXP;

        for(int i = 2; i< expToNextLevel.Length; i++)
        {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * 1.05f);
        }
    }

    
    public void AddEXP(int expToAdd)
    {
        currentEXP += expToAdd;
        
        if(playerLevel < maxLevel)
        {
            if (currentEXP > expToNextLevel[playerLevel])
            {
                currentEXP -= expToNextLevel[playerLevel];

                playerLevel++;
                BuffChar();
                IncreaseHPLevel();
                IncreaseMPLevel();
            }
        }
        if(playerLevel >= maxLevel)
        {
            currentEXP = 0;
        }
        
    }

    public void BuffChar()
    {
        if(playerLevel%2 == 0)
        {
            strength++;
        }
        else
        {
            defence++;
        }
    }

    public void IncreaseHPLevel()
    {
        maxHP = Mathf.FloorToInt(maxHP * 1.05f);
        currentHP = maxHP;
    }

    public void IncreaseMPLevel()
    {
        if(playerLevel%3 == 0)
        {
            mpLvlBonus[playerLevel] += 13;
        }
        maxMP += mpLvlBonus[playerLevel];
        currentMP = maxMP;
    }
}
