using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform lifeRoot;
    [SerializeField] private Image lifeIconPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite emptySprite;

    private readonly List<Image> icons = new();
    private Player boundPlayer;

    private void Awake()
    {
        if (lifeRoot == null)
            return;

        foreach (Transform child in lifeRoot)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
                icons.Add(img);
        }
    }

    public void Bind(Player player)
    {
        if (boundPlayer != null)
            boundPlayer.OnLifeChanged -= OnLifeChanged;

        boundPlayer = player;

        if (boundPlayer == null)
        {
            RefreshIcons(0, icons.Count);
            return;
        }
        
        boundPlayer.OnLifeChanged += OnLifeChanged;
        
        OnLifeChanged(boundPlayer.CurrentHp, boundPlayer.FinalMaxHp);
    }

    private void OnDestroy()
    {
        if (boundPlayer != null)
            boundPlayer.OnLifeChanged -= OnLifeChanged;
    }

    private void OnLifeChanged(int current, int max)
    {
        SyncIconCount(max);
        RefreshIcons(current, max);
    }

    private void SyncIconCount(int max)
    {
        // 부족한 만큼 생성
        while (icons.Count < max)
        {
            Image img = Instantiate(lifeIconPrefab, lifeRoot);
            icons.Add(img);
        }
        
        // 초과하는 만큼 제거
        while (icons.Count > max)
        {
            int last = icons.Count - 1;
            Image img = icons[last];
            icons.RemoveAt(last);
            
            if (img != null)
                Destroy(img.gameObject);
        }
    }

    private void RefreshIcons(int current, int max)
    {
        int activeCount = Mathf.Min(max, icons.Count);
        
        for (int i = 0; i < activeCount; ++i)
        {
            if (i < activeCount)
            {
                icons[i].gameObject.SetActive(true);
                icons[i].sprite = (i < current) ? fullSprite : emptySprite;
            }
            else
            {
                icons[i].gameObject.SetActive(false);
            }
        }
    }
}