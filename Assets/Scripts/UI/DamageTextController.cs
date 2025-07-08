using TMPro;
using UnityEngine;
using System.Collections;

public class DamageTextController : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private CanvasGroup _canvasGroup;
    private float _duration = 1f;
    private float _fadeSpeed = 2f;
    private Vector3 _moveOffset = new Vector3(0, 1f, 0);

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(int damage, Vector3 worldPos, Color color)
    {
        _text.text = damage.ToString();
        _text.color = color;
        _canvasGroup.alpha = 1f;

        transform.position = Camera.main.WorldToScreenPoint(worldPos);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < _duration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / _duration);
            transform.position = startPos + _moveOffset * (timer / _duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
