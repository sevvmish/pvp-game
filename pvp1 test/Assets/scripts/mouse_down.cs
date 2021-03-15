using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouse_down : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isStartSpell6, isCheckForSpell6;
    float cur_time, time_normal_drag;
    

    private void Update()
    {

        if (isCheckForSpell6)
        {
            if (time_normal_drag>0.1f)
            {
                //print(playercontrol.AgregateHoriz + " hor    ver- " +playercontrol.AgregateVertic);
                time_normal_drag = 0;
                //cur_time = 0.05f;
                //isStartSpell6 = true;
                if ( Mathf.Abs(playercontrol.AgregateHoriz)>1 || Mathf.Abs(playercontrol.AgregateVertic)>1)
                {
                    cur_time = 0.05f;
                    isStartSpell6 = true;
                    isCheckForSpell6 = false;
                }
            }
            else
            {
                time_normal_drag += Time.deltaTime;
            }
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            playercontrol.ButtonsManagement.Button2Pressed();
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            isCheckForSpell6 = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            isStartSpell6 = false;
            isCheckForSpell6 = false;
        }

        if (cur_time>=general.Tick && isStartSpell6)
        {
            cur_time = 0;
            playercontrol.ButtonsManagement.Button6Pressed();
            isCheckForSpell6 = false;
            isStartSpell6 = false;

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

                    if (!isStartSpell6)
                    {
                        isCheckForSpell6 = true;
                    }
                    //cur_time = 0.05f;
                    //isStartSpell6 = true;
                    break;

            }

            print(eventData.selectedObject.name);
        }
        catch (System.Exception)
        {

            
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isCheckForSpell6)
        {
            isCheckForSpell6 = false;
        }

        if (isStartSpell6)
        {
            isStartSpell6 = false;
        }
    }
}
