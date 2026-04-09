using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//Gestiona la pestaña de Monsters del Panel del jugador
public class MonstersTabManager : MonoBehaviour
{

    [SerializeField] private GameObject monsterSlotPrefab;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private MonsterDetailPanel detailPanel;

    //Lista de slots instanciados para poder limpiarlos al reconstruir
    private List<MonsterSlotCard> activeSlots = new List<MonsterSlotCard>();

    //Construye todos los slots segun la active party actual del jugador
    public void BuildSlots()
    {
        //Creamos un bucle que recorre los active slots
        foreach(MonsterSlotCard slot in activeSlots)
        {
            //Destruimos los slots actuales
            if(slot != null) Destroy(slot.gameObject);
        }
        //Limpiamos la lista de slots
        activeSlots.Clear();

        //Creamos una lista para la active party del Player y la inicializamos
        List<MonsterSaveData> activeParty = GameManager.Instance.CurrentPlayer.activeParty;

        //Creamos un bucle que tenga tantas iteraciones como MAX_ACTIVE_PARTY
        for(int i = 0; i < PlayerData.MAX_ACTIVE_PARTY; i++)
        {
            //Instanciamos el prefab del slot
            GameObject slotObj = Instantiate(monsterSlotPrefab, slotsContainer);
            //Guardamos el script del slot card
            MonsterSlotCard card = slotObj.GetComponent<MonsterSlotCard>();

            //Si la posicion actual de la party contiene un monster
            if(activeParty[i] != null)
            {
                //Deserializamos el MonsterSaveData a Monster Runtime para obtener sus stats
                Monster monster = MonsterSerializer.Deserialize(activeParty[i],  GameManager.Instance.MonsterDatabase, GameManager.Instance.MoveDatabase);
                //Hacemos setup de la card
                card.Setup(monster, detailPanel);
            }
            //Si la posicion actual de la party no contiene un monster
            else
            {
                card.SetupEmpty();
            }

            //Añadimos la card que hemos creado a la lista de active slots
            activeSlots.Add(card);
        }
    }
}
