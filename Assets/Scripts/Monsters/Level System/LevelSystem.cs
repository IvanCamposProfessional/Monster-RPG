using UnityEngine;

//Clase estatica y pura: gestiona la curva de EXP por tramos (Piecewise Linear Scaling)
public class LevelSystem
{
    public const int MaxLevel = 150;

    //Limites de cada tramo (nivel en el que termina el tramo, inclusive)
    private const int Tramo1End = 50;
    private const int Tramo2End = 100;

    //Devuelve la EXP necesaria para subir del nivel 'level' al 'level + 1'.
    public static int GetExpToNextLevel(int level)
    {
        if (level >= MaxLevel) return 0; // Nivel maximo alcanzado, no hay siguiente nivel

        if (level <= Tramo1End)
            return 600 + 40 * level;
        else if (level <= Tramo2End)
            return 3000 + 60 * level;
        else
            return 6000 + 80 * level;
    }

    //Devuelve el nivel correspondiente a una cantidad de EXP acumulada total
    public static int GetLevelForTotalExp(int totalExp)
    {
        int level = 1;
        int expRemaining = totalExp;

        while (level < MaxLevel)
        {
            int expToNext = GetExpToNextLevel(level);
            if (expRemaining < expToNext) break;

            expRemaining -= expToNext;
            level++;
        }

        return level;
    }

    //Devuelve la EXP acumulada total necesaria para ALCANZAR un nivel concreto
    public static int GetTotalExpForLevel(int level)
    {
        int total = 0;
        for (int l = 1; l < level; l++)
            total += GetExpToNextLevel(l);

        return total;
    }
}
