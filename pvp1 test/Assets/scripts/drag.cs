using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

    }

    public void OnDrag(PointerEventData eventData)
    {
                
       
    }

    public void OnDrop(PointerEventData eventData)
    {
        /*
        rectTransform.anchoredPosition += eventData.delta;
        */
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        print(eventData.selectedObject.name + "where i clicked");
    }
    */
}
