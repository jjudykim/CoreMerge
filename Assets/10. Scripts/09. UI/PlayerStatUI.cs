using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text critChanceText;
    [SerializeField] private TMP_Text critDamageText;
    [SerializeField] private TMP_Text skillCollDownText;

    private void Start()
    {
        if (Managers.Instance.QuickSlots != null)
            Managers.Instance.QuickSlots.OnChanged += Refresh;
        
        Refresh();
    }

    private void OnEnable()
    {
        if (Managers.Instance.QuickSlots != null)
            Managers.Instance.QuickSlots.OnChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        if (Player.LocalPlayer == null)
            return;

        var p = Player.LocalPlayer;

        int addedAttack = p.FinalAttack - p.PlayerStat.Attack;
        int addedDefense = p.FinalDefense - p.PlayerStat.Defense;

        float addedCritChance = Mathf.Max(0, p.FinalCritChance - p.PlayerStat.CritChance);
    
        float baseCritDamage = p.PlayerStat.CritDamageMultiplier; // ex: 1.0
        float finalCritDamage = p.FinalCritDamageMultiplier;
        float addedCritDamage = Mathf.Max(0, finalCritDamage - baseCritDamage);

        float addedCoolDown = Mathf.Max(0, p.FinalSkillCooldownReduction - p.PlayerStat.SkillCooldownReduction);

        attackText.text = $"공격력 : {p.FinalAttack}" + (addedAttack > 0 ? $" <color=#00BA4A>+{addedAttack}</color>" : "");
        defenseText.text = $"방어 가능 횟수 : {p.FinalDefense}" + (addedDefense > 0 ? $" <color=#00BA4A>+{addedDefense}</color>" : "");
        critChanceText.text = $"치명타 확률 : {p.FinalCritChance * 100f:F1}%" + (addedCritChance > 0 ? $" <color=#00BA4A>+{addedCritChance * 100f:F1}%</color>" : "");
        critDamageText.text = $"치명타 피해 : {finalCritDamage * 100f:F1}%" + (addedCritDamage > 0 ? $" <color=#00BA4A>+{addedCritDamage * 100f:F1}%</color>" : "");
        skillCollDownText.text = $"스킬 쿨다운 감소 : {p.FinalSkillCooldownReduction * 100f:F1}%" + (addedCoolDown > 0 ? $" <color=#00BA4A>+{addedCoolDown * 100f:F1}%</color>" : "");
    }
}
