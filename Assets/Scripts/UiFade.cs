using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFade : MonoBehaviour
{

    public static UiFade instance;

    public Image fadeScreen;
    public Text fadeText;
    public float fadeSpeed;

    public string fadeTextValue;

    private bool shouldFadeToBlack;
    private bool shouldFadeFromBlack;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        fadeText.text = fadeTextValue;
        if(shouldFadeToBlack) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, Mathf.MoveTowards(fadeText.color.a, 1f, fadeSpeed * Time.deltaTime));

            if(fadeScreen.color.a == 1f) {
                shouldFadeToBlack = false;
            }
        }

        if(shouldFadeFromBlack) {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, Mathf.MoveTowards(fadeText.color.a, 0f, fadeSpeed * Time.deltaTime));

            if(fadeScreen.color.a == 0f) {
                shouldFadeFromBlack = false;
            }
        }
    }

    public void FadeToBlack() {

        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;
    }

    public void FadeFromBlack() {

        shouldFadeFromBlack = true;
        shouldFadeToBlack = false;
    }
}
