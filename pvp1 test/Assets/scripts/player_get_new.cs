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
    
    public TMP_InputField char_name_input;
    public Transform PlayerLine;
    public GameObject EnterNamePanel;
    public Button pl1, pl2, pl3, pl4, pl5, create_char_button, OkOnChoosing, back_button;

    private int CurrentPlayerNumber = 1;
    private bool isBusy;
    private float delta_for_moving = 0.01f;

    public TextMeshProUGUI char_class_name_in_discr, char_descr, char_conspros, createnewchartext, backtext, enterloginname, enter_char_name_text;

    public GameObject err_log_window;
    MessageInfo error_messages;

    // Start is called before the first frame update
    void Start()
    {
        
        error_messages = new MessageInfo(err_log_window);

        string result = null;
        //result = sr.SendAndGetLoginSetup("1~0~" + general.CurrentTicket);
        try
        {
            result = connection.SendAndGetTCP($"{general.PacketID}~1~0~{general.CurrentTicket}", general.Ports.tcp2324, general.LoginServerIP, true);
        }
        catch (System.Exception ex)
        {
            StartCoroutine(error_messages.process_error("con_err"));
        }

        string[] getstr = result.Split('~');
        List<int> existing_chars = new List<int>();

        if (getstr[2] != "nst" && getstr[2] != "nc")
        {
            if(getstr[2] == "nsc")
            {
                encryption.InitEncodingConnection(general.Ports.tcp2324);
                SceneManager.LoadScene("player_get_new");
            }

            int index = int.Parse(getstr[2]);
            if (index > 0)
            {
                for (int i = 1; i <= index; i++)
                {
                    existing_chars.Add( int.Parse(getstr[2 + 2*i]));

                }
                for (int i = 0; i < existing_chars.Count; i++)
                {
                    print(existing_chars[i] + " - ");
                }
            }           
        }

        


        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        EnterNamePanel.SetActive(false);
        //ConnectionError.SetActive(false);
        pl1.onClick.AddListener(Click1);
        pl2.onClick.AddListener(Click2);
        pl3.onClick.AddListener(Click3);
        pl4.onClick.AddListener(Click4);
        pl5.onClick.AddListener(Click5);
        create_char_button.onClick.AddListener(create_char_panel_on);
        OkOnChoosing.onClick.AddListener(OkOnChoose);
        back_button.onClick.AddListener(Back);

        enter_char_name_text.text = lang.EnterCharName;
        
        
        
        
        createnewchartext.text = lang.CreateNewCharacter;
        backtext.text = lang.back;


        if (existing_chars.Count > 0)
        {
            for (int i = 0; i < existing_chars.Count; i++)
            {
                switch (existing_chars[i])
                {
                    case 1:
                        pl1.interactable = false;
                        break;
                    case 2:
                        pl2.interactable = false;
                        break;
                    case 3:
                        pl3.interactable = false;
                        break;
                    case 4:
                        pl4.interactable = false;
                        break;
                    case 5:
                        pl5.interactable = false;
                        break;
                }
                
            }            
        }

        for (int i = 1; i <= 5; i++)
        {
            if (!existing_chars.Contains(i))
            {
                switch (i)
                {
                    case 1:
                        CurrentPlayerNumber = 1;
                        pl1.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        break;
                    case 2:
                        CurrentPlayerNumber = 2;
                        pl2.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        break;
                    case 3:
                        CurrentPlayerNumber = 3;
                        pl3.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        break;
                    case 4:
                        CurrentPlayerNumber = 4;
                        pl4.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        break;
                    case 5:
                        CurrentPlayerNumber = 5;
                        pl5.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        break;
                }
                StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
                break;
            }            
        }

        update_char_class_description();
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

    private void update_char_class_description()
    {
        pl1.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        pl2.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        pl3.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        pl4.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        pl5.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        switch (CurrentPlayerNumber)
        {
            case 1:
                char_class_name_in_discr.text = lang.WarriorText;
                char_descr.text = lang.WarriorText_descr;
                char_conspros.text = lang.WarriorText_conspros;
                pl1.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 2:
                char_class_name_in_discr.text = lang.ElemText;
                char_descr.text = lang.ElemText_descr;
                char_conspros.text = lang.ElemText_conspros;
                pl2.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 3:
                char_class_name_in_discr.text = lang.BarbarText;
                char_descr.text = lang.BarbarText_descr;
                char_conspros.text = lang.BarbarText_conspros;
                pl3.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 4:
                char_class_name_in_discr.text = lang.RogText;
                char_descr.text = lang.RogText_descr;
                char_conspros.text = lang.RogText_conspros;
                pl4.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 5:
                char_class_name_in_discr.text = lang.WizText;
                char_descr.text = lang.WizText_descr;
                char_conspros.text = lang.WizText_conspros;
                pl5.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;

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

        try
        {
            result = connection.SendAndGetTCP($"{general.PacketID}~1~1~{general.CurrentTicket}~{char_name_input.text}~{CurrentPlayerNumber}", general.Ports.tcp2324, general.LoginServerIP, true);

            string[] getstr = result.Split('~');
            print(result);
            /*
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
            */

            if (codes.GetCodeResult(getstr[2]) != "none")
            {
                StartCoroutine(error_messages.process_error(getstr[2]));
            } 

            if (getstr[2] == "ok")
            {
                SceneManager.LoadScene("player_choose");
            }
            else
            {
                StartCoroutine(LoadAgain());
            }
        }
        catch (System.Exception)
        {
            StartCoroutine(error_messages.process_error("con_err"));
        }
        
        


    }

    IEnumerator LoadAgain()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("player_get_new");
    }

    private void create_char_panel_on()
    {
        Off();
        EnterNamePanel.SetActive(true);
    }

    private void Off()
    {
        /*
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
        */

        if (pl1.isActiveAndEnabled)
        {
            pl1.gameObject.SetActive(false);
        }
        if (pl2.isActiveAndEnabled)
        {
            pl2.gameObject.SetActive(false);
        }
        if (pl3.isActiveAndEnabled)
        {
            pl3.gameObject.SetActive(false);
        }
        if (pl4.isActiveAndEnabled)
        {
            pl4.gameObject.SetActive(false);
        }
        if (pl5.isActiveAndEnabled)
        {
            pl5.gameObject.SetActive(false);
        }


        if (create_char_button.interactable)
        {
            create_char_button.interactable = false;
        }
    }

    private void Onn()
    {
        /*
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
        */

        if (!pl1.isActiveAndEnabled)
        {
            pl1.gameObject.SetActive(true);
        }
        if (!pl2.isActiveAndEnabled)
        {
            pl2.gameObject.SetActive(true);
        }
        if (!pl3.isActiveAndEnabled)
        {
            pl3.gameObject.SetActive(true);
        }
        if (!pl4.isActiveAndEnabled)
        {
            pl4.gameObject.SetActive(true);
        }
        if (!pl5.isActiveAndEnabled)
        {
            pl5.gameObject.SetActive(true);
        }


        if (!create_char_button.interactable)
        {
            create_char_button.interactable = true;
        }
    }

    void reset_all_buttons()
    {
        pl1.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        pl2.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        pl3.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        pl4.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        pl5.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    private void Click1()
    {
        if (CurrentPlayerNumber == 1) return;
        CurrentPlayerNumber = 1;
        reset_all_buttons();
        print("YYYYYYYYYYYYEEEEEEEESSSSSSSSSSSS");
        pl1.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
        
    }

    private void Click2()
    {
        if (CurrentPlayerNumber == 2) return;
        CurrentPlayerNumber = 2;
        reset_all_buttons();
        pl2.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click3()
    {
        if (CurrentPlayerNumber == 3) return;
        CurrentPlayerNumber = 3;
        reset_all_buttons();
        pl3.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click4()
    {
        if (CurrentPlayerNumber == 4) return;
        CurrentPlayerNumber = 4;
        reset_all_buttons();
        pl4.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        StartCoroutine(ChangePlayer(GetPlayerByNumber(CurrentPlayerNumber)));
    }

    private void Click5()
    {
        if (CurrentPlayerNumber == 5) return;
        CurrentPlayerNumber = 5;
        reset_all_buttons();
        pl5.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
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
        update_char_class_description();
        isBusy = true;

        for (float i=0; i<1; i+=0.1f)
        {

            PlayerLine.position = Vector3.Lerp(PlayerLine.position, NewCoords, i);

            yield return new WaitForSeconds(delta_for_moving);
        }

        

        isBusy = false;
        delta_for_moving = 0.025f;
    }

   

}
