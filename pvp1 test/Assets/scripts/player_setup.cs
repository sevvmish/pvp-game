using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.EventSystems;
using System;


public class player_setup : MonoBehaviour
{
    public const int MaxTalents = 11;
    public const int MaxTalentsInRow = 8;

    public GameObject ConnectionError, 
        PlayerType1, PlayerType2, PlayerType3, PlayerType4, PlayerType5,
        MeleeDataPanel, MagicDataPanel, BaseAttrDataPanel, podskazka, AllPlayersTypes, SpellBook;

    public character_data CurrentCharacterData;

    public TextMeshProUGUI CharNameText, SpeedText, HealthText, HealthRegenText, EnergyRegenText, WeaponAttackText,
        HitPowerText, ArmorText, ShieldBlockText, MagicResistanceText, DodgeText, CastSpeedText,
        MeleeCritText, MagicCritText, SpellPowerText, BackTo, HintText, fortesting,

        SpeedTextText, HealthTextText, HealthRegenTextText, EnergyRegenTextText, WeaponAttackTextText,
        HitPowerTextText, ArmorTextText, ShieldBlockTextText, MagicResistanceTextText, DodgeTextText, CastSpeedTextText,
        MeleeCritTextText, MagicCritTextText, SpellPowerTextText,
        
        Line1Talents, Line2Talents, Line3Talents, UsedTalentsInfo;

    public Button BackToLogin, HeroB, TalentsB, PVPB, optionsB,
        pvp11, pvp22, pvp33, testing_but, sending_talent_info, ResetTalentsButton;

    public Canvas Hero, Talents, PVP, options;

    private bool isStartWaitingPVP, isNoTalentPointAvailable, isSpellBookOpened;

    public static bool isSpellDragedFromSpellBook, isEndDragAndDrop;

    private float WaitingTimeForPing = 1f, cur_time, cur_time_check_talents;
    private int CurrentTalentsCount;

