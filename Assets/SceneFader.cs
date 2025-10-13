using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;
    [SerializeField] CanvasGroup canvasGroup;
    Player player;
    public float fadeDuration;
    
    public CharacterController playerMovementScript; // Assign this in the Inspector

    void FreezeCharacter()
    {
        playerMovementScript.enabled = false;
    }

    void UnfreezeCharacter()
    {
        playerMovementScript.enabled = true;
    }

    bool isFading;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (canvasGroup == null)
            Debug.LogError("[SceneFader] Missing CanvasGroup.");

        canvasGroup.alpha = 1f; // start black; Start will fade in
    }

    void Start() => StartCoroutine(FadeIn());

    public void FadeToScene(string sceneName)
    {
        if (!isFading) StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        if (!canvasGroup) yield break;
        isFading = true;
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        isFading = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (!canvasGroup) { SceneManager.LoadScene(sceneName); yield break; }
        isFading = true;
        player.freezecharacter();

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeIn());
        yield return null; // wait one frame

        isFading = false;
        player.unfreezecharacter();
    }
}