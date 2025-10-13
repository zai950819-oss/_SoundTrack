using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public float yposition;
    public string sceneName; //which scene to load
    private BoxCollider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        if (myCollider == null)
        {
            Debug.LogError("[SceneLoader] No BoxCollider2D found. Please add one to this object.");
        }
        else
        {
            myCollider.isTrigger = true; // Make sure it's a trigger
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your player is tagged 'Player'
        {
            Debug.Log($"[SceneLoader] Triggered by {other.name}, loading '{sceneName}'.");
            SceneManager.sceneLoaded += OnSceneLoaded;
            if (SceneFader.Instance != null)
            {
            Debug.Log("[SceneLoader] Fading out to load " + sceneName);
            SceneFader.Instance.FadeToScene(sceneName);
            }
            else
            {
            Debug.LogWarning("[SceneLoader] No SceneFader in scene. Loading directly.");
            SceneManager.LoadScene(sceneName);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 newPos = player.transform.position;
            newPos.y = yposition;
            player.transform.position = newPos;
            Debug.Log($"[SceneLoader] Player Y-position set to {yposition}, X kept at {newPos.x}");
        }
        else
        {
            Debug.LogWarning("[SceneLoader] No player found in the new scene.");
        }

        // Unsubscribe to prevent multiple event triggers
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
