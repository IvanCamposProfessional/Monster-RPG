using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveData", menuName = "Scriptable Objects/MoveData")]
public class MoveData : ScriptableObject
{
    public string MoveID;
    public string MoveName;
    public string MoveDescription;

    //Define si el Move genera Essence (basic) o la counsume (Essence)
    public MoveActionType ActionType;

    //Define si el daño escala con ataque fisico o especial contra las defensas
    public MoveCategory Category;

    public int Power;

    //Variable Target Type para poder definir que tipo de target utiliza
    public TargetType TargetType;
    //Varibale para almacenar el numero de targets en caso de que el Target Type sea Multiple
    public int TargetCount;

    //Lista de efectos del move en orden de ejecucion
    public List<MoveEffect> Effects;

    //Essence generada al ejecutar, solo se rellena en Basic Moves, debe quedar vacia en Essence Moves
    public List<EssenceAmount> EssenceAmountToPool;
    //Essence consumida al ejecutar, solo se rellena en Essence Moves, debe quedar vacia en Basic Moves
    public List<EssenceAmount> EssenceAmountToUse;

    //Tipo de daño del Move que se va a usar para el calculo de efectividad, en Basic Moves se deriva del tipo enm AmountToPool y en EssenceMoves, 
    // deriva del primer elemento de AmountToUse
    public MonsterType DamageType
    {
        get
        {
            if(ActionType == MoveActionType.Basic)
                return EssenceAmountToPool != null && EssenceAmountToPool.Count > 0 ? EssenceAmountToPool[0].Type : MonsterType.Normal;
            else
                return EssenceAmountToUse != null && EssenceAmountToUse.Count > 0 ? EssenceAmountToUse[0].Type : MonsterType.Normal;
        }
    }

    //Lista de todos los Types del Move
    public List< MonsterType> MoveTypes
    {
        get
        {
            //Guardamos los MoveTypes del Move en una variable
            var source = ActionType == MoveActionType.Basic ? EssenceAmountToPool : EssenceAmountToUse;
            //Si es null devuelve una lista de MoveTypes vacial
            if(source == null) return new List <MonsterType>();
            //Si no es null devolvemos la lista de MoveTypes
            return source.Select(e => e.Type). ToList();
        }
    }
}
