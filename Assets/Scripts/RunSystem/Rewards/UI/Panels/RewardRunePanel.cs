using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Subpanel que muestra una EssenceRune como Reward, RewardManager lo configura via Setup() pasandole los callbacks de Claim y Skip
public class RewardRunePanel : MonoBehaviour
{
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeNameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button skipButton;

    private Action<EssenceRune, GameObject> onClaim;
    private Action<GameObject> onSkip;
    private EssenceRune currentRune;

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    //Configura el panel con la Rune a mostrar y los callbacks de los botones
    public void Setup(EssenceRune rune, Action<EssenceRune, GameObject> claimCallback, Action<GameObject> skipCallback)
    {
        currentRune = rune;
        onClaim = claimCallback;
        onSkip = skipCallback;

        //Rellenamos la UI con los datos de la Rune
        runeIcon.sprite = rune.RuneIcon;
        runeNameText.text = rune.MoveData != null ? rune.MoveData.MoveName : rune.RuneID;
        rarityText.text = rune.Rarity.ToString();

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
        onClaim?.Invoke(currentRune, gameObject);
    }
 
    private void OnSkipClicked()
    {
        onSkip?.Invoke(gameObject);
    }
}
