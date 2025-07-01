using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int totalScore; // Total score for the game
    public int stagePoint; // Current level of the game
    public int stageIndex; // Index of the current stage
    public int health; // Player's health
    public PlayerMove playerMove; // Reference to the PlayerMove script
    public GameObject[] Stages;

    public Image[] UIHealth; // Array of UI images to display health
    public TextMeshProUGUI UIPoint; // UI text to display the score
    public TextMeshProUGUI UIStage; // UI text to display the health
    public GameObject RestartButton; // UI button to restart the game

    void Update()
    {
        UIPoint.text = (stagePoint + totalScore).ToString(); // Update the score UI
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length - 1) // Check if the last stage is reached
        {
            Stages[stageIndex].SetActive(false); // Deactivate the previous stage
            stageIndex++; // Increment the stage index
            Stages[stageIndex].SetActive(true); // Activate the next stage
            PlayerReposition(); // Reset player position for the new stage

            UIStage.text = "STAGE " + (stageIndex + 1); // Update the stage UI text
        }
        else
        {
            Time.timeScale = 0; // Stop the game if the last stage is reached

            TextMeshProUGUI btnText = RestartButton.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Clear!"; // Change the button text to "RESTART"
            RestartButton.SetActive(true); // Show the restart button when the last stage is reached
        }


        totalScore += stagePoint; // Add the current stage point to the total score
        stagePoint = 0; // Reset the stage point for the new stage
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthDown(); // Decrease player's health when colliding with an enemy

            if (health > 0)
            {
                PlayerReposition(); // Reset player position if health is above zero
            }
            else
            {
                playerMove.OnDie(); // Call OnDie method in PlayerMove when health reaches zero
            }
        }
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--; // Decrease player's health when colliding with an enemy
            UIHealth[health].color = new Color(1, 0, 0, 0.4f); // change the health UI image
        }
        else
        {
            UIHealth[0].color = new Color(1, 0, 0, 0.4f); // change the health UI image

            playerMove.OnDie(); // Call OnDie method in PlayerMove when health reaches zero

            RestartButton.SetActive(true); // Show the restart button when health reaches zero
        }
    }

    void PlayerReposition()
    {
        playerMove.transform.position = new Vector3(0, 0, -1); // Reset player position to the start of the stage
        playerMove.VelocityZero(); // Reset player velocity to zero
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game time
        SceneManager.LoadScene(0); // Restart the game by loading the first scene
    }
}
