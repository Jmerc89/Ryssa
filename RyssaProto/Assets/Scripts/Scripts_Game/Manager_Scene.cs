using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // This method will be called when the collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            // Check if the object colliding has the tag "Player"
            if (other.CompareTag("Player"))
            {
                // Call the method to handle the scene transition
                TransitionToNextScene();
            }
        }
        catch (System.Exception ex)
        {
            // Log the exception for debugging purposes
            Debug.LogError($"An error occurred during collision detection: {ex.Message}");
        }
    }

    // Method to handle the scene transition
    private void TransitionToNextScene()
    {
        try
        {
            // Get the current scene index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Calculate the next scene index
            int nextSceneIndex = currentSceneIndex + 1;

            // Check if the next scene index is within the valid range
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                // Load the next scene
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No more scenes to load. Returning to the first scene.");
                SceneManager.LoadScene(0); // Load the first scene if no more scenes are available
            }
        }
        catch (System.Exception ex)
        {
            // Log the exception for debugging purposes
            Debug.LogError($"An error occurred during scene transition: {ex.Message}");
        }
    }
}
