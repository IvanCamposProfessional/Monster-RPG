using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Panel de detalle de un monster, 
public class MonsterDetailPanel : MonoBehaviour
{
    //Referencias internas del panel de detail    
    private Image monsterIcon;
    private TMP_Text nameText;
    private TMP_Text typeText;
    private TMP_Text levelText;
    private TMP_Text hpText;
    private TMP_Text attackText;
    private TMP_Text defenseText;
    private TMP_Text specialAttackText;
    private TMP_Text specialDefenseText;
    private TMP_Text speedText;

    [SerializeField] private GameObject slotsContainer;

    private void Awake()
    {
        //El panel empieza oculto
        gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // ABRIR / CERRAR
    // ─────────────────────────────────────────

    public void Show(Monster monster)
    {
        slotsContainer.SetActive(false);

        //Resolvemos las referencias de los gameobjects del inspector
        ResolveReferences();

        //Asignamos los datos del monster y los mostramos
        if (monsterIcon != null && monster.data.MonsterIcon != null)
            monsterIcon.sprite = monster.data.MonsterIcon;
 
        if (nameText != null)
            nameText.text = monster.data.MonsterName;
 
        if (typeText != null)
            typeText.text = monster.data.Type.ToString();
 
        if (levelText != null)
            levelText.text = "Lv. " + monster.level;
 
        if (hpText != null)
            hpText.text = "HP: " + monster.currentHP + "/" + monster.maxHP;
 
        if (attackText != null)
            attackText.text = "ATK: " + monster.currentAttack;
 
        if (defenseText != null)
            defenseText.text = "DEF: " + monster.currentDefense;
 
        if (specialAttackText != null)
            specialAttackText.text = "SP.ATK: " + monster.currentSpecialAttack;
 
        if (specialDefenseText != null)
            specialDefenseText.text = "SP.DEF: " + monster.currentSpecialDefense;
 
        if (speedText != null)
            speedText.text = "SPD: " + monster.currentSpeed;
 
        gameObject.SetActive(true);
    }

    public void HideMonsterDetailPanel()
    {
        slotsContainer.SetActive(true);
        gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    //Funcion que resuelve las referencias de los GameObjects privados en el panel por nombre de hijo
    private void ResolveReferences()
    {
        monsterIcon        = transform.Find("MonsterIcon")?.GetComponent<Image>();
        nameText           = transform.Find("NameText")?.GetComponent<TMP_Text>();
        typeText           = transform.Find("TypeText")?.GetComponent<TMP_Text>();
        levelText          = transform.Find("LevelText")?.GetComponent<TMP_Text>();
        hpText             = transform.Find("HPText")?.GetComponent<TMP_Text>();
        attackText         = transform.Find("ATKText")?.GetComponent<TMP_Text>();
        defenseText        = transform.Find("DEFText")?.GetComponent<TMP_Text>();
        specialAttackText  = transform.Find("SPATKText")?.GetComponent<TMP_Text>();
        specialDefenseText = transform.Find("SPDEFText")?.GetComponent<TMP_Text>();
        speedText          = transform.Find("SPDText")?.GetComponent<TMP_Text>();
    }
}
