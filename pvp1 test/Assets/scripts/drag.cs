using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        player_setup.SpellButtonDraged = int.Parse(eventData.selectedObject.name);
        player_setup.isSpellDragedFromSpellBook = true;

        /*
        canvasgroup.blocksRaycasts = false;
        canvasgroup.alpha = 0.5f;
        PrevCoords = this.GetComponent<RectTransform>().anchoredPosition;
        FromWhereIsSpell = this.gameObject.transform.parent;
        
        eventData.pointerDrag.transform.parent = hero.gameObject.transform;
        */
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print(eventData.selectedObject.name + "w here to...");
        //player_setup.SpellButtonDraged = -999;
        player_setup.isSpellDragedFromSpellBook = false;
        player_setup.isEndDragAndDrop = true;
        /*
        print(eventData.selectedObject.name + "w here to...");
        
        canvasgroup.blocksRaycasts = true;
        canvasgroup.alpha = 1f;
        print(eventData.selectedObject.name + " - nname");
        player_setup.isSpellDragedFromSpellBook = false;

        */

    }

    public void OnDrag(PointerEventData eventData)
    {
                
        /*
        rectTransform.anchoredPosition += eventData.delta;
        player_setup.isSpellDragedFromSpellBook = true;
        */
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        print(eventData.selectedObject.name + "where i clicked");
    }
    */
}
