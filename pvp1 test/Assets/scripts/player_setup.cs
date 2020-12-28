using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.EventSystems;



public class player_setup : MonoBehaviour
{
    public GameObject ConnectionError, 
        PlayerType1, PlayerType2, PlayerType3, PlayerType4, PlayerType5,
        MeleeDataPanel, MagicDataPanel, podskazka, AllPlayersTypes;

    public character_data CurrentCharacterData;

    public TextMeshProUGUI CharNameText, SpeedText, HealthText, HealthRegenText, EnergyRegenText, WeaponAttackText,
        HitPowerText, ArmorText, ShieldBlockText, MagicResistanceText, DodgeText, CastSpeedText,
        MeleeCritText, MagicCritText, SpellPowerText, BackTo, HintText,

        SpeedTextText, HealthTextText, HealthRegenTextText, EnergyRegenTextText, WeaponAttackTextText,
        HitPowerTextText, ArmorTextText, ShieldBlockTextText, MagicResistanceTextText, DodgeTextText, CastSpeedTextText,
        MeleeCritTextText, MagicCritTextText, SpellPowerTextText;

    public Button SpellButton1, SpellButton2, SpellButton3, SpellButton4, SpellButton5, SpellButton6, BackToLogin, HeroB, TalentsB, PVPB, optionsB,
        pvp11, pvp22, pvp33, testing_but, sending_talent_info;

    public Canvas Hero, Talents, PVP, options;

    private bool isStartWaitingPVP;
    private float WaitingTimeForPing = 1f, cur_time;

