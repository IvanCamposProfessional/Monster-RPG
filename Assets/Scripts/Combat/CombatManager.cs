using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.PlasticSCM.Editor.WebApi;

public enum BattleState
{
    BattleStart,
    TimelineUpdate,
    TurnStart,
    PlayerAction,
    EnemyAction,
    TurnEnd,
    Busy,
    BattleWon,
    BattleLost
}

public class CombatManager : MonoBehaviour
{
    //Singleton para que MonsterUnit pueda comunicarse sin referencia en el inspector
    public static CombatManager Instance { get; private set; }

    private BattleState state;
    public List<GameObject> AllySpawnAreas;
    public List<GameObject> EnemySpawnAreas;
    Enemy enemy;
    Player player;
    [SerializeField]
    private GameObject monsterPrefab;
    //Variable para almacenar la unidad a la que le corresponde el turno
    private MonsterUnit currentUnit;
    //Lista para contener todos los monster units del combate
    List<MonsterUnit> allMonsters;
    //Lista para manejar los monsters aliados
    List<MonsterUnit> allyMonsters = new List<MonsterUnit>();   
    //Lista para manejar los monsters enemigos
    List<MonsterUnit> enemyMonsters = new List<MonsterUnit>();  
    //Variable para definir a que velocidad avanzan las unidades en la TimeLine
    [SerializeField] 
    private float timelineSpeed = 20f;

    [Header("TimelineUI")]
    //Variable para guardar el transform del panel de la timeline
    [SerializeField]
    private RectTransform timelinePanel;
    //Variable para guardar el transform del prefab del icon en la timeline
    [SerializeField]
    private Transform iconContainer;
    //Variable para guardar el prefab del icon
    [SerializeField]
    private GameObject timelineIconPrefab;
    //Variable para guardar el icono Highlited de la unit a la que le corresponde el turno
    [SerializeField]
    private TimelineIcon currentHighlightedIcon;

    //Lista para guardar los iconos en la Timeline
    private List<TimelineIcon> timelineIcons = new List<TimelineIcon>();

    [Header("CombatUI")]
    //Variable para guardar el Script del Combat Menu
    [SerializeField]
    private CombatMenu combatMenu;
    //Variable para saber si se ha elegido un movimiento
    [HideInInspector]
    public bool moveChosen = false;
    //Variable para saber que movimiento se ha elegido 
    [HideInInspector]
    public MoveData chosenMove;
    //Variable para saber si la coroutine del player se está ejecutando
    private bool isPlayerActionCoroutineRunning = false;

    [Header("Targeting")]
    //Variable para saber si estamos esperando a que el jugador clicke un target
    private bool isWaitingForTarget = false;
    //Lista de Monster Unit donde se acumulan los targets clickados por el jugador
    private List<MonsterUnit> selectedTargets = new List<MonsterUnit>();

