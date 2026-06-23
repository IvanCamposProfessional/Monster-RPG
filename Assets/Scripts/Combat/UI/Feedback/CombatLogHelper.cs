using System.Collections.Generic;
using UnityEngine;

//Utilidad estatica para emitir mensajes al Combat Feedback Panel via GameEvents.
public static class CombatLogHelper
{
    //Emite un unico mensaje ya formado, sin agrupar (turnos, inicio/fin de combate, KO individual, etc.)
    public static void Raise(string message, CombatLogType type)
    {
        GameEvents.RaiseCombatLogMessage(message, type);
    }

    //Emite una lista de resultados (uno por target) agrupando en una sola linea si hay mas de uno
    public static void RaiseGrouped(List<string> fragments, CombatLogType type)
    {
        //Si no hay nada que loguear (ej. todos los targets esquivaron y se gestiono aparte), no hacemos nada
        if (fragments == null || fragments.Count == 0)
            return;

        //Un unico target: se emite el fragmento tal cual, sin unir
        if (fragments.Count == 1)
        {
            GameEvents.RaiseCombatLogMessage(fragments[0], type);
            return;
        }

        //Varios targets: se agrupan en una sola linea separados por coma
        string grouped = string.Join(", ", fragments);
        GameEvents.RaiseCombatLogMessage(grouped, type);
    }
}
