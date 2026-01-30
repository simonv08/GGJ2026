using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string levelSceneName = "Level";



    /// <summary>
    /// Toggles between Level and Menu based on current scene
    /// </summary>
    public void ToggleScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == levelSceneName)
        {
            LoadScene(menuSceneName);
        }
        else
        {
            LoadScene(levelSceneName);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Load a specific scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("LevelManager: Scene name is null or empty.");
        }
    }
}
