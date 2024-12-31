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

    string activePlayerName;
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
        setPointsDisplay();
    }

    public void setPointsDisplay()
    {
        PointsText.text = "TOTAL POINTS: " + activePlayer.getPoints();
    }

    public void ShopBackButton()
    {
        shop.gameObject.SetActive(false);
        difficultyPanel.gameObject.SetActive(true);
        
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
        setPointsDisplay();
        shop_tab1.gameObject.SetActive(true);
        shop_tab2.gameObject.SetActive(false);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
    }
    public void shopPistolUpgrades()
    {
        setPointsDisplay();
        shop_tab1.gameObject.SetActive(false);
        shop_tab2.gameObject.SetActive(true);
        shop_tab3.gameObject.SetActive(false);
        shop_tab4.gameObject.SetActive(false);
    }

    public void shopMGUpgrades()
    {
        setPointsDisplay();
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
        setPointsDisplay();
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
        int pointValue = (int)(((activePlayer.getMovementSpeed() * 5) + 1) * 30);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMovementSpeed(activePlayer.getMovementSpeed() + 0.1f);

            popup("Movement Speed Upgraded!!");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradeEvolveRate()
    {
        int pointValue = (int)(((activePlayer.getEvolveRate() * 5) + 1) * 20);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setEvolveRate(activePlayer.getEvolveRate() + 0.1f);

            popup("Evolve Rate Upgraded!!");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradeMaxHealth()
    {
        int pointValue = (int)activePlayer.getMaxHealth() * 50;
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMaxHealth(activePlayer.getMaxHealth() + 1);

            popup("Max Health Upgraded!!");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradePistolDamage()
    {
        int pointValue = (int)(((activePlayer.getPistolDamage() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolDamage(activePlayer.getPistolDamage() + 0.3f);

            popup("Pistol Damage Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradePistolAccuracy()
    {
        int pointValue = (int)(((activePlayer.getPistolAccuracy() * 5) + 1) * 12);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolAccuracy(activePlayer.getPistolAccuracy() + 0.4f);

            popup("Pistol Accuracy Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradePistolFireRate()
    {
        int pointValue = (int)(((activePlayer.getPistolFireRate() * 5) + 1) * 20);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolFireRate(activePlayer.getPistolFireRate() + 0.3f);

            popup("Pistol Fire Rate Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradePistolMagazineSize()
    {
        int pointValue = (int)(((activePlayer.getPistolMag() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolMag(activePlayer.getPistolMag() + 0.3f);

            popup("Pistol Magazine Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradePistolReload()
    {
        int pointValue = (int)(((activePlayer.getPistolReload() * 5) + 1) * 25);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setPistolReload(activePlayer.getPistolReload() + 0.3f);

            popup("Pistol Reload Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradeMGDamage()
    {
        int pointValue = (int)(((activePlayer.getMGDamage() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGDamage(activePlayer.getMGDamage() + 0.5f);

            popup("MG Damage Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradeMGAccuracy()
    {
        int pointValue = (int)(((activePlayer.getMGAccuracy() * 5) + 1) * 12);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGAccuracy(activePlayer.getMGAccuracy() + 0.3f);

            popup("MG Accuracy Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeMGFireRate()
    {
        int pointValue = (int)(((activePlayer.getMGFireRate() * 5) + 1) * 20);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGFireRate(activePlayer.getMGFireRate() + 0.4f);

            popup("MG Fire Rate Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeMGMagazineSize()
    {
        int pointValue = (int)(((activePlayer.getMGMag() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGMag(activePlayer.getMGMag() + 0.2f);

            popup("MG Magazine Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeMGReload()
    {
        int pointValue = (int)(((activePlayer.getMGReload() * 5) + 1) * 25);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setMGReload(activePlayer.getMGReload() + 0.3f);

            popup("MG Reload Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeShotgunDamage()
    {
        int pointValue = (int)(((activePlayer.getShotgunDamage() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunDamage(activePlayer.getShotgunDamage() + 0.1f);

            popup("Shotgun Damage Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
    public void upgradeShotgunAccuracy()
    {
        int pointValue = (int)(((activePlayer.getShotgunAccuracy() * 5) + 1) * 12);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunAccuracy(activePlayer.getShotgunAccuracy() + 0.2f);

            popup("Shotgun Accuracy Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeShotgunFireRate()
    {
        int pointValue = (int)(((activePlayer.getShotgunFireRate() * 5) + 1) * 20);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunFireRate(activePlayer.getShotgunFireRate() + 0.2f);

            popup("Shotgun Fire Rate Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeShotgunMagazineSize()
    {
        int pointValue = (int)(((activePlayer.getShotgunMag() * 5) + 1) * 15);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunMag(activePlayer.getShotgunMag() + 0.2f);

            popup("Shotgun Magazine Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }

    public void upgradeShotgunReload()
    {
        int pointValue = (int)(((activePlayer.getShotgunReload() * 5) + 1) * 25);
        if (activePlayer.getPoints() >= pointValue)
        {
            activePlayer.setPoints(activePlayer.getPoints() - pointValue);
            activePlayer.setShotgunReload(activePlayer.getShotgunReload() + 0.2f);

            popup("Shotgun Reload Upgraded");
        }
        else
        {
            string output = pointValue + " points needed!";
            popup(output);
        }

        setPointsDisplay();
    }
}