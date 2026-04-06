using System.Collections.Generic;
using UnityEngine;

public class InventorySystem
{
    //Lista serializable que vive dentro de SaveData
    private List<InventoryItemSaveData> items;

    //Creamos el constructor
    public InventorySystem(List<InventoryItemSaveData> items)
    {
        this.items = items;
    }

    // ─────────────────────────────────────────
    // AÑADIR / ELIMINAR
    // ─────────────────────────────────────────

    //Añade X cantidad de un item, si ya existe en el inventario suma X a la cantidad
    //Inicializamos la quantity que le pasamos a la funcion a 1 ya que al menos se añade esa cantidad al llamarlo
    public void AddItem(string itemID, int quantity = 1)
    {
        //Comprobamos si el item ya existe en el Inventory
        InventoryItemSaveData existing = items.Find(i => i.itemID == itemID);

        //Si ya existe el item en el inventario
        if(existing != null)
        {
            //Sumamos la cantidad
            existing.quantity += quantity;
        }
        //Si no existe en el inventario
        else
        {
            //Creamos una nueva entrada del item
            items.Add(new InventoryItemSaveData{ itemID = itemID, quantity = quantity });
        }

        Debug.Log("Item añadido al inventario: " + itemID + " x" + quantity);
    }

    //Elimina X cantidad de un item, devuelve false si no hay suficiente cantidad
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        //Buscamos el item en el inventario
        InventoryItemSaveData existing = items.Find(i => i.itemID == itemID);

        //Si no existe o no hay suficiente cantidad devolvemos false
        if(existing == null || existing.quantity < quantity)
        {
            Debug.LogWarning("No hay suficiente cantidad de " + itemID + " en el inventario");
            return false;
        }

        //Restamos la cantidad
        existing.quantity -= quantity;

        //Si la cantidad llega a 0 eliminamos la entrada del inventario
        if(existing.quantity <= 0)
        {
            items.Remove(existing);
        }

        return true;
    }

    // ─────────────────────────────────────────
    // CONSULTAS
    // ─────────────────────────────────────────

    //Devuelve la cantidad disponible de un item, 0 si no existe
    public int GetQuantity(string itemID)
    {
        //Buscamos el item en el inventario
        InventoryItemSaveData existing = items.Find(i => i.itemID == itemID);
        //Devuelve la cantidad de existing en caso de que no sea null (existing?), si es null devuelve 0 (??0)
        return existing?.quantity ?? 0;
    }

    //Devuelve true si el inventario tiene al menos la cantidad indicada del item
    public bool HasItem(string itemID, int quantity = 1)
    {
        //Devuelve true si la cantidad del item es mayor o igual a la cantidad que indicamos en la funcion
        return GetQuantity(itemID)>= quantity;
    }

    // ─────────────────────────────────────────
    // MODO DEV
    // ─────────────────────────────────────────

    //Rellena el inventario con x99 de cada item de la base de datos (modo desarrollador)
    public void FillAllItems(ItemDatabase itemDatabase)
    {
        //Recorremos la base de datos de Items y añadimos 99 por cada entrada en la base de datos
        foreach (ItemData item in itemDatabase.allItems)
        {
            AddItem(item.ItemID, 99);
        }
        Debug.Log("Modo dev: inventario rellenado con x99 de cada item");
    }
}
