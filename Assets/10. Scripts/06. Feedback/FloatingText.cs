using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float lifeTime = 1.0f;

    private TMP_Text text;
    private Color originColor;
    private float currentTime;
    private Renderer textRenderer;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        textRenderer = GetComponent<Renderer>();
        textRenderer.sortingOrder = 1000;
    }

    public void Show(string message, Color color, Vector3 worldPosition)
    {
        text.SetText(message);
        originColor = text.color;
        transform.position = worldPosition;
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        currentTime = 0.0f;

        Color targetColor = originColor;
        targetColor.a = 0.0f;

        while (currentTime < lifeTime)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            Color nowColor = Color.Lerp(originColor, targetColor, currentTime / lifeTime);
            text.color = nowColor;

            currentTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}