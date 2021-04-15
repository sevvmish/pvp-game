using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class player_get_new : MonoBehaviour
{
    private Vector3 WarrPos = new Vector3(0, 0, 0);
    private Vector3 MagePos = new Vector3(-7, 0, 0);
    private Vector3 BarbarPos = new Vector3(-14, 0, 0);
    private Vector3 RogPos = new Vector3(-21, 0, 0);
    private Vector3 WizPos = new Vector3(-28, 0, 0);
    private float cur_time;

    public TMP_InputField char_name_input;
    public Transform PlayerLine;
    public GameObject EnterNamePanel, ConnectionError;
    public Button pl1, pl2, pl3, pl4, pl5, create_char_button, OkOnChoosing, back_button;

    private int CurrentPlayerNumber = 1;
    private bool isBusy;

    public TextMeshProUGUI warrtext, elemtext, barbartext, rogtext, wizardtext, createnewchartext, backtext, enterloginname;

    // Start is called before the first frame update
    void Start()
    {
        sr.isConnectionError = false;

        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        EnterNamePanel.SetActive(false);
        ConnectionError.SetActive(false);
        pl1.onClick.AddListener(Click1);
        pl2.onClick.AddListener(Click2);
        pl3.onClick.AddListener(Click3);
        pl4.onClick.AddListener(Click4);
        pl5.onClick.AddListener(Click5);
        create_char_button.onClick.AddListener(create_char_panel_on);
        OkOnChoosing.onClick.AddListener(OkOnChoose);
        back_button.onClick.AddListener(Back);

        warrtext.text = lang.WarriorText;
        elemtext.text = lang.ElemText;
        barbartext.text = lang.BarbarText;
        rogtext.text = lang.RogText;
        wizardtext.text = lang.WizText;
        createnewchartext.text = lang.CreateNewCharacter;
        backtext.text = lang.back;

        //print(sr.SendAndGetLoginSetup("1~2~" + "77NGYdzGd9" + "~" + "wizwizwiz"));

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isBusy)
        {
            Off();
        } 
        else if (!isBusy)
        {
            Onn();
        }

        if (sr.isConnectionError)
        {
            StartCoroutine(ConnectionErr());
        }

        /*
        if (cur_time>2)
        {
            cur_time = 0;
            await Task.Run(() => sr.isServerOff());            

        } else
        {
            cur_time += Time.deltaTime;
        }
        */

        if (char_name_input.text != null)
        {

            bool isOK = true;
            if (char_name_input.text.Length < 6 || char_name_input.text.Length > 16)
            {
                isOK = false;
            }
                        

            if (isOK)
            {
                OkOnChoosing.interactable = true;
                
            }
            else
            {
                OkOnChoosing.interactable = false;
                
            }
        }
        else
        {
            OkOnChoosing.interactable = false;            
        }

    }


    private void Back()
    {
        SceneManager.LoadScene("player_choose");
    }

    private void OkOnChoose()
    {
        EnterNamePanel.SetActive(false);
        string result = null;
        //result = sr.SendAndGetLoginSetup("1~1~" + general.CurrentTicket + "~" + char_name_input.text + "~" + CurrentPlayerNumber);
        result = connection.SendAndGetTCP($"{general.PacketID}~1~1~{general.CurrentTicket}~{char_name_input.text}~{CurrentPlayerNumber}", general.Ports.tcp2324, general.LoginServerIP, true);

        string[] getstr = result.Split('~');
        switch(getstr[2])
        {
            case "ok":
                print("OK");
                break;
            case "wcn":
                print("wrong character name");
                break;
            case "cae":
                print("character name allready in use");
                break;
            case "tae":
                print("you allready have such character type");
                break;
            case "err":
                print("error creating character");
                break;
            case "nst":
                print("wrong login");
                break;

        }

        if (getstr[2]=="ok")
        {
            SceneManager.LoadScene("player_choose");
        } else
        {
            SceneManager.LoadScene("player_get_new");
        }
        


    }

    private void create_char_panel_on()
    {
        Off();
        EnterNamePanel.SetActive(true);
    }

    private void Off()
    {
        if (pl1.interactable)
        {
            pl1.interactable = false;
        }
        if (pl2.interactable)
        {
            pl2.interactable = false;
        }
        if (pl3.interactable)
        {
            pl3.interactable = false;
        }
        if (pl4.interactable)
        {
            pl4.interactable = false;
        }
        if (pl5.interactable)
        {
            pl5.interactable = false;
        }
        if (create_char_button.interactable)
        {
            create_char_button.interactable = false;
        }
    }

    private void Onn()
    {
        if (!pl1.interactable)
        {
            pl1.interactable = true;
        }
        if (!pl2.interactable)
        {
            pl2.interactable = true;
        }
        if (!pl3.interactable)
        {
            pl3.interactable = true;
        }
        if (!pl4.interactable)
        {
            pl4.interactable = true;
        }
        if (!pl5.interactable)
        {
            pl5.interactable = true;
        }
        if (!create_char_button.interactable)
        {
            create_char_button.interactable = true;
        }
    }

    private void Click1()
    {
        if (CurrentPlayerNumber == 1) return;
        CurrentPlayerNumber = 1;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
        
    }

    private void Click2()
    {
        if (CurrentPlayerNumber == 2) return;
        CurrentPlayerNumber = 2;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click3()
    {
        if (CurrentPlayerNumber == 3) return;
        CurrentPlayerNumber = 3;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click4()
    {
        if (CurrentPlayerNumber == 4) return;
        CurrentPlayerNumber = 4;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click5()
    {
        if (CurrentPlayerNumber == 5) return;
        CurrentPlayerNumber = 5;
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private Vector3 GetPlayerByNumber(int Number)
    {
        Vector3 result = Vector3.zero;

        switch(Number)
        {
            case 1:
                result = WarrPos;
                break;
            case 2:
                result = MagePos;
                break;
            case 3:
                result = BarbarPos;
                break;
            case 4:
                result = RogPos;
                break;
            case 5:
                result = WizPos;
                break;

        }

        return result;
    }

    IEnumerator ChangePlayer(Vector3 NewCoords)
    {
        isBusy = true;

        for (float i=0; i<1; i+=0.1f)
        {

            PlayerLine.position = Vector3.Lerp(PlayerLine.position, NewCoords, i);

            yield return new WaitForSeconds(0.05f);
        }

        isBusy = false;

    }

    IEnumerator ConnectionErr()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("player_get_new");
    }

}
