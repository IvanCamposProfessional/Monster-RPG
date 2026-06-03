using System.Collections.Generic;
using UnityEngine;

//Paquete completo de reward generado por RunRewardSystem
public class RewardPackage
{
    //Null si el reward no contiene ninguna Rune
    public EssenceRune Rune;
    public List<ItemRewardEntry> Items;

    public RewardPackage()
    {
        Items = new List<ItemRewardEntry>();
    }

    //True si el paquete no contiene ningun reward
    public bool IsEmpty => Rune == null && (Items == null || Items.Count == 0);
}
