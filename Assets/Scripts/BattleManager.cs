using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{

    public static BattleManager instance;

    private bool battleActive;

    public GameObject battleScene;

    public Transform[] playerPositions;
    public Transform[] enemyPositions;

    public BattleChar[] playerPrefabs;
    public BattleChar[] enemyPrefabs;

    public List<BattleChar> activeBattlers = new List<BattleChar>();

    public int currentTurn;
    public bool turnWaiting;

    public GameObject uiButtonsHolder;

    public BattleMove[] movesList;
    public GameObject enemyAttackEffect;

    public DamageNumber theDamageNumber;

    public Text[] playerName, playerHP, playerMP;

    public GameObject targetMenu;
    public BattleTargetButton[] targetButtons;

    public GameObject magicMenu;
    public BattleMagicSelect[] magicButtons;

    public BattleNotification battleMsg;

    public int chanceToFlee = 25;
    private bool fleeing;

    public string gameOverScene;

    public int rewardXP;
    public string[] rewardItems;

    private bool cannotFlee;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.T)) {
            
            BattleStart(new string[] {"Eyeball", "Spider", "Goblin"});
        }*/

        if (battleActive) {

            if (turnWaiting) {

                if (activeBattlers[currentTurn].isPlayer) {

                    uiButtonsHolder.SetActive(true);
                } else {

                    uiButtonsHolder.SetActive(false);

                    StartCoroutine(EnemyMoveCo());
                }
            }

            if (Input.GetKeyDown(KeyCode.N)) {

                NextTurn();
            }
        }
    }

    public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee)
    {
        if (!battleActive) {

            cannotFlee = setCannotFlee;
            battleActive = true;
            GameManager.instance.battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);

            battleScene.SetActive(true);

            AudioManager.instance.PlayBGM(0);

            for (int i = 0; i < playerPositions.Length; i++) {

                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {

                    for (int j = 0; j < playerPrefabs.Length; j++) {

                        if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName) {

                            BattleChar newPlayer = Instantiate(playerPrefabs[j], playerPositions[i].position, playerPositions[i].rotation);
                            newPlayer.transform.parent = playerPositions[i];
                            activeBattlers.Add(newPlayer);

                            CharStats thePlayer = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHP = thePlayer.currentHP;
                            activeBattlers[i].maxHP = thePlayer.maxHP;
                            activeBattlers[i].currentMP = thePlayer.currentMP;
                            activeBattlers[i].maxMP = thePlayer.maxMP;
                            activeBattlers[i].strength = thePlayer.strength;
                            activeBattlers[i].defence = thePlayer.defence;
                            activeBattlers[i].wpnPower = thePlayer.wpnPwr;
                            activeBattlers[i].armrPower = thePlayer.armrPwr;
                        }
                    }
                }
            }

            for (int i = 0; i < enemiesToSpawn.Length; i++) {

                if (enemiesToSpawn[i] != "") {
                    
                    for (int j = 0; j < enemyPrefabs.Length; j++) {

                        if (enemyPrefabs[j].charName == enemiesToSpawn[i]) {

                            BattleChar newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }

            turnWaiting = true;
            currentTurn = Random.Range(0, activeBattlers.Count);

            UpdateUIStats();
        }
    }

    public void NextTurn()
    {
        currentTurn = (currentTurn + 1) >= activeBattlers.Count ? 0 : currentTurn + 1;

        turnWaiting = true;
        UpdateBattle();
        UpdateUIStats();
    }

    public void UpdateBattle()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++) {

            if (activeBattlers[i].currentHP < 0) {

                activeBattlers[i].currentHP = 0;
            }

            if (activeBattlers[i].currentHP == 0) {

                if (activeBattlers[i].isPlayer) {

                    activeBattlers[i].theSprite.sprite = activeBattlers[i].dead;
                } else {

                    activeBattlers[i].EnemyFade();
                }

            } else {

                if (activeBattlers[i].isPlayer) {

                    allPlayersDead = false;
                    activeBattlers[i].theSprite.sprite = activeBattlers[i].alive;
                } else {

                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead) {

            if (allEnemiesDead) {

                //victory
                StartCoroutine(EndBattleCo());
            } else {

                //lose
                StartCoroutine(GameOverCo());
            }
            
            //battleScene.SetActive(false);
            //GameManager.instance.battleActive = false;
            //battleActive = false;
        } else {
                
            while (activeBattlers[currentTurn].currentHP == 0) {
                
                currentTurn++;
                if (currentTurn >= activeBattlers.Count) {
                    
                    currentTurn = 0;
                }

            }
        }

    }

    public IEnumerator EnemyMoveCo()
    {
        turnWaiting = false;
        yield return new WaitForSeconds(1f);
        EnemyAttack();
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public void EnemyAttack() 
    {
        List<int> players = new List<int>();

        for (int i = 0; i < activeBattlers.Count; i++) {
            
            if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) {

                players.Add(i);
            }
        }

        int selectedTarget = players[Random.Range(0, players.Count)];

        //activeBattlers[selectedTarget].currentHP -= 30;

        int selectedAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
        int movePower = 0;

        for (int i = 0; i < movesList.Length; i++) {

            if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectedAttack]) {

                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);
    }

    public void DealDamage(int target, int movePower)
    {
        float atkPwr = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].wpnPower;
        float defPwr = activeBattlers[target].defence + activeBattlers[currentTurn].armrPower;

        float damageCalc = (atkPwr / defPwr) * movePower * (Random.Range(.9f, 1.1f));
        int damageToDeal = Mathf.RoundToInt(damageCalc);

        Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc + "(" + damageToDeal + ") damage to " + activeBattlers[target].charName);
        activeBattlers[target].currentHP -= damageToDeal;

        Instantiate(theDamageNumber, activeBattlers[target].transform.position,  activeBattlers[target].transform.rotation).SetDamage(damageToDeal);

        UpdateUIStats();
    }

    public void UpdateUIStats()
    {
        for (int i = 0; i < playerName.Length; i++) {

            if (activeBattlers.Count > 1) {

                if (activeBattlers[i].isPlayer) {
                    
                    BattleChar playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.charName;
                    playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
                } else {

                    playerName[i].gameObject.SetActive(false);
                }

            } else {

                playerName[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayerAttack(string moveName, int selectedTarget)
    {
        int movePower = 0;

        for (int i = 0; i < movesList.Length; i++) {

            if (movesList[i].moveName == moveName) {

                Instantiate(movesList[i].theEffect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = movesList[i].movePower;
            }
        }

        Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);

        DealDamage(selectedTarget, movePower);

        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);

        NextTurn();
    }

    public void OpenTargetMenu(string moveName)
    {
        targetMenu.SetActive(true);

        List<int> Enemies = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++) {

            if (!activeBattlers[i].isPlayer) {

                Enemies.Add(i);
            }
        }

        for (int i = 0; i < targetButtons.Length; i++) {

            if (Enemies.Count > i && activeBattlers[Enemies[i]].currentHP > 0) {

                targetButtons[i].gameObject.SetActive(true);
                targetButtons[i].activeBattlerTarget = Enemies[i];
                targetButtons[i].moveName = moveName;
                targetButtons[i].targetName.text = activeBattlers[Enemies[i]].charName;
            } else {

                targetButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenMagicMenu()
    {
        magicMenu.SetActive(true);

        for (int i = 0; i < magicButtons.Length; i++) {

            if (activeBattlers[currentTurn].movesAvailable.Length > i) {

                magicButtons[i].gameObject.SetActive(true);

                magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
                magicButtons[i].nameText.text = magicButtons[i].spellName;

                for (int j = 0; j < movesList.Length; j++) {

                    if (movesList[j].moveName == magicButtons[i].spellName) {

                        magicButtons[i].spellCost = movesList[j].moveCost;
                        magicButtons[i].costText.text = magicButtons[i].spellCost.ToString();
                    }
                }
            } else {

                magicButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Flee()
    {
        if (cannotFlee) {

            battleMsg.msg.text = "Cannot flee this battle";
            battleMsg.Activate();

        } else {

            int fleeSuccess = Random.Range(0,100);

            if (fleeSuccess < chanceToFlee) {

                //battleActive = false;
                //battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            } else {

                NextTurn();
                battleMsg.msg.text = "Coudn't escape!";
                battleMsg.Activate();
            }
        }
    }

    public IEnumerator EndBattleCo()
    {
        battleActive = false;
        uiButtonsHolder.SetActive(false);
        targetMenu.SetActive(false);
        magicMenu.SetActive(false);

        yield return new WaitForSeconds(.5f);

        UiFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < activeBattlers.Count; i++)
        {
            if(activeBattlers[i].isPlayer)
            {
                for(int j = 0; j < GameManager.instance.playerStats.Length; j++)
                {
                    if(activeBattlers[i].charName == GameManager.instance.playerStats[j].charName)
                    {
                        GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
                    }
                }
            }

            Destroy(activeBattlers[i].gameObject);
        }

        UiFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;
        //GameManager.instance.battleActive = false;

        if(fleeing)
        {
            GameManager.instance.battleActive = false;
            fleeing = false;
        } else {

            BattleRewards.instance.OpenRewardScreen(rewardXP, rewardItems);
        }

        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

        public IEnumerator GameOverCo()
    {
        battleActive = false;
        UiFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}