    void Awake()
    {
        //Inicializamos el Singleton
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Guardamos el objeto player en la variable
        player = GameObject.Find("Player").GetComponent<Player>();
        //Guardamos el objeto enemy en la variable
        enemy = GameObject.Find("Enemy").GetComponent<Enemy>();

        //Ponemos el estado del combate en Battle Start
        state = BattleState.BattleStart;
        Debug.Log(state);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BattleState.BattleStart:
                HandleBattleStart();
                break;
            case BattleState.TimelineUpdate:
                HandleTimelineUpdate();
                break;
            case BattleState.TurnStart:
                HandleTurnStart();
                break;
            case BattleState.PlayerAction:
                //Si no se está ejecutando ya la coroutine
                if (!isPlayerActionCoroutineRunning)
                {
                    //Mostramos el panel del Combat Menu
                    combatMenu.gameObject.SetActive(true);
                    //Indicamos que se va a ejecutar
                    isPlayerActionCoroutineRunning = true;
                    //Lanzamos la coroutine
                    StartCoroutine(PlayerActionRoutine());
                }
                break;
            case BattleState.EnemyAction:
                break;
            case BattleState.TurnEnd:
                HandleTurnEnd();
                break;
            case BattleState.Busy:
                break;
            case BattleState.BattleWon:
                break;
            case BattleState.BattleLost:
                break;
        }
    }

    //Funcion para desarrollar el estado BattleStart
    void HandleBattleStart()
    {
        //Hacemos Setup del Battle
        SetupBattle();
        //Inicializamos la timeline según la speed de la unidad
        InitializeTimeline();
        //Hacemos Setup de la UI de la Timeline
        SetupTimelineUI();
        //Ocultamos el Combat Menu
        combatMenu.HideMenu();
        //Ponemos el estado del combate en TimeLineUpdate
        state = BattleState.TimelineUpdate;
        Debug.Log(state);
    }

    //Funcion para desarrollar el estado BattleUpdate
    void HandleTimelineUpdate()
    {
        //Llamamos a la función que hace Update de la logica de la Timeline
        UpdateTimeline();
    }

    //Funcion para desarrollar el estado BattleUpdate
    void HandleTurnStart()
    {
        //Seguridad por si la currentUnit es nula o no está viva
        if(currentUnit == null || !currentUnit.IsAlive)
        {
            //Volvemos al estado de TimelineUpdate
            state = BattleState.TimelineUpdate;
            return;
        }

        //Resaltamos el icono en la timeline de la unidad activa
        /*foreach (var icon in timelineIcons)
        {
            //Especificamos que sea la currentUnit a la que se hace highlight
            icon.SetHighlight(icon.unit == currentUnit);
        }*/

        //Comprobamos si actualmente hay algun icon con Highlight
        if(currentHighlightedIcon != null)
        {
            //Desactivamos el icono que actualmente está resaltado
            currentHighlightedIcon.SetHighlight(false);
        }

        //Recorremos los timeline icons en la escena
        foreach(var icon in timelineIcons)
        {
            //Si la unidad actual del bucle es la que tiene el turno
            if(icon.unit == currentUnit)
            {
                //Almacenamos el icono que está actualmente resaltado y salimos del bucle
                currentHighlightedIcon = icon;
                break;
            }
        }

        //Si el icono actualmente resaltado que hemos almacenado anteriormente no es nulo
        if(currentHighlightedIcon != null)
        {
            //Hacemos que sea visible
            currentHighlightedIcon.SetHighlight(true);
        }

        //Comprobamos si la current Unit es Ally o Enemy y cambiamos al estado de accion correspondiente
        state = currentUnit.IsAlly ? BattleState.PlayerAction : BattleState.EnemyAction;

        Debug.Log(state);

        Debug.Log(currentUnit.IsAlly ? "Turno del Ally " + currentUnit.name : "Turno del Enemy " + currentUnit.name);
    }

    void HandleTurnEnd()
    {
        if(currentUnit != null)
        {
            //Al terminar el turno reseteamos el timeline progress de la unidad que ha hecho el turno
            currentUnit.timelineProgress = 0f;
        }

        //Quitamos el Highlight del icono actual
        if(currentHighlightedIcon != null)
        {
            currentHighlightedIcon.SetHighlight(false);
            currentHighlightedIcon = null;
        }

        //Cambiamos al estado Timeline Update
        state = BattleState.TimelineUpdate;
        Debug.Log(state);
    }

    private void SetupBattle()
    {
        //Hacemos un bucle que recorra los spawn area para instanciar los allys
        for(int i = 0; i < AllySpawnAreas.Count; i++)
        {
            if(player.party[i] == null)
                continue; //Slot de la party vacio, no hacemos nada
                
            //Instanciamos el GameObject del monster
            GameObject monsterInstance = Instantiate(monsterPrefab, AllySpawnAreas[i].transform.position, Quaternion.identity, AllySpawnAreas[i].transform.parent);

            //Guardamos la unit
            MonsterUnit unit = monsterInstance.GetComponent<MonsterUnit>();
            //Indicamos al prefab que monster de la party del player es
            unit.Setup(player.party[i]);
            //Indicamos a la Unit que es Ally
            unit.SetSide(true);
            //Guardamos la unidad en la lista de Monster Ally
            allyMonsters.Add(unit);

            //Cambiamos el nombre al Ally instanciado
            monsterInstance.name = "Ally " + player.party[i].data.MonsterName;
        }   

        //Hacemos un bucle que recorra los spawn area para instanciar los enemys
        for(int i = 0; i < EnemySpawnAreas.Count; i++)
        {
            if(enemy.party[i] == null)
                continue; //Slot de la party vacio, no hacemos nada
                
            //Instanciamos el GameObject del monster
            GameObject monsterInstance = Instantiate(monsterPrefab, EnemySpawnAreas[i].transform.position, Quaternion.identity, EnemySpawnAreas[i].transform.parent);

            //Guardamos la unit
            MonsterUnit unit = monsterInstance.GetComponent<MonsterUnit>();
            //Indicamos al prefab que monster de la party del enemy es
            unit.Setup(enemy.party[i]);
            //Indicamos a la Unit que es Enemy
            unit.SetSide(false);
            //Guardamos la unidad en la lista de Monster Ally
            enemyMonsters.Add(unit);

            //Cambiamos el nombre al Ally instanciado
            monsterInstance.name = "Enemy " + enemy.party[i].data.MonsterName;
        }

        //Inicializamos la lista All Monsters
        allMonsters = new List<MonsterUnit>();
        //Añadimos todos los monster units a la lista
        allMonsters.AddRange(allyMonsters);
        allMonsters.AddRange(enemyMonsters);

        //CalculateTurnQueue();
    }

    void InitializeTimeline()
    {
        //Si no hay unidades en combate termina la funcion
        if (allMonsters == null || allMonsters.Count == 0)
        {
            return;
        }

        //Obtener la speed maxima entre los monsters
        float maxSpeed = allMonsters.Max(u => u.monster.currentSpeed);

        //Por cada unidad en combate
        foreach(var unit in allMonsters)
        {
            //Normalizamos la velocidad
            float normalizedSpeed = unit.monster.currentSpeed / maxSpeed;

            // Ajusta 80f si NO quieres que alguien empiece directamente en 100
            unit.timelineProgress = normalizedSpeed * 80f;
        }
    }

    void SetupTimelineUI()
    {
        //Limpiamos la lista de iconos
        timelineIcons.Clear();

        //Hacemos un bucle que recorra todas las monster units
        foreach(var unit in allMonsters)
        {
            //Instanciamos el prefab del icono en la TimeLine
            GameObject obj = Instantiate(timelineIconPrefab, iconContainer);
            //Guardammos que icono es
            TimelineIcon icon = obj.GetComponent<TimelineIcon>();
            //Hacemos setup del icon con la unidad correspondiente
            icon.SetupTimelineIcon(unit);

            //Guardamos el icono en la lista
            timelineIcons.Add(icon);
        }
    }

    //Funcion para avanzar en la timeline cada unidad
    void UpdateTimeline()
    {
        //Como ya ha hecho el turno la current unit la dejamos a null
        currentUnit = null;

        //Vamos a hacer 2 bucles, 1 para la logica del TimelineProgress y otro para actualizar la posicion en la Timeline por el control de frames
        //Ya que antes el primer bucle tenia un return de la funcion y no se actualizaba correctamente la UI de la Timeline
        //Asi que ahora hacemos el return despues de los 2 bucles al final del todo
        //Bucle 1 solo para la logica
        foreach (var unit in allMonsters)
        {   //Si la unidad no está viva
            if (!unit.IsAlive)
            {
                //Pasa a la siguiente unidad en combate
                continue;
            }

            //Hacemos avanzar la unidad en la timeline
            unit.timelineProgress += timelineSpeed * Time.deltaTime;

            //Si la unidad ha llegado al maximo quiere decir que es su turno
            if(unit.timelineProgress >= 100f)
            {
                //Dejamos el TimelineProgress de la unidad que ha llegado sin decimales
                unit.timelineProgress = 100f;
                
                //Si Current Unit esta vacio
                if(currentUnit == null)
                {
                    //Asignamos Current Unit a la unidad a la que le correspondera el turno
                    currentUnit = unit;
                }
            }
        }

        //Bucle 2 para actualizar la UI de la Timeline
        foreach (var icon in timelineIcons)
        {
            //Movemos el icono en el Width del timelinePanel
            icon.UpdatePosition(timelineIcons);
        }

        //Si ya hemos asignado a que unidad corresponde el siguiente turno cambia de estado
        //Si no se seguira ejecutando esta funcion en cada frame por la maquina de estados
        if(currentUnit != null)
        {
            state = BattleState.TurnStart;
            Debug.Log(state);
        }
    }

    //Funcion llamada desde MonsterUnit.OnPointerClick y solo procesa el click si estamos esperando un target, añade las unidades clickadas a selected targets
    public void OnUnitClicked(MonsterUnit unit)
    {
        //Si no estamos esperando recibir ningun target por el click del Player
        if (!isWaitingForTarget)
        {
            //Salimos de la funcion
            return;
        }

        //Si ya hemos seleccionado anteriormente la unidad clickada, para evitar duplicados
        if (selectedTargets.Contains(unit))
        {
            //Salimos de la funcion
            return;
        }

        //Añadimos la unidad Clickada que nos pasa Monster Unit a la lista de Selected Targets
        selectedTargets.Add(unit);
    }

    //Funcion para esperar hasta que el jugar clicke exactamente 1 target
    private IEnumerator WaitForSingleTarget()
    {
        //Activamos que el jugador tiene que marcar un target
        isWaitingForTarget = true;
        //Limpiamos la lista de targets seleccionados
        selectedTargets.Clear();

        //Mientras que los targets seleccionados sean menor que 1 devolvemos null y hacemos que la coroutine espere antes de cambiar isWaitingForTarget
        while(selectedTargets.Count < 1)
        {
            yield return null;
        }

        //Una vez ya se haya seleccionado 1 target indicamos que el jugador ya ha seleccionado
        isWaitingForTarget = false;
    }

    //Funcion para esperar hasta que el jugar clicke X targets
    private IEnumerator WaitForMultipleTargets(int count)
    {
        //Activamos que el jugador tiene que marcar un target
        isWaitingForTarget = true;
        //Limpiamos la lista de targets seleccionados
        selectedTargets.Clear();

        //Mientras que los targets seleccionados sean menor que X devolvemos null y hacemos que la coroutine espere antes de cambiar isWaitingForTarget
        while(selectedTargets.Count < count)
        {
            yield return null;
        }

        //Una vez ya se haya seleccionado 1 target indicamos que el jugador ya ha seleccionado
        isWaitingForTarget = false;
    }

    //Construimos una lista de Targets segun el TargetType del move
    private List<MonsterUnit> ResolveTargets(TargetType targetType)
    {
        switch (targetType)
        {
            //Estos casos ya tienen selectedTargets rellena por las coroutines de espera
            //Esto lo que hace es inicializar la lista con los selectedTargets que se rellenan entre las funciones OnUnitClicked y WaitForXTarget
            case TargetType.SingleEnemy:
            case TargetType.SigleAlly:
            case TargetType.MultipleEnemies:
            case TargetType.MultipleAllies:
                return new List<MonsterUnit>(selectedTargets);
            //En los casos de AllEnemies y AllAllies se inicializan con la lista de AllEnemy o AllAllies con las unidades que siguen vivas en escena
            case TargetType.AllEnemies:
                return enemyMonsters.Where(u => u.IsAlive).ToList();
            case TargetType.AllAllies:
                return allyMonsters.Where(u => u.IsAlive).ToList();
            //En el caso de que sea Self se añade Current Unit a la lista de Targets
            case TargetType.Self:
                return new List<MonsterUnit> { currentUnit };
            default:
                return new List<MonsterUnit>();
        }
    }

    //Coroutine para ejecutar la accion del Player
    private IEnumerator PlayerActionRoutine()
    {
        //Por seguridad si no hay current unit o no está viva volvemos al estado Timeline Update
        if (currentUnit == null || !currentUnit.IsAlive)
        {
            state = BattleState.TimelineUpdate;
            yield break;
        }

        //Cambiamos el estado a Busy ya que la coroutine termina de ejecutarse y así evitamos volver a lanzar esta coroutine otra vez
        state = BattleState.Busy;
        Debug.Log(state);

        //Inicializamos action chosen a false y chosen move a null ya que todavia no hemos seleccionado nada
        moveChosen = false;
        chosenMove = null;

        //Mostramos el menu con los ataques del monstruo al cual le corresponde el turno
        combatMenu.SetCurrentUnit(currentUnit);

        //Esperar hasta que el jugador seleccione un Move
        while (!moveChosen)
        {
            yield return null;
        }

        //Una vez el player ha seleccionado un Move
        combatMenu.HideMenu();
        Debug.Log("Movimiento elegido : " + chosenMove.MoveName);

        //Gestionamos el targeting segun el tipo de move
        switch (chosenMove.TargetType)
        {
            //En el caso de que el TargetType del Move elegido sea single se lanza la coroutine que espera a Single Target
            case TargetType.SingleEnemy:
            case TargetType.SigleAlly:
                yield return StartCoroutine(WaitForSingleTarget());
                break;
            //En el caso de que el TargetType del Move elegido sea multiple se lanza la coroutine que espera Multiple Targets
            case TargetType.MultipleEnemies:
            case TargetType.MultipleAllies:
                //Target Count viene del Move Data, lo defines en el inspector
                yield return StartCoroutine(WaitForMultipleTargets(chosenMove.TargetCount));
                break;
            //All Enemies, All Allies y Self no esperan clicks
        }

        //Creas una lista de Targets y la inicializas con la lista ResolveTargets en la que hemos guardado los targets seleccionados dependiendo del target type del chosen move
        List<MonsterUnit> targets = ResolveTargets(chosenMove.TargetType);

        //Ejecutar la coroutine ExecuteMove pasandole la unidad que lo ejecuta, el movimiento elegido y los targets para que pueda hacer el Move Effect correctamente
        yield return StartCoroutine(ExecuteMove(currentUnit, chosenMove, targets));

        //Cambiamos al estado Turn End
        state = BattleState.TurnEnd;
        Debug.Log(state);

        //Terminamos la coroutine
        isPlayerActionCoroutineRunning = false;
    }

    //Coroutine para ejecutar los Move Effects en orden
    private IEnumerator ExecuteMove(MonsterUnit user, MoveData move, List<MonsterUnit> targets)
    {
        //Por cada efecto en el move
        foreach(var effect in move.Effects)
        {
            //Ejecutamos la coroutine del MoveEffect Execute
            yield return StartCoroutine(effect.Execute(user, targets));
        }
    }
}
