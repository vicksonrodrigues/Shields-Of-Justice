using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public string mainMenuScene;
    public string loadGameScene;
    public GameObject loadButton;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM(0);

        /* PlayerController.instance.gameObject.SetActive(false);
       GameMenu.instance.gameObject.SetActive(false);
        BattleManager.instance.gameObject.SetActive(false); */
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.HasKey("Current_Scene"))
        {
            if (!loadButton.activeInHierarchy)
            {
                loadButton.SetActive(true);
            }

        }
        else
        {
            if (loadButton.activeInHierarchy)
            {
                loadButton.SetActive(false);
            }
        }
    }

    public void QuitToMain()
    {
        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(GameMenu.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(BattleManager.instance.gameObject);

        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLastSave()
    {
        
            Destroy(GameManager.instance.gameObject);
            Destroy(PlayerController.instance.gameObject);
            Destroy(GameMenu.instance.gameObject);
            Destroy(BattleManager.instance.gameObject);

            SceneManager.LoadScene(loadGameScene);

    }
}
