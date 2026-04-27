using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] CanvasGroup splashGroup;
    [SerializeField] float initialBlackDuration = 1.5f;
    [SerializeField] float fadeInDuration = 1.2f;
    [SerializeField] float holdDuration = 2.0f;
    [SerializeField] float fadeOutDuration = 1.0f;

    void Start()
    {
        if (splashGroup != null) splashGroup.alpha = 0f;
        StartCoroutine(RunSplash());
    }

    IEnumerator RunSplash()
    {
        yield return new WaitForSeconds(initialBlackDuration);
        yield return FadeCanvasGroup(0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return FadeCanvasGroup(1f, 0f, fadeOutDuration);

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadMainMenuDirectly(); // CORREGIDO
    }

    void Update()
    {
        bool skip = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                    (Pointer.current != null && Pointer.current.press.wasPressedThisFrame);

        if (skip)
        {
            StopAllCoroutines();
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.LoadMainMenuDirectly(); // CORREGIDO
        }
    }

    IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            splashGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        splashGroup.alpha = to;
    }
}