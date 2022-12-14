using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText;

    public GameObject charChoiceMenu;
    public Text[] charChoiceNames;

    public static GameMenu instance;
    public Text goldText;

    public string mainMenuGame;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
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

            AudioManager.instance.PlaySFX(5);
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

        goldText.text = GameManager.instance.currentGold.ToString() + "g";
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

        CloseCharChoice();
    }

    public void CloseMenu() 
    {
        ToggleWindow(-1);
        
        for (int i = 0; i < windows.Length; i++) {
            windows[i].SetActive(false);
        }

        theMenu.SetActive(false);

        GameManager.instance.gameMenuOpen = false;

        CloseCharChoice();
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

    public void ShowItems()
    {
        GameManager.instance.SortItems();

        for (int i = 0; i < itemButtons.Length; i++) {

            itemButtons[i].buttonValue = i;

            if (GameManager.instance.itemsHeld[i] != "") {

                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
            } else {

                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text ="";
            }
        }
    }

    public void SelectItem(Item newItem)
    {
        activeItem = newItem;

        if (activeItem.isItem) {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmour) {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;
    }

    public void DiscardItem()
    {
        if (activeItem != null) {
            
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OpenCharChoice() 
    {

        charChoiceMenu.SetActive(true);

        for (int i = 0; i < charChoiceNames.Length; i++) {
            
            charChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            charChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }

    }

    public void CloseCharChoice() 
    {
        charChoiceMenu.SetActive(false);
    }

    public void UseItem(int selectedChar) {

        activeItem.Use(selectedChar);
        CloseCharChoice();
    }

    public void SaveGame()
    {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void PlayButtonSound()
    {

        AudioManager.instance.PlaySFX(4);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(mainMenuGame);

        Destroy(GameManager.instance.gameObject);
        Destroy(PlayerController.instance.gameObject);
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }
}
