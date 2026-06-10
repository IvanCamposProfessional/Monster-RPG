using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Subpanel que muestra un Item como reward, RewardManager lo configura via Setup() pasandole los callbacks de Claim y Skip
public class RewardItemPanel : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button skipButton;
 
    private Action<ItemRewardEntry, GameObject> onClaim;
    private Action<GameObject> onSkip;
    private ItemRewardEntry currentEntry;

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    //Configura el panel con el Item a mostrar y los callbacks de los botones
    public void Setup(ItemRewardEntry entry, Action<ItemRewardEntry, GameObject> claimCallback, Action<GameObject> skipCallback)
    {
        currentEntry = entry;
        onClaim = claimCallback;
        onSkip = skipCallback;
 
        //Rellenamos la UI con los datos del Item
        itemIcon.sprite = entry.Item.ItemSprite;
        itemNameText.text = entry.Item.ItemName;
        itemDescriptionText.text = entry.Item.ItemDescription;
        quantityText.text = "x" + entry.Quantity;
    
        //Registramos los listeners, limpiamos primero para evitar duplicados
        claimButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();
 
        claimButton.onClick.AddListener(OnClaimClicked);
        skipButton.onClick.AddListener(OnSkipClicked);
    }

    // ─────────────────────────────────────────
    // CALLBACKS
    // ─────────────────────────────────────────
 
    private void OnClaimClicked()
    {
        onClaim?.Invoke(currentEntry, gameObject);
    }
 
    private void OnSkipClicked()
    {
        onSkip?.Invoke(gameObject);
    }
}
