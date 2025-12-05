using System;
using System.Collections;
using UnityEngine;
using TMPro; // TMP를 사용할꺼면 TMPro 네임스페이스를 using을 이용해 사용한다고 선언해준다

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float lifeTime = 1.0f;
    
    private TMP_Text text;
    private Color originColor;
    private float currentTime;
    
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void Show(string message, Color color, Vector3 worldPosition)
    {
        text.SetText(message);
        originColor = color;
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
            // 상승 로직
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 컬러 로직 (사라지는 효과는 컬러가 담당함)
            Color nowColor = Color.Lerp(originColor, targetColor, currentTime / lifeTime);
            text.color = nowColor;
            
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}

