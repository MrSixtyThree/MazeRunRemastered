using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{   
    [SerializeField] GameObject buttonPanel;
    [SerializeField] GameObject difficultyPanel;
    [SerializeField] GameObject saveLoadPanel;
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject playerListPanel;
    List<GameObject> playerListButtons;
    [SerializeField] GameObject newGamePanel;
    [SerializeField] GameObject beginButtonText;
    [SerializeField] GameObject inputNameText;
    [SerializeField] GameObject popupObject;
    [SerializeField] GameObject popupText;
    [SerializeField] MazeGen generator;
    [SerializeField] Canvas mainMenu;
    [SerializeField] Image Man;
    [SerializeField] GameObject SaveController;
    [SerializeField] TMPro.TMP_FontAsset autumn;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject shop;
    [SerializeField] GameObject shop_tab1;
    [SerializeField] GameObject shop_tab2;
    [SerializeField] GameObject shop_tab3;
    [SerializeField] GameObject shop_tab4;
    [SerializeField] GameObject GameOverDeathPanel;
    [SerializeField] GameObject GameOverEscapedPanel;
    [SerializeField] TextMeshProUGUI PointsText;

    // Upgrade Point Cost Text References
    [SerializeField] TextMeshProUGUI upgradeMovementSpeedCostText;
    [SerializeField] TextMeshProUGUI upgradeTotalHealthCostText;
    [SerializeField] TextMeshProUGUI upgradePistolDamageCostText;
    [SerializeField] TextMeshProUGUI upgradePistolFireRateCostText;
    [SerializeField] TextMeshProUGUI upgradePistolAccuracyCostText;
    [SerializeField] TextMeshProUGUI upgradePistolMagazineSizeCostText;
    [SerializeField] TextMeshProUGUI upgradePistolReloadCostText;
    [SerializeField] TextMeshProUGUI upgradeMGDamageCostText;
    [SerializeField] TextMeshProUGUI upgradeMGFireRateCostText;
    [SerializeField] TextMeshProUGUI upgradeMGAccuracyCostText;
    [SerializeField] TextMeshProUGUI upgradeMGMagazineSizeCostText;
    [SerializeField] TextMeshProUGUI upgradeMGReloadCostText;
    [SerializeField] TextMeshProUGUI upgradeShotgunDamageCostText;
    [SerializeField] TextMeshProUGUI upgradeShotgunAccuracyCostText;
    [SerializeField] TextMeshProUGUI upgradeShotgunMagazineSizeCostText;
    [SerializeField] TextMeshProUGUI upgradeShotgunReloadCostText;


    public string activePlayerName;
    public UserData activePlayer;

    public int MazeHeight;
    public int MazeWidth;
    public int enemyCount;
    public int artifactValueMin;
    public int artifactValueMax;
    public int artifactValue;
    public string difficulty;
    public bool popupPresence = false;
    public int interval = 0;

    // Upgrade Variables
    public float growthRate = 1.3f;
    public float baseCostMovementSpeed = 30;
    public float baseCostDamage = 30;
    public float baseCostAccuracy = 30;
    public float baseCostFireRate = 30;
    public float baseCostMagazineSize = 30;
    public float baseCostReload = 30;

    // Limit Counters for Upgrades
    private int upgradeLimit = 2;
    private int movementSpeedCount = 0;
    private int totalHealthCount = 0;
    private int pistolDamageCount = 0;
    private int pistolAccuracyCount = 0;
    private int pistolFireRateCount = 0;
    private int pistolMagazineSizeCount = 0;
    private int pistolReloadCount = 0;
    private int mgDamageCount = 0;
    private int mgAccuracyCount = 0;
    private int mgFireRateCount = 0;
    private int mgMagazineSizeCount = 0;
    private int mgReloadCount = 0;
    private int shotgunDamageCount = 0;
    private int shotgunAccuracyCount = 0;
    private int shotgunMagazineSizeCount = 0;
    private int shotgunReloadCount = 0;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);

        SaveController.GetComponent<SaveAndLoad>().setSaveLocation();
        SaveController.GetComponent<SaveAndLoad>().loadMasterList();
        playerListButtons = new List<GameObject>();

        mainMenu.gameObject.SetActive(true);
        Man.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(true);
        saveLoadPanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false);
        difficultyPanel.gameObject.SetActive(false);
        popupObject.gameObject.SetActive(false);
        shop.gameObject.SetActive(false);
        shop_tab1.gameObject.SetActive(false);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
        GameOverDeathPanel.SetActive(false);
        GameOverEscapedPanel.SetActive(false);

        //Testing Below

        mainMenu.gameObject.SetActive(true);
        shop.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false); // Changed to false

        //TEST();
        //setEasy();
        //setMedium();
        //setHard();
        //setDeadly();
        //setImpossible();

        //StartGame();
    }

    private void Update()
    {
        if (popupPresence)
        {
            interval++;
            if (interval > 500)
            {
                popupPresence = false;
                popupObject.gameObject.SetActive(false);
                interval = 0;
            }
        }
    }
    public void ExitGameButton()
    {
        #if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void playButton()
    {
        buttonPanel.gameObject.SetActive(false);
        saveLoadPanel.gameObject.SetActive(true);
    }

    public void NewGame()
    {
        saveLoadPanel.gameObject.SetActive(false);
        Man.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(true);

    }

    public void LoadGame()
    {
        saveLoadPanel.gameObject.SetActive(false);
        Man.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(true);

        int count = 0;
        
        List<string> playerList = SaveController.GetComponent<SaveAndLoad>().masterList.getPlayerList();
        playerListPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 75 * playerList.Count);
        foreach (string player in playerList)
        {
            int temp = count++;
            GameObject playerName = new GameObject();
            playerName.AddComponent<TextMeshProUGUI>();
            playerName.GetComponent<TextMeshProUGUI>().text = player;
            playerName.GetComponent<TextMeshProUGUI>().font = autumn;
            playerName.GetComponent<TextMeshProUGUI>().fontSize = 72;
            playerName.GetComponent<TextMeshProUGUI>().fontStyle  = TMPro.FontStyles.Bold;
            playerName.GetComponent<TextMeshProUGUI>().rectTransform.sizeDelta = new Vector2(600, 75);
            playerName.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            playerName.GetComponent<TextMeshProUGUI>().color = new Color(255, 228, 0);

            playerName.AddComponent<Button>();
            playerName.GetComponent<Button>().onClick.AddListener(delegate { setPlayer(player, temp); });

            playerName.transform.SetParent(playerListPanel.transform);

            playerListPanel.GetComponent<RectTransform>().transform.localPosition = new Vector3(0.0f, 0.0f, -4.0f);

            playerListButtons.Add(playerName);
        }
    }

    public void GoBackButton()
    {
        loadPanel.gameObject.SetActive(false);
        saveLoadPanel.gameObject.SetActive(true);
        Man.gameObject.SetActive(true);
    }

    public void ShopButton()
    {
        shop.gameObject.SetActive(true);
        difficultyPanel.gameObject.SetActive(false);
        shop_tab1.gameObject.SetActive(true);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
        updateAllPointsText();
    }

    public void setPointsDisplay()
    {
        PointsText.text = "TOTAL POINTS: " + activePlayer.getPoints();
    }

    public void ShopBackButton()
    {
        shop.gameObject.SetActive(false);
        difficultyPanel.gameObject.SetActive(true);

        // Save player data
        SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        
    }

    public void setPlayer(string name, int index)
    {
        activePlayerName = name;

        foreach(GameObject player in playerListButtons){
            player.GetComponent<TextMeshProUGUI>().color = new Color(255, 228, 0);
        }
        playerListButtons.ElementAt(index).GetComponent<TextMeshProUGUI>().color = new Color(255, 228, 0);
        beginButtonText.GetComponent<TextMeshProUGUI>().text = "Let's Go, " + name + "!!!";
    }

    public void LetsGoButton1()
    {
        if(activePlayerName == "" || activePlayerName == null)
        {
            popup("Select A User To Load Before You Start The Game.");
        }
        else
        {
            loadPanel.gameObject.SetActive(false);
            difficultyPanel.gameObject.SetActive(true);

            activePlayer = SaveController.GetComponent<SaveAndLoad>().Load(activePlayerName);
            
        }
    }

    public void letsGoButton2()
    {
        string name = inputNameText.GetComponent<TMP_InputField>().text;
        UserData tempPlayer = SaveController.GetComponent<SaveAndLoad>().Load(activePlayerName);

        if (name == "")
        {
            popup("User Name Cannot be Empty");
        }
        else if(tempPlayer == null)
        {
            newGamePanel.gameObject.SetActive(false);
            difficultyPanel.gameObject.SetActive(true);

            activePlayer = new UserData(name);
            SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        }
        else
        {
            popup("User Name Cannot be " + name + ".");
        }
    }

    public void TEST()
    {
        MazeHeight = 5;
        MazeWidth = 5;
        enemyCount = 3;
        artifactValueMin = 1;
        artifactValueMax = 100;
        difficulty = "TEST";
    }

    public void setEasy()
    {
        MazeHeight = 5;
        MazeWidth = 5;
        enemyCount = 1;
        artifactValueMin = 5;
        artifactValueMax = 13;
        difficulty = "EASY";
    }
    public void setMedium()
    {
        MazeHeight = 7;
        MazeWidth = 7;
        enemyCount = 2;
        artifactValueMin = 10;
        artifactValueMax = 25;
        difficulty = "MEDIUM";
    }
    public void setHard()
    {
        MazeHeight = 9;
        MazeWidth = 9;
        enemyCount = 5;
        artifactValueMin = 18;
        artifactValueMax = 35;
        difficulty = "HARD";
    }
    public void setDeadly()
    {
        MazeHeight = 12;
        MazeWidth = 12;
        enemyCount = 10;
        artifactValueMin = 25;
        artifactValueMax = 60;
        difficulty = "DEADLY";
    }
    public void setImpossible()
    {
        MazeHeight = 20;
        MazeWidth = 20;
        enemyCount = 20;
        artifactValueMin = 32;
        artifactValueMax = 100;
        difficulty = "IMPOSSIBLE";
    }

    public void setWeaponPistol()
    {
        generator.setWeaponPistol();
    }

    public void setWeaponMachineGun()
    {
        if (activePlayer.isMGUnlocked())
        {
            generator.setWeaponMachineGun();
        }
        else
        {
            string output = "Machine Gun is not unlocked!";
            popup(output);
        }
    }

    public void setWeaponShotgun()
    {
        if (activePlayer.isShotgunUnlocked())
        {
            generator.setWeaponShotgun();
        }
        else
        {
            string output = "Shotgun is not unlocked!";
            popup(output);
        }
    }


    public void StartGame()
    {
        artifactValue = Random.Range(artifactValueMin, artifactValueMax);
        generator.clearMaze();
        generator.generateMaze(MazeHeight, MazeWidth, enemyCount, artifactValue, difficulty);

        for(int i = 0; i < enemyCount; i++)
        {
            generator.addEnemy();
        }

        // Maze End Trigger
        GameObject mazeEnd = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mazeEnd.transform.position = new Vector3(0f, 1f, 0f);
        mazeEnd.transform.localScale = new Vector3(1f, 1f, 1f);
        mazeEnd.GetComponent<BoxCollider>().isTrigger = true;
        mazeEnd.GetComponent<MeshRenderer>().enabled = false;
        mazeEnd.tag = "MazeEnd";
        mazeEnd.layer = 9;
        mazeEnd.gameObject.transform.parent = GameObject.FindWithTag("Maze").transform;

        mainMenu.gameObject.SetActive(false);
 
        HUD = GameObject.Find("PlayerCamera").gameObject.transform.Find("HUD").gameObject; // Gets the players HUD and not the inactive HUD. Cant remove the HUD in scene as it causes difficultypanel to display, idk why
        HUD.gameObject.SetActive(true); 
    }

    void popup(string text)
    {
        popupPresence = true;
        popupText.GetComponent<TextMeshProUGUI>().text = text;
        popupObject.gameObject.SetActive(true);
        interval = 0;
    }

    public void EndGameEscape()
    {
        mainMenu.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(false);
        saveLoadPanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false);
        difficultyPanel.gameObject.SetActive(false);
        popupObject.gameObject.SetActive(false);
        shop.gameObject.SetActive(false);
        shop_tab1.gameObject.SetActive(false);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
        GameOverDeathPanel.SetActive(false);
        GameOverEscapedPanel.SetActive(true);

        // Hide Maze
        generator.hideMaze();

    }

    public void EndGameDeath()
    {
        mainMenu.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(false);
        saveLoadPanel.gameObject.SetActive(false);
        loadPanel.gameObject.SetActive(false);
        newGamePanel.gameObject.SetActive(false);
        HUD.gameObject.SetActive(false);
        difficultyPanel.gameObject.SetActive(false);
        popupObject.gameObject.SetActive(false);
        shop.gameObject.SetActive(false);
        shop_tab1.gameObject.SetActive(false);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
        GameOverEscapedPanel.SetActive(false);
        GameOverDeathPanel.SetActive(true);

        // Hide Maze
        generator.hideMaze();
    }

    public void EscapeSaveReplay()
    {
        activePlayer.setPoints(activePlayer.getPoints() + artifactValue);
        SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        GameOverEscapedPanel.SetActive(false);
        difficultyPanel.gameObject.SetActive(true);
    }
    public void EscapeSaveQuit()
    {
        activePlayer.setPoints(activePlayer.getPoints() + artifactValue);
        SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        GameOverEscapedPanel.SetActive(false);
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void DeathSaveReplay()
    {
        GameOverDeathPanel.SetActive(false);
        SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        difficultyPanel.gameObject.SetActive(true);
    }
    public void DeathSaveQuit()
    {
        SaveController.GetComponent<SaveAndLoad>().Save(activePlayer);
        GameOverDeathPanel.SetActive(false);
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void shopPersonalUpgrades()
    {
        updateAllPointsText();
        shop_tab1.gameObject.SetActive(true);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
    }
    public void shopPistolUpgrades()
    {
        updateAllPointsText();
        shop_tab1.gameObject.SetActive(false);
        shop_tab2.gameObject.SetActive(true);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
    }

    public void shopMGUpgrades()
    {
        updateAllPointsText();
        if (activePlayer.isMGUnlocked())
        {
            shop_tab1.gameObject.SetActive(false);
            shop_tab2.gameObject.SetActive(false);
            shop_tab3.gameObject.SetActive(true);
            shop_tab4.gameObject.SetActive(false);
        }
        else if (activePlayer.getPoints() >= 30)
        {
            activePlayer.setPoints(activePlayer.getPoints() - 30);
            activePlayer.unlockMG();
        }
        else
        {
            string output = 30 + " points needed!";
            popup(output);
        }
    }
    public void shopShotgunUpgrades()
    {
        updateAllPointsText();
        if (activePlayer.isShotgunUnlocked())
        {
            shop_tab1.gameObject.SetActive(false);
            shop_tab2.gameObject.SetActive(false);
            shop_tab3.gameObject.SetActive(false);
            shop_tab4.gameObject.SetActive(true);
        }
        else if (activePlayer.getPoints() >= 50)
        {
            activePlayer.setPoints(activePlayer.getPoints() - 50);
            activePlayer.unlockShotgun();
        }
        else
        {
            string output = 50 + " points needed!";
            popup(output);
        }
    }
    public void upgradeMovementSpeed()
    {
        if (movementSpeedCount > upgradeLimit) // Limit to 3 upgrades
        {
            popup("Upgrade already maxed out!");
            return;
        }

        int pointValue = findUpgradeCost("upgradeMovementSpeed");

        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMovementSpeed(activePlayer.getMovementSpeed() + 0.1f);

            popup("Movement Speed Upgraded!!");

            movementSpeedCount++;

        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        // Update UI
        updateAllPointsText();


    }


    public void upgradeMaxHealth()
    {
        int pointValue = findUpgradeCost("upgradeMaxHealth");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMaxHealth(activePlayer.getMaxHealth() + 1);

            popup("Max Health Upgraded!!");

            totalHealthCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        // Update UI
        updateAllPointsText();
    }

    public void upgradePistolDamage()
    {
        int pointValue = findUpgradeCost("upgradePistolDamage");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolDamage(activePlayer.getPistolDamage() + 0.5f);

            popup("Pistol Damage Upgraded");

            pistolDamageCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }
    public void upgradePistolAccuracy()
    {
        int pointValue = findUpgradeCost("upgradePistolAccuracy");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolAccuracy(activePlayer.getPistolAccuracy() + 0.5f);

            popup("Pistol Accuracy Upgraded");

            pistolAccuracyCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradePistolFireRate()
    {
        int pointValue = findUpgradeCost("upgradePistolFireRate");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolFireRate(activePlayer.getPistolFireRate() + 0.4f);

            popup("Pistol Fire Rate Upgraded");

            pistolFireRateCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradePistolMagazineSize()
    {
        int pointValue = findUpgradeCost("upgradePistolMagazineSize");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolMag(activePlayer.getPistolMag() + 0.2f);

            popup("Pistol Magazine Upgraded");

            pistolMagazineSizeCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradePistolReload()
    {
        int pointValue = findUpgradeCost("upgradePistolReload");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolReload(activePlayer.getPistolReload() + 0.4f);

            popup("Pistol Reload Upgraded");

            pistolReloadCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeMGDamage()
    {
        int pointValue = findUpgradeCost("upgradeMGDamage");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGDamage(activePlayer.getMGDamage() + 0.5f);

            popup("MG Damage Upgraded");

            mgDamageCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }
    public void upgradeMGAccuracy()
    {
        int pointValue = findUpgradeCost("upgradeMGAccuracy");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGAccuracy(activePlayer.getMGAccuracy() + 0.3f);

            popup("MG Accuracy Upgraded");

            mgAccuracyCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeMGFireRate()
    {
        int pointValue = findUpgradeCost("upgradeMGFireRate");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGFireRate(activePlayer.getMGFireRate() + 0.3f);

            popup("MG Fire Rate Upgraded");

            mgFireRateCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeMGMagazineSize()
    {
        int pointValue = findUpgradeCost("upgradeMGMagazineSize");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGMag(activePlayer.getMGMag() + 0.2f);

            popup("MG Magazine Upgraded");

            mgMagazineSizeCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeMGReload()
    {
        int pointValue = findUpgradeCost("upgradeMGReload");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGReload(activePlayer.getMGReload() + 0.1f);

            popup("MG Reload Upgraded");

            mgReloadCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeShotgunDamage()
    {
        int pointValue = findUpgradeCost("upgradeShotgunDamage");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunDamage(activePlayer.getShotgunDamage() + 0.2f);

            popup("Shotgun Damage Upgraded");

            shotgunDamageCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }
    public void upgradeShotgunAccuracy()
    {
        int pointValue = findUpgradeCost("upgradeShotgunAccuracy");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunAccuracy(activePlayer.getShotgunAccuracy() + 0.2f);

            popup("Shotgun Accuracy Upgraded");

            shotgunAccuracyCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    // Redundant as fire rate for shotgun cannot be upgraded (always 0)
    public void upgradeShotgunFireRate()
    {
        int pointValue = findUpgradeCost("upgradeShotgunFireRate");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunFireRate(activePlayer.getShotgunFireRate() + 0.0f);

            popup("Shotgun Fire Rate Upgraded");

            shotgunDamageCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeShotgunMagazineSize()
    {
        int pointValue = findUpgradeCost("upgradeShotgunMagazineSize");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunMag(activePlayer.getShotgunMag() + 0.2f);

            popup("Shotgun Magazine Upgraded");

            shotgunMagazineSizeCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    public void upgradeShotgunReload()
    {
        int pointValue = findUpgradeCost("upgradeShotgunReload");
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunReload(activePlayer.getShotgunReload() + 0.4f);

            popup("Shotgun Reload Upgraded");

            shotgunReloadCount++;
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        updateAllPointsText();
    }

    // Function to calculate upgrade cost for upgrades and for updating text
    public int findUpgradeCost(string upgradeName)
    {
        switch (upgradeName)
        {
            // Player Upgrades
            case "upgradeMovementSpeed":
                if (movementSpeedCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostMovementSpeed * Mathf.Pow(activePlayer.getMovementSpeed(), growthRate));
                }
                

            case "upgradeMaxHealth":
                if (totalHealthCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(activePlayer.getMaxHealth() * 40);
                }

            // Pistol Upgrades
            case "upgradePistolDamage":
                if (pistolDamageCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostDamage * Mathf.Pow(activePlayer.getPistolDamage(), growthRate));
                }
            
            case "upgradePistolAccuracy":
                if (pistolAccuracyCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostAccuracy * Mathf.Pow(activePlayer.getPistolAccuracy(), growthRate));
                }
            
            case "upgradePistolFireRate":
                if (pistolFireRateCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostFireRate * Mathf.Pow(activePlayer.getPistolFireRate(), growthRate));
                }

            case "upgradePistolMagazineSize":
                if (pistolMagazineSizeCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostMagazineSize * Mathf.Pow(activePlayer.getPistolMag(), growthRate));
                }

            case "upgradePistolReload":
                if (pistolReloadCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostReload * Mathf.Pow(activePlayer.getPistolReload(), growthRate));
                }

            // Machine Gun Upgrades
            case "upgradeMGDamage":
                if (mgDamageCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostDamage * Mathf.Pow(activePlayer.getMGDamage(), growthRate));
                }            

            case "upgradeMGAccuracy":
                if (mgAccuracyCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostAccuracy * Mathf.Pow(activePlayer.getMGAccuracy(), growthRate));
                }

            case "upgradeMGFireRate":
                if (mgFireRateCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostFireRate * Mathf.Pow(activePlayer.getMGFireRate(), growthRate));
                }
            
            case "upgradeMGMagazineSize":
                if (mgMagazineSizeCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostMagazineSize * Mathf.Pow(activePlayer.getMGMag(), growthRate));
                }

            case "upgradeMGReload":
                if (mgReloadCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostReload * Mathf.Pow(activePlayer.getMGReload(), growthRate));
                }

            // Shotgun Upgrades
            case "upgradeShotgunDamage":
                if (shotgunDamageCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostDamage * Mathf.Pow(activePlayer.getShotgunDamage(), growthRate));
                }
            
            case "upgradeShotgunAccuracy":
                if (shotgunAccuracyCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostAccuracy * Mathf.Pow(activePlayer.getShotgunAccuracy(), growthRate));
                }
            
            case "upgradeShotgunMagazineSize":
                if (shotgunMagazineSizeCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostMagazineSize * Mathf.Pow(activePlayer.getShotgunMag(), growthRate));
                }

            case "upgradeShotgunReload":
                if (shotgunReloadCount > upgradeLimit)
                {
                    return 0;
                }
                else
                {
                    return (int)(baseCostReload * Mathf.Pow(activePlayer.getShotgunReload(), growthRate));
                }

            // Default case for incorrect string
            default:
                return -1;
        }
    }

    // Update All UI Points Text in Shop
    public void updateAllPointsText()
    {
        setPointsDisplay();
        // If upgrade cost is 0, display "Maxed Out"
        upgradeMovementSpeedCostText.text = findUpgradeCost("upgradeMovementSpeed") == 0 ? "N/A" : findUpgradeCost("upgradeMovementSpeed").ToString();
        upgradeTotalHealthCostText.text = findUpgradeCost("upgradeMaxHealth") == 0 ? "N/A" : findUpgradeCost("upgradeMaxHealth").ToString();
        upgradePistolDamageCostText.text = findUpgradeCost("upgradePistolDamage") == 0 ? "N/A" : findUpgradeCost("upgradePistolDamage").ToString();
        upgradePistolAccuracyCostText.text = findUpgradeCost("upgradePistolAccuracy") == 0 ? "N/A" : findUpgradeCost("upgradePistolAccuracy").ToString();
        upgradePistolFireRateCostText.text = findUpgradeCost("upgradePistolFireRate") == 0 ? "N/A" : findUpgradeCost("upgradePistolFireRate").ToString();
        upgradePistolMagazineSizeCostText.text = findUpgradeCost("upgradePistolMagazineSize") == 0 ? "N/A" : findUpgradeCost("upgradePistolMagazineSize").ToString();
        upgradePistolReloadCostText.text = findUpgradeCost("upgradePistolReload") == 0 ? "N/A" : findUpgradeCost("upgradePistolReload").ToString();
        upgradeMGDamageCostText.text = findUpgradeCost("upgradeMGDamage") == 0 ? "N/A" : findUpgradeCost("upgradeMGDamage").ToString();
        upgradeMGAccuracyCostText.text = findUpgradeCost("upgradeMGAccuracy") == 0 ? "N/A" : findUpgradeCost("upgradeMGAccuracy").ToString();
        upgradeMGFireRateCostText.text = findUpgradeCost("upgradeMGFireRate") == 0 ? "N/A" : findUpgradeCost("upgradeMGFireRate").ToString();
        upgradeMGMagazineSizeCostText.text = findUpgradeCost("upgradeMGMagazineSize") == 0 ? "N/A" : findUpgradeCost("upgradeMGMagazineSize").ToString();
        upgradeMGReloadCostText.text = findUpgradeCost("upgradeMGReload") == 0 ? "N/A" : findUpgradeCost("upgradeMGReload").ToString();
        upgradeShotgunDamageCostText.text = findUpgradeCost("upgradeShotgunDamage") == 0 ? "N/A" : findUpgradeCost("upgradeShotgunDamage").ToString();
        upgradeShotgunAccuracyCostText.text = findUpgradeCost("upgradeShotgunAccuracy") == 0 ? "N/A" : findUpgradeCost("upgradeShotgunAccuracy").ToString();
        upgradeShotgunMagazineSizeCostText.text = findUpgradeCost("upgradeShotgunMagazineSize") == 0 ? "N/A" : findUpgradeCost("upgradeShotgunMagazineSize").ToString();
        upgradeShotgunReloadCostText.text = findUpgradeCost("upgradeShotgunReload") == 0 ? "N/A" : findUpgradeCost("upgradeShotgunReload").ToString();
        
    }
}