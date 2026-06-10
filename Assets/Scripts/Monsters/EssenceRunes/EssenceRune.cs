using UnityEngine;

//ScriptableObject que representa una Essence Rune, contiene el MoveData que desbloquea al obtenerla y su rareza
[CreateAssetMenu(fileName = "EssenceRune", menuName = "Scriptable Objects/EssenceRune")]
public class EssenceRune : ScriptableObject
{
    public string RuneID;
    public Sprite RuneIcon;
    //Move que esta Rune desbloquea al equipar en un Monster compatible
    public MoveData MoveData;
    public RarityType Rarity;
    public MonsterType MainType => MoveData != null && MoveData.EssenceAmountToUse != null && MoveData.EssenceAmountToUse.Count > 0 ? MoveData.EssenceAmountToUse[0].Type : default;
}
