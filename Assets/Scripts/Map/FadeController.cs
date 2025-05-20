using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
   public static FadeController Instance;

    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeSpeed = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void FadeOutAndMove(System.Action onBlack)
    {
        StartCoroutine(FadeOutCoroutine(onBlack));
    }

    private IEnumerator FadeOutCoroutine(System.Action onBlack)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * _fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);
        onBlack?.Invoke();

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * _fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (_fadeImage != null)
        {
            Color color = _fadeImage.color;
            color.a = Mathf.Clamp01(alpha);
            _fadeImage.color = color;
        }
    }
}
