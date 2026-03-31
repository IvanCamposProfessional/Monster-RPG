using UnityEngine;

//Singleton de la HubScene. Gestiona el input global.
public class HubManager : MonoBehaviour
{
    //Creamos la instancia del Manager
    public static HubManager Instance { get; private set; }

    //Guardamos el HubUIManager;
    [SerializeField] private HubUIManager hubUIManager;

    private void Awake()
    {
        //Codigo de seguridad por si hemos duplicado la instancia
        if(Instance != null) { Destroy(gameObject); return; }
        //Inicializamos la instancia
        Instance = this;
    }
}
