using UnityEngine;

//Clase estatica y pura: calcula el valor de cualquier stat de un Monster segun su nivel.
public class StatCalculator
{
    //Tasas de crecimiento fijas, definidas en el documento de diseño de Stats.
    public const int HpGrowthRate = 16;
    public const int CombatStatGrowthRate = 8; //ATK, DEF, SpATK, SpDEF
    public const int SpeedGrowthRate = 4;
    public const float EvasionGrowthRate = 0.5f;

    //Formula general para todas las stats de crecimiento entero: valor = base + growth * (nivel - 1)
    public static int CalculateStat(int baseValue, int level, int growthRate)
    {
        return baseValue + growthRate * (level - 1);
    }

    //Cubre Attack, Defense, SpecialAttack y SpecialDefense: las 4 comparten formula y growth rate, asi que una sola funcion evita que se desincronicen si el growth rate cambia en el futuro.
    public static int CalculateCombatStat(int baseValue, int level)
    {
        return CalculateStat(baseValue, level, CombatStatGrowthRate);
    }

    public static int CalculateHP(int baseHP, int level)
    {
        return CalculateStat(baseHP, level, HpGrowthRate);
    }

    public static int CalculateSpeed(int baseSpeed, int level)
    {
        return CalculateStat(baseSpeed, level, SpeedGrowthRate);
    }

    public static float CalculateEvasion(int baseEvasion, int level)
    {
        return baseEvasion + EvasionGrowthRate * (level - 1);
    }
}
