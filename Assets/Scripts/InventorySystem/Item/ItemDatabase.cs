using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allItems;

    //Funcion que devuelve el item con el ID que le pasamos
    public ItemData GetItemByID(string id)
    {
        return allItems.Find(i => i.ItemID == id);
    }
}
