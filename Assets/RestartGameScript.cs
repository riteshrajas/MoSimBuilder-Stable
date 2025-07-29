using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameScript : MonoBehaviour
{
    private bool canRestart = true;
    private const float restartCooldown = 0.5f; // Prevent spam clicking

    void Update()
    {
        if (!canRestart) return;

        // Check inputs more efficiently
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button6))
        {
            StartCoroutine(RestartWithCooldown());
        }
    }

    private IEnumerator RestartWithCooldown()
    {
        canRestart = false;

        // Optional: Add a brief pause to prevent frame drops
        yield return new WaitForEndOfFrame();

        // Use build index instead of scene name (faster)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Cooldown to prevent spam (though scene will reload anyway)
        yield return new WaitForSeconds(restartCooldown);
        canRestart = true;
    }

    // Public method for UI buttons
    public void RestartGame()
    {
        if (canRestart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}