    public int[,] CurrentTalentsSpread = new int [,] { {0,0,0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

    TalentsButton r00, r01, r02, r10, r11, r12, r20, r21, r30, r31, r32, r40, r41, r42, r50, r51, r60, r61, r62, r70, r71, r72;
    List<TalentsButton> TalentBottonsList = new List<TalentsButton>();
    
    public GameObject err_log_window;
    MessageInfo error_messages;

    private RectTransform SpellAttrInside;
    public  RectTransform SpellBookContent;

    private SpellDescription current_spell1, current_spell2, current_spell3, current_spell4, current_spell5;
    private List<SpellDescription> AllCurrentSpells = new List<SpellDescription>();
    private List<SpellDescription> AllSpellBookSpells = new List<SpellDescription>();
    private int SpellCheckedInSpellBook;
    private string SpellCheckedInCurrentSpellsName=null;

    Ray ray;
    RaycastHit hit;

    private void GetCharDataToView()
    {
        //string result = connection.SendAndGetTCP($"{general.PacketID}~2~0~{general.CurrentTicket}~{general.CharacterName}", general.Ports.tcp2326, general.SetupServerIP, true);
        string result = connection.SendAndGetTCP($"{general.PacketID}~2~0~{general.CurrentTicket}~{general.CharacterName}", general.Ports.tcp2326, general.SetupServerIP, true);
        
        try
        {
            CurrentCharacterData = new character_data(result);
        } 
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        FromStringToArrTalents(out CurrentTalentsSpread, CurrentCharacterData.talents);


        print(general.CharacterName + " - " + general.CharacterType + " - in SETUP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        print(FromArrToStringTalents(CurrentTalentsSpread) + " !");

        CharNameText.text = general.CharacterName;
        SpeedText.text = CurrentCharacterData.speed.ToString("f1");
        HealthText.text = CurrentCharacterData.health.ToString("f0");
        HealthRegenText.text = CurrentCharacterData.health_regen.ToString("f1");
        EnergyRegenText.text = CurrentCharacterData.energy_regen.ToString("f1");
        WeaponAttackText.text = CurrentCharacterData.weapon_attack;
        HitPowerText.text = CurrentCharacterData.hit_power.ToString("f1");
        ArmorText.text = CurrentCharacterData.armor.ToString("f0");
        ShieldBlockText.text = CurrentCharacterData.shield_block.ToString("f1");
        MagicResistanceText.text = CurrentCharacterData.magic_resistance.ToString("f1");
        DodgeText.text = CurrentCharacterData.dodge.ToString("f1");
        CastSpeedText.text = CurrentCharacterData.cast_speed.ToString("f1");
        MeleeCritText.text = CurrentCharacterData.melee_crit.ToString("f1");
        MagicCritText.text = CurrentCharacterData.magic_crit.ToString("f1");
        SpellPowerText.text = CurrentCharacterData.spell_power.ToString("f1");

        if (Hero.gameObject.activeSelf)
        {

            CurrentCharacterData.spell_book = CurrentCharacterData.spell_book.Substring(2);
            print(CurrentCharacterData.spell_book);

            CloseSpellBook();

        }


        //SpellsButton sp1 = new SpellsButton(1, new Vector2(0, 0), "butt11111");

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


        current_spell1.SetNewSpell(CurrentCharacterData.spell1);
        current_spell2.SetNewSpell(CurrentCharacterData.spell2);
        current_spell3.SetNewSpell(CurrentCharacterData.spell3);
        current_spell4.SetNewSpell(CurrentCharacterData.spell4);
        current_spell5.SetNewSpell(CurrentCharacterData.spell5);


    }

    

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        ConnectionError.SetActive(false);

        error_messages = new MessageInfo(err_log_window);

        try
        {
            encryption.InitEncodingConnection(general.Ports.tcp2326);
        }
        catch (Exception)
        {
            StartCoroutine(error_messages.process_error("con_err"));
        }
        

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

        SpellAttrInside = GameObject.Find("spell attr inside").GetComponent<RectTransform>();


        HeroB.gameObject.transform.position = new Vector3(-35, HeroB.transform.position.y, 0);

        podskazka.SetActive(false);

        Hero.gameObject.SetActive(true);

        current_spell1 = new SpellDescription(0, new Vector2(-17, 200), GameObject.Find("spell attr inside").transform, "current_spell1", true);
        current_spell2 = new SpellDescription(0, new Vector2(-17, 112.5f), GameObject.Find("spell attr inside").transform, "current_spell2", true);
        current_spell3 = new SpellDescription(0, new Vector2(-17, 25), GameObject.Find("spell attr inside").transform, "current_spell3", true);
        current_spell4 = new SpellDescription(0, new Vector2(-17, -62.5f), GameObject.Find("spell attr inside").transform, "current_spell4", true);
        current_spell5 = new SpellDescription(0, new Vector2(-17, -150), GameObject.Find("spell attr inside").transform, "current_spell5", true);
        AllCurrentSpells.Add(current_spell1);
        AllCurrentSpells.Add(current_spell2);
        AllCurrentSpells.Add(current_spell3);
        AllCurrentSpells.Add(current_spell4);
        AllCurrentSpells.Add(current_spell5);


        GetCharDataToView();

        int StartTalentNumber = 0;

        switch(general.CharacterType)
        {
            case 1:
                StartTalentNumber = 1;
                break;
            case 2:
                StartTalentNumber = 23;
                break;
            case 3:
                StartTalentNumber = 45;
                break;
            case 4:
                StartTalentNumber = 67;
                break;
            case 5:
                StartTalentNumber = 89;
                break;

        }

        Talents.gameObject.SetActive(true);
        r00 = new TalentsButton(StartTalentNumber, DB.GetTalentByNumber(StartTalentNumber).Talent_icon, CurrentTalentsSpread[0, 0], 1, "r00", new Vector2(-424, 146), 80);
        r01 = new TalentsButton(StartTalentNumber + 1, DB.GetTalentByNumber(StartTalentNumber + 1).Talent_icon, CurrentTalentsSpread[0, 1], 1, "r01", new Vector2(-424, 5),70);
        r02 = new TalentsButton(StartTalentNumber + 2, DB.GetTalentByNumber(StartTalentNumber + 2).Talent_icon, CurrentTalentsSpread[0, 2], 1, "r02", new Vector2(-424, -139), 100);
        r10 = new TalentsButton(StartTalentNumber + 3, DB.GetTalentByNumber(StartTalentNumber + 3).Talent_icon, CurrentTalentsSpread[1, 0], 1, "r10", new Vector2(-279, 146), 100);
        r11 = new TalentsButton(StartTalentNumber + 4, DB.GetTalentByNumber(StartTalentNumber + 4).Talent_icon, CurrentTalentsSpread[1, 1], 1, "r11", new Vector2(-279, 5), 100);
        r12 = new TalentsButton(StartTalentNumber + 5, DB.GetTalentByNumber(StartTalentNumber + 5).Talent_icon, CurrentTalentsSpread[1, 2], 1, "r12", new Vector2(-279, -139), 100);
        r20 = new TalentsButton(StartTalentNumber + 6, DB.GetTalentByNumber(StartTalentNumber + 6).Talent_icon, CurrentTalentsSpread[2, 0], 1, "r20", new Vector2(-154, 146), 100);
        r21 = new TalentsButton(StartTalentNumber + 7, DB.GetTalentByNumber(StartTalentNumber + 7).Talent_icon, CurrentTalentsSpread[2, 1], 1, "r21", new Vector2(-154, 5), 100);
        r30 = new TalentsButton(StartTalentNumber + 8, DB.GetTalentByNumber(StartTalentNumber + 8).Talent_icon, CurrentTalentsSpread[3, 0], 1, "r30", new Vector2(-35, 146), 100);
        r31 = new TalentsButton(StartTalentNumber + 9, DB.GetTalentByNumber(StartTalentNumber + 9).Talent_icon, CurrentTalentsSpread[3, 1], 1, "r31", new Vector2(-35, 5), 100);
        r32 = new TalentsButton(StartTalentNumber + 10, DB.GetTalentByNumber(StartTalentNumber + 10).Talent_icon, CurrentTalentsSpread[3, 2], 1, "r32", new Vector2(-35, -139), 100);
        r40 = new TalentsButton(StartTalentNumber + 11, DB.GetTalentByNumber(StartTalentNumber + 11).Talent_icon, CurrentTalentsSpread[4, 0], 1, "r40", new Vector2(98, 146), 100);
        r41 = new TalentsButton(StartTalentNumber + 12, DB.GetTalentByNumber(StartTalentNumber + 12).Talent_icon, CurrentTalentsSpread[4, 1], 1, "r41", new Vector2(98, 5), 100);
        r42 = new TalentsButton(StartTalentNumber + 13, DB.GetTalentByNumber(StartTalentNumber + 13).Talent_icon, CurrentTalentsSpread[4, 2], 1, "r42", new Vector2(98, -139), 100);
        r50 = new TalentsButton(StartTalentNumber + 14, DB.GetTalentByNumber(StartTalentNumber + 14).Talent_icon, CurrentTalentsSpread[5, 0], 1, "r50", new Vector2(223, 146), 100);
        r51 = new TalentsButton(StartTalentNumber + 15, DB.GetTalentByNumber(StartTalentNumber + 15).Talent_icon, CurrentTalentsSpread[5, 1], 1, "r51", new Vector2(223, 5), 100);
        r60 = new TalentsButton(StartTalentNumber + 16, DB.GetTalentByNumber(StartTalentNumber + 16).Talent_icon, CurrentTalentsSpread[6, 0], 1, "r60", new Vector2(345, 146), 100);
        r61 = new TalentsButton(StartTalentNumber + 17, DB.GetTalentByNumber(StartTalentNumber + 17).Talent_icon, CurrentTalentsSpread[6, 1], 1, "r61", new Vector2(345, 5), 100);
        r62 = new TalentsButton(StartTalentNumber + 18, DB.GetTalentByNumber(StartTalentNumber + 18).Talent_icon, CurrentTalentsSpread[6, 2], 1, "r62", new Vector2(345, -139), 100);
        r70 = new TalentsButton(StartTalentNumber + 19, DB.GetTalentByNumber(StartTalentNumber + 19).Talent_icon, CurrentTalentsSpread[7, 0], 1, "r70", new Vector2(479, 146), 100);
        r71 = new TalentsButton(StartTalentNumber + 20, DB.GetTalentByNumber(StartTalentNumber + 20).Talent_icon, CurrentTalentsSpread[7, 1], 1, "r71", new Vector2(479, 5), 100);
        r72 = new TalentsButton(StartTalentNumber + 21, DB.GetTalentByNumber(StartTalentNumber + 21).Talent_icon, CurrentTalentsSpread[7, 2], 1, "r72", new Vector2(479, -139), 100);

        TalentBottonsList.AddRange(new TalentsButton[] { r00, r01, r02, r10, r11, r12, r20, r21, r30, r31, r32, r40, r41, r42, r50, r51, r60, r61, r62, r70, r71, r72 });

        CheckNormalTalentDisp();       

        BackToLogin.onClick.AddListener(BackToLogChoose);
        pvp11.onClick.AddListener(pvp1vs1);
        pvp22.onClick.AddListener(pvp2vs2);
        pvp33.onClick.AddListener(pvp5vs5);
        testing_but.onClick.AddListener(testing_regime);
        ResetTalentsButton.onClick.AddListener(ResetAllTalents);

        sending_talent_info.onClick.AddListener(send_talents);

        Hero.gameObject.SetActive(true);
        Talents.gameObject.SetActive(false);
        PVP.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        
        CloseSpellBook();
        
        

    }


    private void SendNewBaseSpells()
    {
        string NewSpellBaseButtons = AllCurrentSpells[0].GetSpellNumber().ToString() + "," +
            AllCurrentSpells[1].GetSpellNumber().ToString() + "," +
            AllCurrentSpells[2].GetSpellNumber().ToString() + "," +
            AllCurrentSpells[3].GetSpellNumber().ToString() + "," +
            AllCurrentSpells[4].GetSpellNumber().ToString() + "," +
            "0";

        string result;
        //string result = sr.SendAndGetOnlySetup("3~5~" + general.CurrentTicket + "~" + general.CharacterName + "~" + NewSpellBaseButtons);
        try
        {
            result = connection.SendAndGetTCP($"{general.PacketID}~3~5~{general.CurrentTicket}~{general.CharacterName}~{NewSpellBaseButtons}", general.Ports.tcp2326, general.SetupServerIP, true);
            print(result + " spells new");
        }
        catch (Exception ex)
        {
            print(ex);
        }
        
        
        GetCharDataToView();
    }


    private void OpenSpellBook()
    {
        if (!isSpellBookOpened)
        {
            isSpellBookOpened = true;
            SpellBook.SetActive(true);
            BaseAttrDataPanel.SetActive(false);

            if (SpellBookContent.gameObject.transform.childCount > 0)
            {
                for (int i = 0; i < SpellBookContent.gameObject.transform.childCount; i++)
                {
                    Destroy(SpellBookContent.gameObject.transform.GetChild(i).gameObject);
                }
            }

            AllSpellBookSpells = new List<SpellDescription>();

            float VerticalInterval = -80;

            string[] _spells = CurrentCharacterData.spell_book.Split(',');

            print(CurrentCharacterData.spell_book + " - " + _spells.Length);

            float Ylimit = _spells.Length * Mathf.Abs(VerticalInterval) + 20;
            SpellBookContent.sizeDelta = new Vector2(0, Ylimit);

            for (int i = 0; i < _spells.Length; i++)
            {
                AllSpellBookSpells.Add(new SpellDescription(int.Parse(_spells[i]), new Vector2(0, (Ylimit/2-50) + i * VerticalInterval), SpellBookContent.gameObject.transform, $"spell {i}", false));
            }
        }
    }

    private void CloseSpellBook()
    {
        
        isSpellBookOpened = false;
        SpellBook.SetActive(false);
        for (int i = 0; i < AllSpellBookSpells.Count; i++)
        {
            if (AllSpellBookSpells[i].IsPressed())
            {
                AllSpellBookSpells[i].PressedOff();
            }
        }

        for (int i = 0; i < AllCurrentSpells.Count; i++)
        {
            if (AllCurrentSpells[i].IsPressed())
            {
                AllCurrentSpells[i].PressedOff();
            }
        }

        SpellCheckedInCurrentSpellsName = null;
        SpellCheckedInSpellBook = -1;
        BaseAttrDataPanel.SetActive(true);
    }

    private void CheckNormalTalentDisp()
    {
        

        r00.MakeActive();
        r01.MakeActive();
        r02.MakeActive();
        r10.MakeActive();
        r11.MakeActive();
        r12.MakeActive();

        //only one talent per row==================
        if (r20.GetCurrentTalentPoint() == 1)
        {
            r21.NonAvailable();
        }
        else
        {
            r21.Available();
        }

        if (r21.GetCurrentTalentPoint() == 1)
        {
            r20.NonAvailable();
        }
        else
        {
            r20.Available();
        }

        if (r50.GetCurrentTalentPoint() == 1)
        {
            r51.NonAvailable();
        }
        else
        {
            r51.Available();
        }
        if (r51.GetCurrentTalentPoint() == 1)
        {
            r50.NonAvailable();
        }
        else
        {
            r50.Available();
        }

        //first row==========================

        if ((r00.GetCurrentTalentPoint() + r10.GetCurrentTalentPoint())<2)
        {
            r20.MakeInactive();
            r30.MakeInactive();
            r40.MakeInactive();            
        } else
        {
            r20.MakeActive();
            r30.MakeActive();
            r40.MakeActive();
            
        }

        if ((r01.GetCurrentTalentPoint() + r11.GetCurrentTalentPoint()) < 2)
        {
            r21.MakeInactive();
            r31.MakeInactive();
            r41.MakeInactive();
            
        } else
        {
            r21.MakeActive();
            r31.MakeActive();
            r41.MakeActive();
            
        }

        if ((r02.GetCurrentTalentPoint() + r12.GetCurrentTalentPoint()) < 2)
        {            
            r32.MakeInactive();
            r42.MakeInactive();            
            
        } else
        {
            r32.MakeActive();
            r42.MakeActive();
            
        }

        //==================================================

        if ((r00.GetCurrentTalentPoint() + r10.GetCurrentTalentPoint() + r20.GetCurrentTalentPoint() + r30.GetCurrentTalentPoint() + r40.GetCurrentTalentPoint()) < 4)
        {
            r50.MakeInactive();
            r60.MakeInactive();            
        }
        else
        {
            r50.MakeActive();
            r60.MakeActive();
            

        }

        if ((r01.GetCurrentTalentPoint() + r11.GetCurrentTalentPoint() + r21.GetCurrentTalentPoint()  +r31.GetCurrentTalentPoint()  +r41.GetCurrentTalentPoint()) < 4)
        {
            r51.MakeInactive();
            r61.MakeInactive();
        }
        else
        {
            r51.MakeActive();
            r61.MakeActive();
        }

        if ((r02.GetCurrentTalentPoint() + r12.GetCurrentTalentPoint() + r32.GetCurrentTalentPoint() + r42.GetCurrentTalentPoint()) < 4)
        {
            
            r62.MakeInactive();

        }
        else
        {
            
            r62.MakeActive();

        }

        //==================================================

        if ((r00.GetCurrentTalentPoint() + r10.GetCurrentTalentPoint() + r20.GetCurrentTalentPoint() + r30.GetCurrentTalentPoint() + r40.GetCurrentTalentPoint() + r50.GetCurrentTalentPoint() + r60.GetCurrentTalentPoint()) < 6)
        {
            r70.MakeInactive();            
        }
        else
        {
            r70.MakeActive();            
        }

        if ((r01.GetCurrentTalentPoint() + r11.GetCurrentTalentPoint() + r21.GetCurrentTalentPoint() +r31.GetCurrentTalentPoint() +r41.GetCurrentTalentPoint() + r51.GetCurrentTalentPoint() + r61.GetCurrentTalentPoint()) < 6)
        {
            r71.MakeInactive();            
        }
        else
        {
            r71.MakeActive();            
        }

        if ((r02.GetCurrentTalentPoint() + r12.GetCurrentTalentPoint() + r32.GetCurrentTalentPoint() + r42.GetCurrentTalentPoint() + r62.GetCurrentTalentPoint()) < 5)
        {

            r72.MakeInactive();

        }
        else
        {

            r72.MakeActive();

        }

        //counts
        int line1 = r00.GetCurrentTalentPoint() + r10.GetCurrentTalentPoint() + r20.GetCurrentTalentPoint() + r30.GetCurrentTalentPoint() + r40.GetCurrentTalentPoint() + r50.GetCurrentTalentPoint() + r60.GetCurrentTalentPoint() + r70.GetCurrentTalentPoint();
        Line1Talents.text = line1.ToString();

        int line2 = r01.GetCurrentTalentPoint() + r11.GetCurrentTalentPoint() + r21.GetCurrentTalentPoint() + r31.GetCurrentTalentPoint() + r41.GetCurrentTalentPoint() + r51.GetCurrentTalentPoint() + r61.GetCurrentTalentPoint() + r71.GetCurrentTalentPoint();
        Line2Talents.text = line2.ToString();

        int line3 = r02.GetCurrentTalentPoint() + r12.GetCurrentTalentPoint() + r32.GetCurrentTalentPoint() + r42.GetCurrentTalentPoint() + r62.GetCurrentTalentPoint() + r72.GetCurrentTalentPoint();
        Line3Talents.text = line3.ToString();

        int summ = 0;
        for (int i = 0; i < TalentBottonsList.Count; i++)
        {
            summ += TalentBottonsList[i].GetCurrentTalentPoint();
        }

        if (summ >= MaxTalents)
        {
            for (int i = 0; i < TalentBottonsList.Count; i++)
            {
                if (TalentBottonsList[i].GetCurrentTalentPoint() == 0)
                {
                    TalentBottonsList[i].MakeInactive();
                }
            }
        }


    }

   
    private void ResetAllTalents()
    {
        for (int i=0; i<TalentBottonsList.Count; i++)
        {
            TalentBottonsList[i].ResetTalents();
        }
    }


    private void send_talents()
    {
        string TalentsToSend = r00.GetCurrentTalentPointString() + "-" + r01.GetCurrentTalentPointString() + "-" + r02.GetCurrentTalentPointString() + "," +
            r10.GetCurrentTalentPointString() + "-" + r11.GetCurrentTalentPointString() + "-" + r12.GetCurrentTalentPointString() + "," +
            r20.GetCurrentTalentPointString() + "-" + r21.GetCurrentTalentPointString() + "," +
            r30.GetCurrentTalentPointString() + "-" + r31.GetCurrentTalentPointString() + "-" + r32.GetCurrentTalentPointString() + "," +
            r40.GetCurrentTalentPointString() + "-" + r41.GetCurrentTalentPointString() + "-" + r42.GetCurrentTalentPointString() + "," +
            r50.GetCurrentTalentPointString() + "-" + r51.GetCurrentTalentPointString() + "," +
            r60.GetCurrentTalentPointString() + "-" + r61.GetCurrentTalentPointString() + "-" + r62.GetCurrentTalentPointString() + "," +
            r70.GetCurrentTalentPointString() + "-" + r71.GetCurrentTalentPointString() + "-" + r72.GetCurrentTalentPointString();

        print(TalentsToSend + " - for sending");

        //string result = sr.SendAndGetOnlySetup("3~4~" + general.CurrentTicket + "~" + general.CharacterName + "~" + TalentsToSend);
        string result = connection.SendAndGetTCP($"{general.PacketID}~3~4~{general.CurrentTicket}~{general.CharacterName}~{TalentsToSend}", general.Ports.tcp2326, general.SetupServerIP, true);
        print(result);

        GetCharDataToView();
    }


    private void testing_regime()
    {
        SceneManager.LoadScene("plchoose");
        /*
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
        }    */    
    }


    private void pvp1vs1()
    {
        string result;

        try
        {
            //result = sr.SendAndGetOnlySetup("3~1~" + general.CurrentTicket + "~" + general.CharacterName);
            result = connection.SendAndGetTCP($"{general.PacketID}~3~1~{general.CurrentTicket}~{general.CharacterName}", general.Ports.tcp2326, general.SetupServerIP, true);
            print(result);

            string[] getstr = result.Split('~');
            string type = getstr[2];
            int session_type = int.Parse(getstr[3]);

            if (type == "in" && session_type == 1)
            {
                general.SessionNumberOfPlayers = 2;
                isStartWaitingPVP = true;
            }

            if (type == "out" && session_type == 1)
            {
                isStartWaitingPVP = false;
            }
        }
        catch (Exception ex)
        {
            print(ex);
        }       

    }


    private void pvp2vs2()
    {
        string result = "";
        //string result = sr.SendAndGetOnlySetup("3~2~" + general.CurrentTicket + "~" + general.CharacterName);
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
        string result = "";
        //string result = sr.SendAndGetOnlySetup("3~3~" + general.CurrentTicket + "~" + general.CharacterName);
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
        try
        {
            if (general.PacketID != null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2326, general.SetupServerIP, false);
            }
        }
        catch (Exception ex)
        {
            print(ex);
        }
        
        SceneManager.LoadScene("player_choose");
    }

    private bool CheckIfNameEqual(string TestName)
    {

        

        bool result = false;

        for (int i=0; i<TalentBottonsList.Count; i++)
        {
            
            if (TalentBottonsList[i].GetName() == TestName)
            {

                if (!isNoTalentPointAvailable)
                {
                    //CheckNormalTalentDisp();
                    TalentBottonsList[i].AddTalentPoint();
                    //CheckNormalTalentDisp();
                    result = true;
                    return result;
                } else
                {
                    //CheckNormalTalentDisp();
                    TalentBottonsList[i].RemoveTalentPoint();
                    //CheckNormalTalentDisp();
                    result = true;
                    return result;
                }


            } 
            
        }

        
        return result;

    }



    // Update is called once per frame
    void Update()
    {
       
        if (SpellCheckedInCurrentSpellsName!=null && SpellCheckedInSpellBook!=-1)
        {
            print("curent chek - " + SpellCheckedInCurrentSpellsName + "  spell book - " + SpellCheckedInSpellBook);
            
            for (int i = 0; i < AllCurrentSpells.Count; i++)
            {
                
                if (AllCurrentSpells[i].GetSpellNumber() == SpellCheckedInSpellBook && AllCurrentSpells[i].GetName() != SpellCheckedInCurrentSpellsName)
                {
                    AllCurrentSpells[i].SetNewSpell(0);
                    
                }
            }

            for (int i = 0; i < AllCurrentSpells.Count; i++)
            {
                if (AllCurrentSpells[i].GetName()== SpellCheckedInCurrentSpellsName)
                {
                    AllCurrentSpells[i].PressedOff();
                    AllCurrentSpells[i].SetNewSpell(SpellCheckedInSpellBook);
                    break;
                }
                
            }            
            
            
            CloseSpellBook();
            SendNewBaseSpells();
        }


        if (podskazka.activeSelf && isSpellDragedFromSpellBook)
        {
            podskazka.SetActive(false);
        }

        //=================================================
        if (Talents.gameObject.activeSelf)
        {
            if (cur_time_check_talents > 0.1f)
            {
                cur_time_check_talents = 0;

                int sum = 0;

                for (int i = 0; i < TalentBottonsList.Count; i++)
                {
                    sum = sum + TalentBottonsList[i].GetCurrentTalentPoint();
                }

                if (CurrentTalentsCount != sum)
                {
                    CheckNormalTalentDisp();
                }

                CurrentTalentsCount = sum;

                UsedTalentsInfo.text = sum.ToString() + $"/{MaxTalents}";

                if (sum >= MaxTalents)
                {
                    isNoTalentPointAvailable = true;
                }
                else
                {
                    isNoTalentPointAvailable = false;
                }
            }
            else
            {
                cur_time_check_talents += Time.deltaTime;
            }
        }


        //===================================================

        

        if (Input.GetMouseButtonDown(0) || Input.touchCount>0) //
        {

        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.tag != "spells")
            {
                //spell not touched
                if (SpellAttrInside.localScale.x == 1f)
                {
                    //SpellAttrInside.localScale = new Vector3(0.8f, 0.8f, 1);
                }

                for (int i = 0; i < AllCurrentSpells.Count; i++)
                {
                    if (AllCurrentSpells[i].IsPressed())
                    {
                        AllCurrentSpells[i].PressedOff();
                    }
                }

                CloseSpellBook();
                
            }

            if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.tag != "hints")
            {
                podskazka.SetActive(false);
            }


            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.tag == "spells")
                {
                    

                    for (int i = 0; i < AllCurrentSpells.Count; i++)
                    {
                        if (EventSystem.current.currentSelectedGameObject.name == AllCurrentSpells[i].GetThisSpellName())
                        {
                            for (int iii = 0; iii < AllCurrentSpells.Count; iii++)
                            {
                                if (EventSystem.current.currentSelectedGameObject.name == AllCurrentSpells[iii].GetThisSpellName())
                                {
                                    print("yes ccurr - " + EventSystem.current.currentSelectedGameObject.name);
                                    AllCurrentSpells[iii].PressedAction();
                                    SpellCheckedInCurrentSpellsName = AllCurrentSpells[iii].GetName();
                                }
                                else
                                {
                                    AllCurrentSpells[iii].PressedOff();
                                }
                            }
                            break;
                        }
                    }

                    

                    for (int i = 0; i < AllSpellBookSpells.Count; i++)
                    {
                        if (EventSystem.current.currentSelectedGameObject.name == AllSpellBookSpells[i].GetThisSpellName())
                        {
                            for (int iii = 0; iii < AllSpellBookSpells.Count; iii++)
                            {
                                if (EventSystem.current.currentSelectedGameObject.name == AllSpellBookSpells[iii].GetThisSpellName())
                                {
                                    print("yes sp book - " + EventSystem.current.currentSelectedGameObject.name);
                                    AllSpellBookSpells[iii].PressedAction();
                                    SpellCheckedInSpellBook = AllSpellBookSpells[iii].GetSpellNumber();
                                }
                                else
                                {
                                    AllSpellBookSpells[iii].PressedOff();
                                }
                            }
                            break;
                        }
                    }

                    

                    if (SpellAttrInside.localScale.x == 0.8f)
                    {
                        //SpellAttrInside.localScale = new Vector3(1f, 1f, 1);
                    }
                    OpenSpellBook();

                } 


