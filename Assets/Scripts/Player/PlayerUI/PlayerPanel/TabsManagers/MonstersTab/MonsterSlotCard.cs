using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//Componente del prefab de cada slot de monster en la pestaña de Monsters, muestra la info basica o EMPTY si esta el slot vacio
public class MonsterSlotCard : MonoBehaviour, IPointerClickHandler
{
    private Image monsterIcon;
    private Image typeIcon;
    private TMP_Text nameText;
    private TMP_Text levelText;
    private TMP_Text hpText;
    private GameObject emptyLabel;
    private GameObject monsterContent;

    //Monster que representa este slot (null si es vacio)
    private Monster monster;

    //Panel de detalle al que se le pasa el monster al hacer click
    private MonsterDetailPanel detailPanel;

    // ─────────────────────────────────────────
    // SETUP
    // ─────────────────────────────────────────

    //Funcion que se llama desde Monster Tab Manager.Build Slots cuando detecta que si hay un monster en el slot de activeParty
    public void Setup(Monster monster, MonsterDetailPanel detailPanel)
    {
        this.monster = monster;
        this.detailPanel = detailPanel;

        //Resuelve las referencias de los GameObjects privados en el prefab por nombre de hijo
        ResolveReferences();

        //Mostramos el contenido del monster y ocultamos el label de vacio
        if (monsterContent != null) monsterContent.SetActive(true);
        if (emptyLabel != null) emptyLabel.SetActive(false);
        
        //Asignamos los datos del monster a los elementos UI
        if(monsterIcon != null && monster.data.MonsterIcon != null)
            monsterIcon.sprite = monster.data.MonsterIcon;

        if(typeIcon != null)
        {
            Sprite typeSprite = GameManager.Instance.TypeIconDatabase.GetIconByType(monster.data.Type);
            if(typeSprite != null)
                typeIcon.sprite = typeSprite;
        }
        
        if(nameText != null)
            nameText.text = monster.data.MonsterName;
        
        if(levelText != null)
            levelText.text = "LvL. " + monster.level;

        if(hpText != null)
            hpText.text = "HP: " + monster.currentHP + "/" + monster.maxHP;
    }

    //Funcion que se llama desde Monster Tab Manager.Build Slots cuando detecta que no hay un monster en el slot de activeParty
    public void SetupEmpty()
    {
        monster = null;
        detailPanel = null;
 
        ResolveReferences();

        //Ocultamos el contenido del monster y mostramos el label vacio
        if (monsterContent != null) monsterContent.SetActive(false);
        if (emptyLabel != null) emptyLabel.SetActive(true);
    }

    //Funcion que resuelve las referencias de los GameObjects privados en el prefab por nombre de hijo
    private void ResolveReferences(){
        Transform monsterContentTransform = transform.Find("MonsterContent");
        monsterContent = monsterContentTransform != null ? monsterContentTransform.gameObject : null;

        Transform emptyLabelTransform = transform.Find("EmptyLabel");
        emptyLabel = emptyLabelTransform != null ? emptyLabelTransform.gameObject : null;

        if (monsterContent != null)
        {
            monsterIcon = monsterContent.transform.Find("MonsterIcon")?.GetComponent<Image>();
            typeIcon    = monsterContent.transform.Find("TypeIcon")?.GetComponent<Image>();
            nameText    = monsterContent.transform.Find("NameText")?.GetComponent<TMP_Text>();
            levelText   = monsterContent.transform.Find("LevelText")?.GetComponent<TMP_Text>();
            hpText      = monsterContent.transform.Find("HPText")?.GetComponent<TMP_Text>();
        }
    }

    // ─────────────────────────────────────────
    // CLICK
    // ─────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        //Solo abrimos el detalle si el slot tiene un monster
        if(monster == null) return;

        detailPanel.Show(monster);
    }
}
