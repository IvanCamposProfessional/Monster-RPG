using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Modo de operacion del RewardManager
public enum RewardManagerMode { Combat, Run }

//Gestiona el flujo de rewards tras BattleWon en Combat Scene, genera los rewards, los muestra uno a uno y carga la RunScene al terminar
public class RewardManager : MonoBehaviour
{
    [Header("Modo")]
    [SerializeField] private RewardManagerMode mode;

    [Header("Referencias — Solo modo Combat")]
    [SerializeField] private EssenceRuneDatabase runeDatabase;
    [SerializeField] private string runSceneName = "RunScene";

    [Header("UI")]
    //Panel raiz del reward, se activa al mostrar un reward y se desactiva al terminar todos
    [SerializeField] private GameObject rewardPanel;
    //Contenedor donde se instancian los paneles de reward dinamicamente
    [SerializeField] private Transform rewardContainer;
    //Prefab del panel de Rune
    [SerializeField] private GameObject runeRewardPanelPrefab;
    //Prefab del panel de Item
    [SerializeField] private GameObject itemRewardPanelPrefab;
    //Sistema puro de generacion de rewards
    private RunRewardSystem rewardSystem;
    //Lista de paneles instanciados actualmente
    private List<GameObject> activePanels = new List<GameObject>();
    //Contador de rewards pendientes de gestionar
    private int pendingCount = 0;

    // ─────────────────────────────────────────
    // CICLO DE VIDA
    // ─────────────────────────────────────────

    private void Awake()
    {
        rewardPanel.SetActive(false);
 
        GameEvents.OnRewardsReady += ShowRewards;
 
        if (mode == RewardManagerMode.Combat)
        {
            GameEvents.OnBattleWon += HandleBattleWon;
            rewardSystem = new RunRewardSystem(runeDatabase);
        }
    }
 
    private void OnDestroy()
    {
        GameEvents.OnRewardsReady -= ShowRewards;
 
        if (mode == RewardManagerMode.Combat)
            GameEvents.OnBattleWon -= HandleBattleWon;
    }

    // ─────────────────────────────────────────
    // MODO COMBAT — GENERACION
    // ─────────────────────────────────────────

    //Se ejecuta al escribir OnBattleWon, inicializa el sistem ay genera los rewards
    private void HandleBattleWon()
    {
        // Comprobacion de seguridad
        if (!RunCombatContext.IsSet)
        {
            Debug.LogWarning("RewardManager: RunCombatContext no esta activo, volviendo a RunScene sin rewards");
            LoadRunScene();
            return;
        }

        //Obtenemos el RunTypeData actual desde RunManager
        RunTypeData runTypeData = RunManager.Instance != null ? RunManager.Instance.RunType : null;

        //Comprobacion de seguridad
        if (runTypeData == null)
        {
            Debug.LogWarning("RewardManager: RunTypeData no disponible, volviendo a RunScene sin rewards");
            LoadRunScene();
            return;
        }

        //Guardamos el Node Type, el Floor Index y el Current Player para generar los Rewards
        NodeType nodeType = RunCombatContext.NodeType;
        int floorIndex = RunCombatContext.FloorIndex;
        PlayerData playerData = GameManager.Instance.CurrentPlayer;

        rewardSystem.GenerateCombatReward(nodeType, floorIndex, runTypeData, playerData);
    }

    // ─────────────────────────────────────────
    // FLUJO PRINCIPAL
    // ─────────────────────────────────────────

    //Recibe el RewardPackage y construye los paneles dinamicamente
    private void ShowRewards(RewardPackage package)
    {
        //Limpiamos los paneles y ponemos la cuenta de rewards a mostrar a 0
        ClearPanels();
        pendingCount = 0;

        //Comprobacion de seguridad
        if (package == null || package.IsEmpty)
        {
            if (mode == RewardManagerMode.Combat) LoadRunScene();
            return;
        }

        //Activamos el panel de rewards
        rewardPanel.SetActive(true);

        //Instanciamos el panel de Rune si hay una
        if (package.Rune != null)
        {
            GameObject obj = Instantiate(runeRewardPanelPrefab, rewardContainer);
            RewardRunePanel runePanel = obj.GetComponent<RewardRunePanel>();
            runePanel.Setup(package.Rune, OnClaimRune, DismissPanel);
            activePanels.Add(obj);

            //Añadimos 1 a la pending count para llevar el recuento de los rewards mostrados
            pendingCount++;
        }

        //Instanciamos un panel por cada Item
        if (package.Items != null)
        {
            //Creamos un bucle que recorra los items en el package
            foreach (ItemRewardEntry entry in package.Items)
            {
                //Comprobacion de seguridad
                if (entry.Item == null) continue;

                GameObject obj = Instantiate(itemRewardPanelPrefab, rewardContainer);
                RewardItemPanel itemPanel = obj.GetComponent<RewardItemPanel>();
                itemPanel.Setup(entry, OnClaimItem, DismissPanel);
                activePanels.Add(obj);

                //Añadimos 1 a la pending count para llevar el recuento de los rewards mostrados
                pendingCount++;
            }
        }

        //Si no se instancio ningun panel cerramos
        if (pendingCount == 0)
        {
            rewardPanel.SetActive(false);
            if (mode == RewardManagerMode.Combat) LoadRunScene();
        }
    }

    // ─────────────────────────────────────────
    // CALLBACKS DE UI
    // ─────────────────────────────────────────

    //El jugador acepta la Rune, se desbloquea y se muestra el siguiente reward
    private void OnClaimRune(EssenceRune rune, GameObject panel)
    {
        GameManager.Instance.CurrentPlayer.unlockedRuneIDs.Add(rune.RuneID);
        GameEvents.RaiseRuneUnlocked(rune.RuneID);
        Debug.Log("RewardManager: Rune desbloqueada — " + rune.RuneID);
        DismissPanel(panel);
    }

    //El jugador acepta el Item y se añade al inventario
    private void OnClaimItem(ItemRewardEntry entry, GameObject panel)
    {
        GameEvents.RaiseItemGranted(entry.Item.ItemID, entry.Quantity);
        DismissPanel(panel);
    }

    //Destruye el panel gestionado y comprueba si quedan rewards pendientes
    private void DismissPanel(GameObject panel)
    {
        activePanels.Remove(panel);
        Destroy(panel);
        pendingCount--;
 
        if (pendingCount <= 0)
        {
            rewardPanel.SetActive(false);
            if (mode == RewardManagerMode.Combat) LoadRunScene();
        }
    }

    // ─────────────────────────────────────────
    // UTILIDADES
    // ─────────────────────────────────────────
 
    private void ClearPanels()
    {
        foreach (GameObject panel in activePanels)
            if (panel != null) Destroy(panel);
 
        activePanels.Clear();
        pendingCount = 0;
    }

    // ─────────────────────────────────────────
    // TRANSICION
    // ─────────────────────────────────────────
 
    private void LoadRunScene()
    {
        rewardPanel.SetActive(false);
        RunCombatContext.ClearResult();
        SceneManager.LoadScene(runSceneName);
    }
}
