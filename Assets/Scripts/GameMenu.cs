using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject[] windows;

    private CharStats[] playerStats;

    public Text[] nameText, hpText, mpText, levelText, expText;
    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

    public GameObject[] statusButtons;

    public Text statusName, statusHP, statusMP, statusStr, statusDef, statusEqpdWpn, statusWpn, statusEqpdArmr, statusArmr, statusExp;
    public Image statusImage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) {
            if (theMenu.activeInHierarchy) {

                CloseMenu();
            } else {
                theMenu.SetActive(true);
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }
        }
    }

    public void UpdateMainStats()
    {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++) {

            if (playerStats[i].gameObject.activeInHierarchy) {
                charStatHolder[i].SetActive(true);

                nameText[i].text = playerStats[i].charName;
                hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
                mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
                levelText[i].text = "Level: " + playerStats[i].playerLevel;
                expText[i].text = playerStats[i].currentExp.ToString() + "/" + playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel];
                expSlider[i].value = playerStats[i].currentExp;
                charImage[i].sprite = playerStats[i].CharImage;

            } else {
                charStatHolder[i].SetActive(false);
            }
        }
    } 

    public void ToggleWindow(int windowNumber)
    {

        UpdateMainStats();

        for (int i = 0; i < windows.Length; i++) {
            if (i == windowNumber) {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            } else {
                windows[i].SetActive(false);
            }
        }
    }

    public void CloseMenu() 
    {
        ToggleWindow(-1);
        theMenu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;
    }

    public void OpenStatus()
    {
        UpdateMainStats();
        //update the information that is shown

        StatusChar(0);

        for (int i = 0; i < statusButtons.Length; i++) {

            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
            statusButtons[i].GetComponentInChildren<Text>().text = playerStats[i].charName;
        }
    }

    public void StatusChar(int selected)
    {
        statusName.text = playerStats[selected].charName;
        statusHP.text = "" + playerStats[selected].currentHP + "/" + playerStats[selected].maxHP;
        statusMP.text = "" + playerStats[selected].currentMP + "/" + playerStats[selected].maxMP;
        statusStr.text = playerStats[selected].strength.ToString();
        statusDef.text = playerStats[selected].defence.ToString();
        statusEqpdWpn.text = playerStats[selected].equippedWpn != "" ? playerStats[selected].equippedWpn : "None";
        statusWpn.text = playerStats[selected].wpnPwr.ToString();
        statusEqpdArmr.text = playerStats[selected].equippedArmr != "" ? playerStats[selected].equippedArmr : "None";
        statusArmr.text = playerStats[selected].armrPwr.ToString();
        statusExp.text = (playerStats[selected].expToNextLevel[playerStats[selected].playerLevel] - playerStats[selected].currentExp).ToString();
        statusImage.sprite = playerStats[selected].CharImage;
    }
}
