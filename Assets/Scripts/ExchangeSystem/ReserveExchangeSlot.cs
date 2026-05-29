using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReserveExchangeSlot : ExchangeSlot
{
    [SerializeField] private Image monsterIconImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject emptyVisual;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject favoriteActiveIcon;
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private GameObject lockButton;
    [SerializeField] private GameObject favoriteButton;

    public override void RefreshVisual()
    {
        MonsterData data = GameManager.Instance.MonsterDatabase.GetMonsterByID(SaveData.monsterID);
        if (data == null) return;

        emptyVisual.SetActive(false);
        monsterIconImage.gameObject.SetActive(true);
        monsterIconImage.sprite = data.MonsterIcon;
        levelText.text = "Lv." + SaveData.level;

        lockedOverlay.SetActive(SaveData.isLocked);
        favoriteActiveIcon.SetActive(SaveData.isFavorite);
        deleteButton.SetActive(!SaveData.isLocked);
        lockButton.SetActive(true);
        favoriteButton.SetActive(true);
    }

    protected override void SetEmpty()
    {
        emptyVisual.SetActive(true);
        monsterIconImage.gameObject.SetActive(false);
        levelText.text = "";
        lockedOverlay.SetActive(false);
        favoriteActiveIcon.SetActive(false);
        deleteButton.SetActive(false);
        lockButton.SetActive(false);
        favoriteButton.SetActive(false);
    }

    //Al pulsar sobre OnLockButtonClicked y OnFavoriteButtonClicked llama a las funciones del ExschangeManager
    public void OnLockButtonClicked()     => ExchangeManager.Instance.ToggleLock(this);
    public void OnFavoriteButtonClicked() => ExchangeManager.Instance.ToggleFavorite(this);
}
