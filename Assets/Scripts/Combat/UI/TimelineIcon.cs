using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimelineIcon : MonoBehaviour
{
    public MonsterUnit unit;
    public Image IconImage;
    public Image Highlight;
    private RectTransform iconRt, containerRt;

    private void Awake()
    {
        //Al despertar el script guardamos el rect transform del objeto
        iconRt = GetComponent<RectTransform>();
        //Guardamos el RectTransform del Timeline Panel
        containerRt = transform.parent as RectTransform;
        //Deshabilitamos la imagen de Highlight al despertar
        SetHighlight(false);
    }

    //Creamos una funcion Setap en la que le decimos que MonsterUnit es y cambiamos de imagen el icon al icono de la Unit
    public void SetupTimelineIcon(MonsterUnit monsterUnit)
    {
        unit = monsterUnit;
        IconImage.sprite = unit.monster.data.MonsterIcon; 
    }
    
    //Creamos una funcion para mover la posicion del prefab
    public void UpdatePosition(List<TimelineIcon> allIcons)
    {   
        //Hacemos clamp para evitar el timeline progress 100.0024 (que pase de 100)
        unit.timelineProgress = Mathf.Clamp(unit.timelineProgress, 0f, 100f);

        //Normalizamos (0-1) el Timeline Progress
        float normalized = unit.timelineProgress / 100f;

        //Guardamos las medidas del panel y del icon
        float containerlWidth = containerRt.rect.width;
        float iconWidth = iconRt.rect.width;

        //Calculamos la posicion usando Lerp
        float minX = iconWidth * 0.5f;
        float maxX = containerlWidth - iconWidth * 0.5f;
        float x = Mathf.Lerp(minX, maxX, normalized);

        //Creamos una variable para guardar la posicion Y del icono y añadimos un condicional en el que si es Ally lo pone debajo de la timeline (-30f) y si es enemy
        //encima de la timeline (30f)
        float baseY = unit.IsAlly ? -30f : 30f;

        //Contador para saber cuantos iconos se superponen entre ellos
        int overlapIndex = 0;
        
        //Recorremos la lista de AllIcons
        foreach (var icon in allIcons)
        {
            //Solo queremos comprobar los iconos anteriores al actual por lo que cuando lleguemos al actual cortamos el bucle
            if(icon == this) break;

            //Normalizamos entre 0 y 1 el timelineprogress del icon actual del bucle
            float normalizedIcon = icon.unit.timelineProgress / 100f;
            //Calculamos el eje X del icon actual del bucle 
            float otherX = Mathf.Lerp(minX, maxX, normalizedIcon);

            //Si estan muy cerca en el eje X los iconos (ponemos una tolerancia de 5 pixdeles)
            if(Mathf.Abs(otherX - x) < 5f)
            {
                //Sumamos uno al overlapIndex
                overlapIndex++;
            }
        }

        //Creamos la separacion horizontal que queremos utilizar y se la añadimos a la X (En caso de no haber aumentado el overlapIndex sera 0
        float spacing = 30f;
        float finalX = x - (overlapIndex * spacing);

        //Si el Timeline Progress de una Unit es 0, porque acaba de realizar su turno, colocamos el icono al inicio de la Timeline (minX)
        /*if(unit.timelineProgress == 0f)
        {
            finalX = minX;
        }*/

        //Clamp final por seguridad
        finalX = Mathf.Clamp(finalX, minX, maxX);

        //Movemos el icon
        iconRt.anchoredPosition = new Vector2(finalX, baseY);
    }

    //Creamos una funcion para activar el resaltado del icono
    public void SetHighlight(bool active)
    {
        Highlight.enabled = active;
    }
}
