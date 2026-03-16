using UnityEngine;

public class BossHpBarUI : MonoBehaviour
{
    [SerializeField] private Boss boss;
    [SerializeField] private RectTransform hpBarRect;
    
    [Header("Padding")]
    [SerializeField] private float leftPadding = 10f;
    [SerializeField] private float rightPaddingMax = 10f;

    private float parentWidth;

    private void Awake()
    {
        RectTransform parent = hpBarRect.parent as RectTransform;
        parentWidth = parent.rect.width;
    }

    private void Update()
    {
        float bossHp = boss.Stat.MaxHp > 0 ? (float)boss.Stat.CurrentHp / boss.Stat.MaxHp : 0f;
        SetHPRatio(bossHp);
    }

    public void SetHPRatio(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        
        float right = Mathf.Lerp(parentWidth - leftPadding, rightPaddingMax, ratio);

        hpBarRect.offsetMin = new Vector2(leftPadding, hpBarRect.offsetMin.y);
        hpBarRect.offsetMax = new Vector2(-right, hpBarRect.offsetMax.y);
    }
}
