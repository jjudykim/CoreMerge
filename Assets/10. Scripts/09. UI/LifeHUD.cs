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
        
        OnLifeChanged(boundPlayer.CurrentHp, boundPlayer.PlayerStat.MaxHp);
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
        while (icons.Count < max)
        {
            Image img = Instantiate(lifeIconPrefab, lifeRoot);
            icons.Add(img);
        }
        
        while (icons.Count > max)
        {
            int last = icons.Count - 1;
            Image img = icons[last];
            icons.RemoveAt(last);
            Destroy(img.gameObject);
        }
    }

    private void RefreshIcons(int current, int max)
    {
        int count = Mathf.Min(max, icons.Count);
        
        for (int i = 0; i < max; ++i)
        {
            bool filled = (i < current);
            icons[i].sprite = filled ? fullSprite : emptySprite;
            icons[i].enabled = true;
        }
        
        for(int i = count; i < icons.Count; ++i)
            icons[i].enabled = false;
    }
}