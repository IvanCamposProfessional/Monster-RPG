using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

//Controla el movimiento vertical de la camara en la escena de la run, el jugador arrastra con el boton izquierdo del mouse para desplazarse
public class RunCameraController : MonoBehaviour
{
    [Header("Limites verticales")]
    //Posicion Y minima que puede alcanzar la camara (parte baja del mapa)
    [SerializeField] private float minY = -5f;
    //Posicion Y maxima que puede alcanzar la camara (parte alta del mapa)
    [SerializeField] private float maxY = 10f;

    [Header("Sensibilidad")]
    [SerializeField] private float dragSensitivity = 1f;
    //Pixels que debe moverse el mouse para considerar que es un drag y no un click
    [SerializeField] private float dragThreshold = 10f;

    //Variable para saber si se ha pulsado el mouse
    private bool isPressed = false;
    //Variable privada para saber si se esta drageando la camara en este momento
    private bool isDragging = false;
    private Vector2 pressStartPosition;
    //Variable privada para guardar la ultima posicion del mouse
    private Vector2 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
        HandleDrag();
    }

    private void HandleDrag()
    {
        //Guardamos el mouse
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        //Guardamos la posicion del mouse en cad frame
        Vector2 currentMousePosition = mouse.position.ReadValue();

        //Inicio del drag
        if (mouse.leftButton.wasPressedThisFrame)
        {
            //Guardamos que se esta haciendo drag
            isPressed = true;
            //Guardamos cuando la posicion del Mouse cuando se pulsa el raton y la posicion actual
            pressStartPosition = currentMousePosition;
            lastMousePosition = currentMousePosition;
        }

        //Fin del drag
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isPressed = false;
            isDragging = false;
        }

        //Si se ha pulsado el raton
        if (isPressed)
        {
            //Si no hemos guardado que se este draggeando aun
            if (!isDragging)
            {
                //Guardamos la distancia que se ha movido el raton
                float distanceMoved = Vector2.Distance(currentMousePosition, pressStartPosition);

                //Si la distancia que se ha movido el raton es mayor a la drag threshold que hemos definido guardamos que se esta drageando
                if (distanceMoved > dragThreshold)
                    isDragging = true;
            }

            if (isDragging)
            {
                //Calculamos la posicion recorrida del mouse en el eje y
                float deltaY = currentMousePosition.y - lastMousePosition.y;

                //Convertimos el delta de pixels a unidades de mundo
                float worldDelta = deltaY * dragSensitivity * (Camera.main.orthographicSize * 2f / Screen.height);

                //Creamos la nueva posicion de la camara guardando la posicion actual
                Vector3 newPos = transform.position;
                //Aplicamos al eje y la delta que hemos calculado anteriormente
                newPos.y = Mathf.Clamp(newPos.y - worldDelta, minY, maxY);

                //Movemos la camara a la nueva posicion calculada
                transform.position = newPos;
            }
        }

        //Guardamos la posicion actual del mouse
        lastMousePosition = currentMousePosition;
    }
}
