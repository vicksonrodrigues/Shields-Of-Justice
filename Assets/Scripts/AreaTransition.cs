using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AreaTransition : MonoBehaviour
{
    public string sceneName;

    public GameObject areaStart;

    public string areaExitName;

    public string areaStartName ;

    public float waitToLoad = 1f;
    private bool shouldLoadAfterFade;

    // Start is called before the first frame update
    void Start()
    {
        if (areaStartName == PlayerController.instance.playerCurrentArea)
        {
            UIFade.instance.SetFadeToClear();
            GameManager.instance.fadinfBetweenAreas = false;
            PlayerController.instance.transform.position = areaStart.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldLoadAfterFade)
        {
            waitToLoad -= Time.deltaTime;
            if(waitToLoad <= 0)
            {
                shouldLoadAfterFade = false;
                SceneManager.LoadScene(sceneName);

            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            
            shouldLoadAfterFade = true;
            GameManager.instance.fadinfBetweenAreas = true;
            UIFade.instance.SetFadeToBlack();
            PlayerController.instance.playerCurrentArea = areaExitName;
        }
    }
}