    private void GetCharDataToView()
    {
        CharNameText.text = general.CharacterName;
        SpeedText.text = CurrentCharacterData.speed.ToString();
        HealthText.text = CurrentCharacterData.health.ToString();
        HealthRegenText.text = CurrentCharacterData.health_regen.ToString();
        EnergyRegenText.text = CurrentCharacterData.energy_regen.ToString();
        WeaponAttackText.text = CurrentCharacterData.weapon_attack;
        HitPowerText.text = CurrentCharacterData.hit_power.ToString();
        ArmorText.text = CurrentCharacterData.armor.ToString();
        ShieldBlockText.text = CurrentCharacterData.shield_block.ToString();
        MagicResistanceText.text = CurrentCharacterData.magic_resistance.ToString();
        DodgeText.text = CurrentCharacterData.dodge.ToString();
        CastSpeedText.text = CurrentCharacterData.cast_speed.ToString();
        MeleeCritText.text = CurrentCharacterData.melee_crit.ToString();
        MagicCritText.text = CurrentCharacterData.magic_crit.ToString();
        SpellPowerText.text = CurrentCharacterData.spell_power.ToString();

        SpellButton1.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell1).Spell1_icon;
        SpellButton2.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell2).Spell1_icon;
        SpellButton3.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell3).Spell1_icon;
        SpellButton4.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell4).Spell1_icon;
        SpellButton5.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell5).Spell1_icon;
        SpellButton6.image.sprite = DB.GetSpellByNumber(CurrentCharacterData.spell6).Spell1_icon;

        switch (general.CharacterType)
        {
            case 1:
                PlayerType1.SetActive(true);
                PlayerType2.SetActive(false);
                PlayerType3.SetActive(false);
                PlayerType4.SetActive(false);
                PlayerType5.SetActive(false);
                MeleeDataPanel.SetActive(true);
                MagicDataPanel.SetActive(false);


                break;

            case 2:
                PlayerType1.SetActive(false);
                PlayerType2.SetActive(true);
                PlayerType3.SetActive(false);
                PlayerType4.SetActive(false);
                PlayerType5.SetActive(false);
                MeleeDataPanel.SetActive(false);
                MagicDataPanel.SetActive(true);

                break;

            case 3:
                PlayerType1.SetActive(false);
                PlayerType2.SetActive(false);
                PlayerType3.SetActive(true);
                PlayerType4.SetActive(false);
                PlayerType5.SetActive(false);
                MeleeDataPanel.SetActive(true);
                MagicDataPanel.SetActive(false);

                break;

            case 4:
                PlayerType1.SetActive(false);
                PlayerType2.SetActive(false);
                PlayerType3.SetActive(false);
                PlayerType4.SetActive(true);
                PlayerType5.SetActive(false);
                MeleeDataPanel.SetActive(true);
                MagicDataPanel.SetActive(false);

                break;

            case 5:
                PlayerType1.SetActive(false);
                PlayerType2.SetActive(false);
                PlayerType3.SetActive(false);
                PlayerType4.SetActive(false);
                PlayerType5.SetActive(true);
                MeleeDataPanel.SetActive(false);
                MagicDataPanel.SetActive(true);

                break;



        }
    }


    // Start is called before the first frame update
    void Start()
    {
        sr.isConnectionError = false;

        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        ConnectionError.SetActive(false);

        BackTo.text = lang.back;

        SpeedTextText.text = lang.SpeedText;
        HealthTextText.text = lang.HealthText;
        HealthRegenTextText.text = lang.HealthRegenText;
        EnergyRegenTextText.text = lang.EnergyRegenText;
        WeaponAttackTextText.text = lang.WeaponAttackText;
        HitPowerTextText.text = lang.HitPowerText;
        ArmorTextText.text = lang.ArmorText;
        ShieldBlockTextText.text = lang.ShieldBlockText;
        MagicResistanceTextText.text = lang.MagicResistanceText;
        DodgeTextText.text = lang.DodgeText;
        CastSpeedTextText.text = lang.CastSpeedText;
        MeleeCritTextText.text = lang.MeleeCritText;
        MagicCritTextText.text = lang.MagicCritText;
        SpellPowerTextText.text = lang.SpellPowerText;

        Hero.gameObject.SetActive(true);
        Talents.gameObject.SetActive(false);
        PVP.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        
        HeroB.gameObject.transform.position = new Vector3(-35, HeroB.transform.position.y, 0);

        podskazka.SetActive(false);

        string result = sr.SendAndGetOnlySetup("2~0~" + general.CurrentTicket + "~" + general.CharacterName);
        CurrentCharacterData = new character_data(result);

        GetCharDataToView();

        BackToLogin.onClick.AddListener(BackToLogChoose);
        pvp11.onClick.AddListener(pvp1vs1);
        pvp22.onClick.AddListener(pvp2vs2);
        pvp33.onClick.AddListener(pvp5vs5);
        testing_but.onClick.AddListener(testing_regime);

        sending_talent_info.onClick.AddListener(send_talents);
    }


    private void send_talents()
    {
        string result = sr.SendAndGetOnlySetup("3~4~" + general.CurrentTicket + "~" + general.CharacterName + "~" + CurrentCharacterData.talents);
        print(result);
    }


    private void testing_regime()
    {
        string result = sr.SendAndGetOnlySetup("3~0~" + general.CurrentTicket + "~" + general.CharacterName);
        print(result + "=====================================================");

        string[] getstr = result.Split('~');
        string ticket = getstr[2];

        if (ticket=="err")
        {

        } 
        else
        {
            string session = getstr[3];
            string hub_data = getstr[4];

            general.SessionTicket = session;
            general.CurrentTicket = ticket;
            general.SessionPlayerID = ticket;

            switch (hub_data)
            {
                case "1":
                    general.GameServerIP = general.HUB1_ip;
                    break;
            }

            general.SessionNumberOfPlayers = 3;

            print("OOOOOOOOOOOKKKKKKKKKKKKKK" + general.CurrentTicket + " - " + general.SessionTicket);

            SceneManager.LoadScene("SampleScene");
        }        
    }


    private void pvp1vs1()
    {
        
        string result = sr.SendAndGetOnlySetup("3~1~" + general.CurrentTicket + "~" + general.CharacterName);
        print(result);

        string[] getstr = result.Split('~');
        string type = getstr[2];
        int session_type = int.Parse(getstr[3]);

        if (type=="in" && session_type==1)
        {
            general.SessionNumberOfPlayers = 2;
            isStartWaitingPVP = true;
        }

        if (type == "out" && session_type == 1)
        {
            isStartWaitingPVP = false;
        }

    }


    private void pvp2vs2()
    {
        string result = sr.SendAndGetOnlySetup("3~2~" + general.CurrentTicket + "~" + general.CharacterName);
        print(result);

        string[] getstr = result.Split('~');
        string type = getstr[2];
        int session_type = int.Parse(getstr[3]);

        if (type == "in" && session_type == 2)
        {
            general.SessionNumberOfPlayers = 4;
            isStartWaitingPVP = true;
        }

        if (type == "out" && session_type == 2)
        {
            isStartWaitingPVP = false;
        }
    }


    private void pvp5vs5()
    {
        string result = sr.SendAndGetOnlySetup("3~3~" + general.CurrentTicket + "~" + general.CharacterName);
        print(result);

        string[] getstr = result.Split('~');
        string type = getstr[2];
        int session_type = int.Parse(getstr[3]);

        if (type == "in" && session_type == 3)
        {
            general.SessionNumberOfPlayers = 10;
            isStartWaitingPVP = true;
        }

        if (type == "out" && session_type == 3)
        {
            isStartWaitingPVP = false;
        }
    }

    private void BackToLogChoose()
    {
        SceneManager.LoadScene("player_choose");
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.isConnectionError)
        {
            StartCoroutine(ConnectionErr());
        }

        if (Input.GetMouseButtonDown(0))
        {
            

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                print(EventSystem.current.currentSelectedGameObject.name);

                switch(EventSystem.current.currentSelectedGameObject.name)
                {
                    case "herobutton":
                        Hero.gameObject.SetActive(true);                        
                        Talents.gameObject.SetActive(false);
                        PVP.gameObject.SetActive(false);
                        options.gameObject.SetActive(false);
                        
                        ChangeCanvasButton(true, false, false, false);
                        break;

                    case "talentsbutton":
                        Hero.gameObject.SetActive(false);
                        Talents.gameObject.SetActive(true);
                        PVP.gameObject.SetActive(false);
                        options.gameObject.SetActive(false);
                        
                        ChangeCanvasButton(false, true, false, false);
                        break;

                    case "PVPbutton":
                        Hero.gameObject.SetActive(false);
                        Talents.gameObject.SetActive(false);
                        PVP.gameObject.SetActive(true);
                        options.gameObject.SetActive(false);
                        
                        ChangeCanvasButton(false, false, true, false);
                        break;

                    case "optionsbutton":
                        Hero.gameObject.SetActive(false);
                        Talents.gameObject.SetActive(false);
                        PVP.gameObject.SetActive(false);
                        options.gameObject.SetActive(true);
                        
                        ChangeCanvasButton(false, false, false, true);
                        break;
                }

                if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "hints")
                {
                    StartCoroutine(GetHintOver(EventSystem.current.currentSelectedGameObject.name, new Vector2(EventSystem.current.currentSelectedGameObject.transform.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y)));
                } 
                else
                {
                    podskazka.SetActive(false);
                }
            } 
            else
            {
                podskazka.SetActive(false);
            }
        }

        if (isStartWaitingPVP)
        {
            if (cur_time>WaitingTimeForPing)
            {
                print(general.CurrentTicket + " ============" + general.CharacterName);
                cur_time = 0;
                string result = sr.SendAndGetOnlySetup("4~0~" + general.CurrentTicket + "~" + general.CharacterName);
                print(result);

                string[] getstr = result.Split('~');

                int session_type = int.Parse(getstr[2]);
                string ticket = getstr[3];
                string session = getstr[4];
                string hub_data = getstr[5];

                if (session_type==0 && ticket=="nst")
                {
                    SceneManager.LoadScene("player_choose");
                }

                if (session_type == 2)
                {
                    
                    print("GGGGGGGEEEEEEEETTTTTT      RRRRRRRREEEEEAAAADDDDDDDDYYYYYYYYY");
                }

                if (session_type == 4)
                {
                    
                    general.CurrentTicket = ticket;

                    print("FFFFFFFAAAAAAAAAAIIIILLLLLLLLLLLLLEEEEEEEEEDDDDDDDDDDDD");
                    SceneManager.LoadScene("player_choose");
                }

                if (session_type == 3)
                {
                    general.SessionTicket = session;
                    general.CurrentTicket = ticket;
                    general.SessionPlayerID = ticket;

                    switch(hub_data)
                    {
                        case "1":
                            general.GameServerIP = general.HUB1_ip;
                            break;
                    }
                    
                    print("OOOOOOOOOOOKKKKKKKKKKKKKK");

                    SceneManager.LoadScene("SampleScene");
                }


            } 
            else
            {
                cur_time += Time.deltaTime;
            }
            

        }

    }


    private void ChangeCanvasButton(bool h, bool t, bool p, bool o)
    {
                
        StartCoroutine(CanvasButtonOn(HeroB.gameObject, h));
        StartCoroutine(CanvasButtonOn(TalentsB.gameObject, t));
        StartCoroutine(CanvasButtonOn(PVPB.gameObject, p));
        StartCoroutine(CanvasButtonOn(optionsB.gameObject, o));
       
    }


    IEnumerator CanvasButtonOn(GameObject ButtonGameObject, bool direction)
    {
        // -80  -30
        
        if ((direction && ButtonGameObject.transform.position.x != -35) || (!direction && ButtonGameObject.transform.position.x != -75))
        {

            HeroB.interactable = false;
            TalentsB.interactable = false;
            PVPB.interactable = false;
            optionsB.interactable = false;

            for (float i = 0; i < 1; i += 0.1f)
            {
                if (direction)
                {
                    if (ButtonGameObject.transform.position.x != -35) ButtonGameObject.transform.position = Vector3.Lerp(new Vector3(-80, ButtonGameObject.transform.position.y, 0), new Vector3(-30, ButtonGameObject.transform.position.y, 0), i);
                }
                else
                {
                    if (ButtonGameObject.transform.position.x != -75) ButtonGameObject.transform.position = Vector3.Lerp(new Vector3(-30, ButtonGameObject.transform.position.y, 0), new Vector3(-80, ButtonGameObject.transform.position.y, 0), i);
                }


                yield return new WaitForSeconds(Time.deltaTime);
            }

            HeroB.interactable = true;
            TalentsB.interactable = true;
            PVPB.interactable = true;
            optionsB.interactable = true;
        }

    }


    IEnumerator ConnectionErr()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("player_setup");
    }

    IEnumerator GetHintOver(string HintLog, Vector2 coords)
    {
        podskazka.SetActive(true);
        podskazka.transform.position = new Vector3(coords.x, coords.y);
            

        switch(HintLog)
        {
            case "sp":
                HintText.text = lang.SpeedTextHint;
                break;

            case "h":
                HintText.text = lang.HealthTextHint;
                break;

            case "hr":
                HintText.text = lang.HealthRegenTextHint;
                break;

            case "er":
                HintText.text = lang.EnergyRegenTextHint;
                break;

            case "ar":
                HintText.text = lang.ArmorTextHint;
                break;

            case "do":
                HintText.text = lang.DodgeTextHint;
                break;

            case "mr":
                HintText.text = lang.MagicResistanceTextHint;
                break;

            case "sb":
                HintText.text = lang.ShieldBlockTextHint;
                break;

            case "cs":
                HintText.text = lang.CastSpeedTextHint;
                break;

            case "spw":
                HintText.text = lang.SpellPowerTextHint;
                break;

            case "mc":
                HintText.text = lang.MagicCritTextHint;
                break;

            case "wa":
                HintText.text = lang.WeaponAttackTextHint;
                break;

            case "hp":
                HintText.text = lang.HitPowerTextHint;
                break;

            case "melc":
                HintText.text = lang.MeleeCritTextHint;
                break;

        }

        string BaseHintText = HintText.text;
        for (float i = 0; i < 5; i+=0.1f)
        {
            if (BaseHintText!= HintText.text)
            {
                BaseHintText = HintText.text;
                i = 0;
            }
            yield return new WaitForSeconds(0.1f);
        }
        podskazka.SetActive(false);

    }

}


