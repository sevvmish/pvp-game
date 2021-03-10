using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouse_down : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isStartSpell6;
    float cur_time;
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    private void Update()
    {

        if (cur_time>=general.Tick && isStartSpell6)
        {
            cur_time = 0;
            playercontrol.ButtonsManagement.Button6Pressed();
        } else
        {
            cur_time += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        try
        {
            switch(eventData.selectedObject.name)
            {
                case "spell1":
                    playercontrol.ButtonsManagement.Button1Pressed();
                    break;
                case "spell2":
                    playercontrol.ButtonsManagement.Button2Pressed();
                    break;
                case "spell3":
                    playercontrol.ButtonsManagement.Button3Pressed();
                    break;
                case "spell4":
                    playercontrol.ButtonsManagement.Button4Pressed();
                    break;
                case "spell5":
                    playercontrol.ButtonsManagement.Button5Pressed();
                    break;
                case "spell6":
                    cur_time = 0.05f;
                    isStartSpell6 = true;
                    break;

            }

            print(eventData.selectedObject.name);
        }
        catch (System.Exception)
        {

            throw;
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isStartSpell6)
        {
            isStartSpell6 = false;
        }
    }
}