                print(EventSystem.current.currentSelectedGameObject.name);
                fortesting.text = EventSystem.current.currentSelectedGameObject.name;

                if (Talents.gameObject.activeSelf)
                {
                    
                    bool result = CheckIfNameEqual(EventSystem.current.currentSelectedGameObject.name);
                    
                }

                

                switch (EventSystem.current.currentSelectedGameObject.name)
                {
                    case "herobutton":
                        Hero.gameObject.SetActive(true);                        
                        Talents.gameObject.SetActive(false);
                        PVP.gameObject.SetActive(false);
                        options.gameObject.SetActive(false);

                        GetCharDataToView();

                        ChangeCanvasButton(true, false, false, false);
                        break;

                    case "talentsbutton":
                        Hero.gameObject.SetActive(false);
                        Talents.gameObject.SetActive(true);
                        PVP.gameObject.SetActive(false);
                        options.gameObject.SetActive(false);
                        //SpellBook.SetActive(false);
                        CloseSpellBook();
                        CheckNormalTalentDisp();
                        
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

                if (EventSystem.current.currentSelectedGameObject != null && (EventSystem.current.currentSelectedGameObject.tag == "hints" || EventSystem.current.currentSelectedGameObject.tag == "spells"))
                {
                    if (EventSystem.current.currentSelectedGameObject.tag == "hints")
                        StartCoroutine(GetHintOver(EventSystem.current.currentSelectedGameObject.name, new Vector2(EventSystem.current.currentSelectedGameObject.transform.position.x, EventSystem.current.currentSelectedGameObject.transform.position.y), "hints"));
                                                            
                } 
                
            } 
            
        }

        if (isStartWaitingPVP)
        {
            if (cur_time>WaitingTimeForPing)
            {
                print(general.CurrentTicket + " ============" + general.CharacterName);
                cur_time = 0;
                //string result = sr.SendAndGetOnlySetup("4~0~" + general.CurrentTicket + "~" + general.CharacterName);
                string result = connection.SendAndGetTCP($"{general.PacketID}~4~0~{general.CurrentTicket}~{general.CharacterName}", general.Ports.tcp2326, general.SetupServerIP, true);
                print(result);

                string[] getstr = result.Split('~');

                int status = int.Parse(getstr[2]);
                string ticket = getstr[3];
                //string session = getstr[4];
                //string hub_data = getstr[5];

                if (status == 0 && (ticket=="nst" || ticket == "wds" || ticket == "nsc"))
                {
                    try
                    {
                        if (general.PacketID != null && general.PacketID != "")
                        {
                            connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2326, general.SetupServerIP, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        print(ex);
                    }
                    
                    SceneManager.LoadScene("player_setup");
                }

                if (status == 2)
                {
                    
                    print("GGGGGGGEEEEEEEETTTTTT      RRRRRRRREEEEEAAAADDDDDDDDYYYYYYYYY");
                }
                               
                if (status == 3)
                {
                    isStartWaitingPVP = false;
                    //answer 4~0~status~ticket~session~HUB
                    //status 0 - nothng good, 3 - OK sending data and play

                    general.SessionTicket = getstr[4];
                    general.CurrentTicket = ticket;
                    general.SessionPlayerID = ticket;
                    general.SessionNumberOfPlayers = 2;                    
                    general.GameServerIP = general.HUB1_ip;

                    switch (getstr[5])
                    {
                        case "1":
                            
                            general.GameServerIP = general.HUB1_ip;
                            break;
                    }
                    
                    print("OOOOOOOOOOOKKKKKKKKKKKKKK  new ticket - " + general.SessionPlayerID + "    new session - " + general.SessionTicket);

                    /*
                    try
                    {
                        if (general.PacketID != null && general.PacketID != "")
                        {
                            connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2326, general.SetupServerIP, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        print(ex);
                    }
                    */


                    SceneManager.LoadScene("SampleScene");
                    
                }


            } 
            else
            {
                cur_time += Time.deltaTime;
            }
            

        }

        

    }

    private string FromArrToStringTalents(int[,] TalentsSpread)
    {
        string result = "0-0-0,0-0-0,0-0,0-0-0,0-0-0,0-0,0-0-0,0-0-0";

        result = TalentsSpread[0, 0] + "-" + TalentsSpread[0, 1] + "-" + TalentsSpread[0, 2] + "," +
            TalentsSpread[1, 0] + "-" + TalentsSpread[1, 1] + "-" + TalentsSpread[1, 2] + "," +
            TalentsSpread[2, 0] + "-" + TalentsSpread[2, 1] + "," +
            TalentsSpread[3, 0] + "-" + TalentsSpread[3, 1] + "-" + TalentsSpread[3, 2] + "," +
            TalentsSpread[4, 0] + "-" + TalentsSpread[4, 1] + "-" + TalentsSpread[4, 2] + "," +
            TalentsSpread[5, 0] + "-" + TalentsSpread[5, 1] + "," +
            TalentsSpread[6, 0] + "-" + TalentsSpread[6, 1] + "-" + TalentsSpread[6, 2] + "," +
            TalentsSpread[7, 0] + "-" + TalentsSpread[7, 1] + "-" + TalentsSpread[7, 2];


        return result;
    }
    
    private void FromStringToArrTalents(out int [,] TalentsSpread, string talents_string)
    {
        TalentsSpread = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
        string[] get_talents = talents_string.Split(',');
        List<string[]> Rows = new List<string[]>();

        for (int i=0; i< get_talents.Length; i++)
        {
            string[] current_row = get_talents[i].Split('-');
            Rows.Add(current_row);
        }

        TalentsSpread[0, 0] = int.Parse( Rows[0].GetValue(0).ToString());
        TalentsSpread[0, 1] = int.Parse(Rows[0].GetValue(1).ToString());
        TalentsSpread[0, 2] = int.Parse(Rows[0].GetValue(2).ToString());

        TalentsSpread[1, 0] = int.Parse(Rows[1].GetValue(0).ToString());
        TalentsSpread[1, 1] = int.Parse(Rows[1].GetValue(1).ToString());
        TalentsSpread[1, 2] = int.Parse(Rows[1].GetValue(2).ToString());

        TalentsSpread[2, 0] = int.Parse(Rows[2].GetValue(0).ToString());
        TalentsSpread[2, 1] = int.Parse(Rows[2].GetValue(1).ToString());

        TalentsSpread[3, 0] = int.Parse(Rows[3].GetValue(0).ToString());
        TalentsSpread[3, 1] = int.Parse(Rows[3].GetValue(1).ToString());
        TalentsSpread[3, 2] = int.Parse(Rows[3].GetValue(2).ToString());

        TalentsSpread[4, 0] = int.Parse(Rows[4].GetValue(0).ToString());
        TalentsSpread[4, 1] = int.Parse(Rows[4].GetValue(1).ToString());
        TalentsSpread[4, 2] = int.Parse(Rows[4].GetValue(2).ToString());

        TalentsSpread[5, 0] = int.Parse(Rows[5].GetValue(0).ToString());
        TalentsSpread[5, 1] = int.Parse(Rows[5].GetValue(1).ToString());

        TalentsSpread[6, 0] = int.Parse(Rows[6].GetValue(0).ToString());
        TalentsSpread[6, 1] = int.Parse(Rows[6].GetValue(1).ToString());
        TalentsSpread[6, 2] = int.Parse(Rows[6].GetValue(2).ToString());

        TalentsSpread[7, 0] = int.Parse(Rows[7].GetValue(0).ToString());
        TalentsSpread[7, 1] = int.Parse(Rows[7].GetValue(1).ToString());
        TalentsSpread[7, 2] = int.Parse(Rows[7].GetValue(2).ToString());

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

    IEnumerator GetHintOver(string HintLog, Vector2 coords, string tagg)
    {
        podskazka.SetActive(true);

        switch (tagg) {
            case "hints":
                podskazka.transform.position = new Vector3(coords.x - 50, coords.y + 10);
                break;
            case "spells":
                podskazka.transform.position = new Vector3(coords.x - 100, coords.y + 30);
                break;

        }

        
        if (tagg == "spells")
        {
            //HintText.text = DB.GetSpellByNumber(int.Parse(HintLog)).Spell1_full_description;
            //HintText.text = "<b>" + DB.GetSpellByNumber(int.Parse(HintLog)).Spell1_name + "</b>\n<size=40%>    <size=100%>\n" + DB.GetSpellByNumber(int.Parse(HintLog)).Spell1_full_description + "\n<size=40%>    <size=100%>\nenergy: " + DB.GetSpellByNumber(int.Parse(HintLog)).Spell_manacost.ToString("f0");

        } 
        else if (tagg == "hints")
        {
            switch (HintLog)
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


public class SpellsButton : MonoBehaviour
{
    public GameObject WholeButton;
    public Button MainSpellButton;
    
    private int SpellNumber;
    
    

    public SpellsButton(int Spell, Vector2 coords, string Place)
    {
        WholeButton = Instantiate(Resources.Load<GameObject>("prefabs/SpellButton"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find(Place).transform);
        WholeButton.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(coords.x, coords.y, 0);
        WholeButton.name = Spell.ToString();

        MainSpellButton = WholeButton.GetComponent<Button>();
        MainSpellButton.name = Spell.ToString();
        MainSpellButton.image.sprite = DB.GetSpellByNumber(Spell).Spell1_icon;
        

        //hint = WholeButton.transform.GetChild(1).GetComponent<Image>();
        //HintText = WholeButton.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        //manacost = WholeButton.gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        //HintText.text = "<b>" + DB.GetSpellByNumber(Spell).Spell1_name + "</b>\n<size=40%>    <size=100%>\n" + DB.GetSpellByNumber(Spell).Spell1_full_description;
        //manacost.text = "energy: " + DB.GetSpellByNumber(Spell).Spell_manacost.ToString();

        SpellNumber = Spell;
        //HideDescription();
    }

    /*
    public bool isHintShowing()
    {
        return isHintShow;
    }
    */

    public void SetNewSpell(int SpellNumb)
    {
        MainSpellButton.image.sprite = DB.GetSpellByNumber(SpellNumb).Spell1_icon;
        SpellNumber = SpellNumb;
        WholeButton.name = SpellNumb.ToString();
        MainSpellButton.name = SpellNumb.ToString();
    }

    public string GetName()
    {
        return MainSpellButton.name;
    }
       
    public int GetSpellNumber()
    {
        return SpellNumber;
    }

    public Sprite GetSpellImage()
    {
        return MainSpellButton.image.sprite;
    }

}



public class TalentsButton : MonoBehaviour
{
    public GameObject WholeButtonImage;
    private Button MainThemeImage;
    private TextMeshProUGUI TalentsNumbers;
    private int MaxTalents;
    private int TalentNumber;
    private int CurrentTalents;
    private bool isActive;
    private Image nonavailable;

    public TalentsButton(int TalentNumb, Sprite MainTheme, int CurrTalents, int MTalents, string TalentName, Vector2 coords, float sizef)
    {
        WholeButtonImage = Instantiate(Resources.Load<GameObject>("prefabs/point"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("talents").transform);
        WholeButtonImage.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(coords.x, coords.y, 0);

        MainThemeImage = WholeButtonImage.transform.GetChild(0).GetComponent<Button>();
        MainThemeImage.image.sprite = MainTheme;
        WholeButtonImage.name = TalentName;
        MainThemeImage.name = TalentName;
        nonavailable = WholeButtonImage.transform.GetChild(3).GetComponent<Image>();
        MainThemeImage.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(sizef, sizef);

        

        CurrentTalents = CurrTalents;
        TalentNumber = TalentNumb;
        MaxTalents = MTalents;

        isActive = true;
        nonavailable.gameObject.SetActive(false);

        TalentsNumbers = WholeButtonImage.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        GetCurrTalents();

    }



    public string GetName()
    {
        return MainThemeImage.name;
    }

    public void MakeInactive()
    {
        CurrentTalents = 0;
        GetCurrTalents();
        //MainThemeImage.interactable = false;
        Color curcolor = MainThemeImage.image.color;
        curcolor.a = 0.4f;
        MainThemeImage.image.color = curcolor;

        isActive = false;
    }

    public void MakeActive()
    {
        //MainThemeImage.interactable = true;
        Color curcolor = MainThemeImage.image.color;
        curcolor.a = 1f;
        MainThemeImage.image.color = curcolor;
        isActive = true;
    }

    public int GetTalentNumber()
    {
        return TalentNumber;
    }

    public void ResetTalents()
    {
        CurrentTalents = 0;
        GetCurrTalents();
    }

    public void NonAvailable()
    {
        MakeInactive();
        //MainThemeImage.interactable = false;
        nonavailable.gameObject.SetActive(true);
    }

    public void Available()
    {
        MakeActive();
        //MainThemeImage.interactable = false;
        nonavailable.gameObject.SetActive(false);
    }

    public void AddTalentPoint()
    {
        if (isActive && !nonavailable.gameObject.activeSelf)
        {
            if (CurrentTalents == MaxTalents)
            {
                CurrentTalents = 0;
                GetCurrTalents();
            }
            else
            {
                CurrentTalents++;
                GetCurrTalents();
            }
        }
    }

    public void RemoveTalentPoint()
    {
        if (isActive && !nonavailable.gameObject.activeSelf)
        {
            if (CurrentTalents == 0)
            {
                CurrentTalents = 0;
                GetCurrTalents();
            }
            else
            {
                CurrentTalents--;
                GetCurrTalents();
            }
        }
    }

    public string GetCurrentTalentPointString()
    {
        return CurrentTalents.ToString();
    }

    public int GetCurrentTalentPoint()
    {
        return CurrentTalents;
    }

    private void GetCurrTalents()
    {
        TalentsNumbers.text = CurrentTalents + "/" + MaxTalents;
    }


    

}


