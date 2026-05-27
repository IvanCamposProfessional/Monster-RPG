using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Panel siempre visible durante el combate que muestra el estado de ambas Essence Pools
public class EssencePoolUI : MonoBehaviour
{
    [Header("Contenedores")]
    //Contenedor donde se instancias las entradas de la pool aliada
    [SerializeField] private Transform allyPoolContainer;
    //Contenedor donde se instancias las entradas de la pool enemiga
    [SerializeField] private Transform enemyPoolContainer;

    [Header("Prefab y Base de Datos")]
    //Prefab de cada entrada de Essence
    [SerializeField] private GameObject essenceEntryPrefab;
    //Base de datos de iconos por tipo
    [SerializeField] private TypeIconDatabase typeIconDatabase;

    //Listas para poder limpiar las entradas al refrescar
    private List<GameObject> allyEntries = new List<GameObject>();
    private List<GameObject> enemyEntries = new List<GameObject>();

    // ─────────────────────────────────────────
    // SUSCRIPCIONES
    // ─────────────────────────────────────────

    private void Awake()
    {
        GameEvents.OnCombatStarted += HandleCombatStarted;
        GameEvents.OnEssencePoolChanged += HandleEssencePoolChanged;
    }

    private void OnDestroy()
    {
        GameEvents.OnCombatStarted -= HandleCombatStarted;
        GameEvents.OnEssencePoolChanged -= HandleEssencePoolChanged;
    }

    // ─────────────────────────────────────────
    // HANDLERS
    // ─────────────────────────────────────────

    //Al arrancar el combate ambas pools estan vacias, limpiamos los contenedores
    private void HandleCombatStarted()
    {
        ClearContainer(allyPoolContainer, allyEntries);
        ClearContainer(enemyPoolContainer, enemyEntries);
    }

    //Refresca el contenedor del bando indicado cuando su pool cambia
    private void HandleEssencePoolChanged(bool isAlly)
    {
        if (isAlly)
        {
            //Guardamos toda la pool del Ally llamando al CombatManager
            Dictionary<MonsterType, int> allyPool = CombatManager.Instance.AllyEssencePool.GetAll();
            //Refrescamos el container del Ally
            RefreshContainer(allyPoolContainer, allyEntries, allyPool);
        }
        else
        {
            //Guardamos toda la pool del Enemy llamando al CombatManager
            Dictionary<MonsterType, int> enemyPool = CombatManager.Instance.EnemyEssencePool.GetAll();
            //Refrescamos el container del Ally
            RefreshContainer(enemyPoolContainer, enemyEntries, enemyPool);
        }
    }

    // ─────────────────────────────────────────
    // DISPLAY
    // ─────────────────────────────────────────

    //Limpia el contenedor y lo repopula con las entradas actuales de la pool
    private void RefreshContainer(Transform container, List<GameObject> entries, Dictionary<MonsterType, int> pool)
    {
        //Limpiamos los containers
        ClearContainer(container, entries);

        //Creamos un bucle que recorre el diccionario de la pool
        foreach(var pair in pool)
        {
            //Creamos el GameObject de la Entry y lo instanciamos en el container
            GameObject entryObj = Instantiate(essenceEntryPrefab, container);

            //Guardamos el icono del tipo del prefab
            Image icon = entryObj.GetComponentInChildren<Image>();

            //Comprobacion de seguridad
            if(icon != null)
            {
                //Guardamos el sprite del tipo buscando en la base de datos de iconos la Key del diccionario (el Type)
                Sprite typeSprite = typeIconDatabase.GetIconByType(pair.Key);

                //Comprobacion de seguridad
                if(typeSprite != null)
                    //Asignamos el TypeSprite al icon
                    icon.sprite = typeSprite;
            }

            //Guardamos el testo de la cantidad de Essence disponible de ese tipo del prefab
            TMP_Text amountText = entryObj.GetComponentInChildren<TMP_Text>();

            //Comprobacion de seguridad
            if(amountText != null)
                //Cambiamos el texto a la quantity de la essence (value del diccionario)
                amountText.text = pair.Value.ToString();

            //Añadimos el gameObject instanciado a la lista de entries
            entries.Add(entryObj);
        }
    }

    //Destruye todos los GameObjects del contenedor y limpia la lista
    private void ClearContainer(Transform container, List<GameObject> entries)
    {
        //Creamos un bucle que recorre las entries
        foreach(var entry in entries)
        {
            //Destruimos el GameObject de la entry
            Destroy(entry);
        }

        //Limpiamos la lista de entries
        entries.Clear();
    }
}
