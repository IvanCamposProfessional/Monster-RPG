using UnityEngine;

public enum NodeRole
{
    Start, //Node inicial del piso - siempre camp
    Normal, //Node regular - tipo asignado por pesos en runtime
    Boss //Node final del piso - siempre boss
}
