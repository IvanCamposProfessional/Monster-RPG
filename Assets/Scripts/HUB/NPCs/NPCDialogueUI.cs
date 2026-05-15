using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

//Panel de dialogo para las interacciones con NPCs en el HUB, el jugador avanza las lineas haciendo click en cualquier parte del panel
public class NPCDialogueUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;

    //Estado interno
    private List<string> lines;
    private int currentLine;

    // ─────────────────────────────────────────
    // INICIALIZACION
    // ─────────────────────────────────────────

    private void Awake()
    {
        //El panel empieza cerrado
        panel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // FUNCION PUBLICA
    // ─────────────────────────────────────────

    //Abre el panel con el bloque de dialogo correspondiente al estado actual del jugador
    public void Open(NPCData npcData)
    {
        //Comprobacion de seguridad
        if (npcData == null) return;

        //Obtenemos el bloque de dialogo mas avanzado que el jugador puede ver
        lines = npcData.GetCurrrentDialogue(GameManager.Instance.Knowledge);
        currentLine = 0;

        //Aplicamos los datos visuales del NPC
        if (portrait != null)    portrait.sprite  = npcData.portrait;
        if (npcNameText != null) npcNameText.text = npcData.npcName;

        panel.SetActive(true);
        ShowCurrentLine();
    }

    // ─────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────

    //Click en cualquier parte del panel para avanzar el dialogo
    public void OnPointerClick(PointerEventData eventData)
    {
        AdvanceLine();
    }

    // ─────────────────────────────────────────
    // PRIVADOS
    // ─────────────────────────────────────────

    private void ShowCurrentLine()
    {
        //Si no hay lineas cerramos directamente
        if (lines == null || lines.Count == 0) { Close(); return; }
 
        //Cambiamos el texto de la caja de texto a la linea actual
        dialogueText.text = lines[currentLine];
    }

    private void AdvanceLine()
    {
        //Avanzamos en la linea de texto que estamos mostrando
        currentLine++;

        //Si hemos llegado al final cerramos el panel
        if (currentLine >= lines.Count) { Close(); return; }

        ShowCurrentLine();
    }

    private void Close()
    {
        panel.SetActive(false);
    }
}
