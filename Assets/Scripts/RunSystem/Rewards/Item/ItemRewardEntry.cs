using UnityEngine;

//Par de ItemData + cantidad dentro de un RewardPackage
public class ItemRewardEntry
{
    public ItemData Item;
    public int Quantity;
 
    public ItemRewardEntry(ItemData item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}