public struct character_data
{    
    public float speed;
    public float health;
    public float health_regen;
    public float energy_regen;
    public string weapon_attack;
    public float hit_power;
    public float armor;
    public float shield_block;
    public float magic_resistance;
    public float dodge;
    public float cast_speed;
    public float melee_crit;
    public float magic_crit;
    public float spell_power;
    public int spell1;
    public int spell2;
    public int spell3;
    public int spell4;
    public int spell5;
    public int spell6;
    public string spell_book;
    public string talents;

    public character_data(string resultdata)
    {
        Debug.Log(resultdata);

        string[] getstr = resultdata.Split('~');

        speed = float.Parse(getstr[2], CultureInfo.InvariantCulture);
        health = float.Parse(getstr[3], CultureInfo.InvariantCulture);
        health_regen = float.Parse(getstr[4], CultureInfo.InvariantCulture);
        energy_regen = float.Parse(getstr[5], CultureInfo.InvariantCulture);
        weapon_attack = getstr[6];
        hit_power = float.Parse(getstr[7], CultureInfo.InvariantCulture);
        armor = float.Parse(getstr[8], CultureInfo.InvariantCulture);
        shield_block = float.Parse(getstr[9], CultureInfo.InvariantCulture);
        magic_resistance = float.Parse(getstr[10], CultureInfo.InvariantCulture);
        dodge = float.Parse(getstr[11], CultureInfo.InvariantCulture);
        cast_speed = float.Parse(getstr[12], CultureInfo.InvariantCulture);
        melee_crit = float.Parse(getstr[13], CultureInfo.InvariantCulture);
        magic_crit = float.Parse(getstr[14], CultureInfo.InvariantCulture);
        spell_power = float.Parse(getstr[15], CultureInfo.InvariantCulture);
        spell1 = int.Parse(getstr[16]);
        spell2 = int.Parse(getstr[17]);
        spell3 = int.Parse(getstr[18]);
        spell4 = int.Parse(getstr[19]);
        spell5 = int.Parse(getstr[20]);
        spell6 = int.Parse(getstr[21]);
        spell_book = getstr[22];
        talents = getstr[23];


    }
}