using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Startup Settings")]
    // The name of the scene you want to start with.
    public string startingSceneName = "MainScene";
    // Toggle for locking the cursor.
    public bool lockCursor = true;

    void Awake()
    {
        // Ensure that there's only one GameManager and persist it across scenes.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Lock the cursor if the option is enabled.
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Load the starting scene if it's not already loaded.
        if (SceneManager.GetActiveScene().name != startingSceneName)
        {
            SceneManager.LoadScene(startingSceneName);
        }
    }

    // Future game-wide methods (pause, restart, etc.) can be added here.
}
