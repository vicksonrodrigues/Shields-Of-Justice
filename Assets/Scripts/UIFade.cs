using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public static UIFade instance;

    public float fadeSpeed;
    public Image fadeScreen;

    private bool shouldFadeToBlack;
    private bool shouldFadeToClear;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        FadeToBlack();
        FadeToClear();
    }

    public void FadeToBlack()
    {
        if(shouldFadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if(fadeScreen.color.a == 1f)
            {
                shouldFadeToBlack = false;
            }
        }
    }

    public void FadeToClear()
    {
        if (shouldFadeToClear)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 0f)
            {
                shouldFadeToClear = false;
            }
        }
    }

    public void SetFadeToBlack()
    {
        shouldFadeToBlack = true;
        shouldFadeToClear = false;
    }

    public void SetFadeToClear()
    {
        shouldFadeToBlack = false;
        shouldFadeToClear = true;
    }
}
