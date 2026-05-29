using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyExchangeSlot : ExchangeSlot
{
    [SerializeField] private Image monsterIconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private GameObject emptyVisual;
    [SerializeField] private GameObject deleteButton;

    public override void RefreshVisual()
    {
        MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(SaveData.monsterID);
        if (data == null) return;

        emptyVisual.SetActive(false);
        monsterIconImage.gameObject.SetActive(true);
        monsterIconImage.sprite = data.MonsterIcon;
        nameText.text  = data.MonsterName;
        levelText.text = "Lv." + SaveData.level;
        hpText.text    = SaveData.currentHP + " / " + SaveData.maxHP;
        deleteButton.SetActive(true);
    }

    protected override void SetEmpty()
    {
        emptyVisual.SetActive(true);
        monsterIconImage.gameObject.SetActive(false);
        nameText.text  = "";
        levelText.text = "";
        hpText.text    = "";
        deleteButton.SetActive(false);
    }
}
