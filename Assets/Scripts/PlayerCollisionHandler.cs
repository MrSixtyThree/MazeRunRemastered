using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    public bool hasArtifact = false;
    public int lives = 1;
    public float invincibilityDuration = 5f;
    private bool invincible = true;
    [SerializeField] GameObject ui;
    public TextMeshProUGUI HUDArtifact;
    public TextMeshProUGUI HUDLives;
    public UserData activePlayer;



    private void Update()
    {
        
    }

    private void Start()
    {
        activePlayer = GameObject.Find("Menu Controller").GetComponent<UIController>().activePlayer;
        lives = activePlayer.getMaxHealth();
        ui = GameObject.Find("Menu Controller");
        Invoke("endInvincibility", 3);
        HUDLives.text = "";
        for (int i = 0; i < lives; i++)
        {
            HUDLives.text = HUDLives.text + "<3";
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Artifact")) // Collision for picking up artifact
        {
            HUDArtifact.text = "()";
            hasArtifact = true;
            Destroy(other.gameObject);
            
        }
        else if (other.CompareTag("MazeEnd") && hasArtifact) // Collision for finishing maze
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Display UI
            ui.GetComponent<UIController>().EndGameEscape();
            

            gameObject.SetActive(false);
            
        }
        else if (other.CompareTag("Enemy") && !invincible) // Collision for enemy contact and losing life
        {
            
            if (other.GetComponent<EnemyUI>().stunned == false)
            {
                
                invincible = true;
                loseLife();
                
            }
            
            
        }
    }

    private void loseLife()
    {
        
        lives = lives - 1;
        HUDLives.text = "";
        for (int i = 1; i <= lives; i++)
        {
            HUDLives.text = HUDLives.text + "<3";
        }
        Invoke("endInvincibility", invincibilityDuration);
        
        if (lives <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Display UI
            ui.GetComponent<UIController>().EndGameDeath();

            gameObject.SetActive(false);
        }

    }

    private void endInvincibility()
    {
        invincible = false;
    }

}
