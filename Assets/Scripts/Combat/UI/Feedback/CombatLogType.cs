using UnityEngine;

//Categoria del mensaje de log de combate, usada por el Combat Feedback Panel para colorear/filtrar cada entrada
public enum CombatLogType
{
    System,     //Inicio/fin de combate, victoria, derrota
    Turn,       //Inicio/fin de turno
    Action,     //Move usado
    Miss,       //Fallo de ataque
    Damage,     //Daño infligido, critico, resistencia, debilidad
    Heal,       //Curacion
    Stat,       //Subida/bajada de stats
    Status,     //Estados alterados: aplicado, tick, intensifica, desaparece
    Timeline,   //Cambios de posicion en la timeline
    KO          //Monstruo derrotado
}
