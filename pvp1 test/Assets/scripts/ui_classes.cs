using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ui_classes : MonoBehaviour
{
    
}

public class SpellDescription : MonoBehaviour
{
    private GameObject _mainObject, blink_effect, blink_outline;
    private RectTransform _mainObjectRect;
    private spellsIDs CurrentSpell;
    private int SpellNumber;
    private Image SpellIcon;
    private string _name;
    private TextMeshProUGUI SpellDescr, ManaDescr;
    private bool isPressed, isOnlyNames;


    public SpellDescription(int _spell_number, Vector2 _place_coords, Transform _where_to_add, string curr_name, bool _isonlynames)
    {
        _mainObject = Instantiate(Resources.Load<GameObject>("prefabs/SpellDescription"), _place_coords, Quaternion.identity, _where_to_add);
        _mainObjectRect = _mainObject.GetComponent<RectTransform>();
        _mainObjectRect.anchoredPosition = _place_coords;
        _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
        blink_effect = _mainObject.transform.GetChild(0).gameObject;
        blink_outline = _mainObject.transform.GetChild(5).gameObject;
        _name = curr_name;
        _mainObject.name = _name;
        isOnlyNames = _isonlynames;
        SpellNumber = _spell_number;
        SpellIcon = _mainObject.transform.GetChild(1).GetComponent<Image>();
        SpellDescr = _mainObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        ManaDescr = _mainObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        CurrentSpell = DB.GetSpellByNumber(_spell_number);
        BlinkOFF();
        SetNewSpell();
    }

    public void SetNewSpell(int _spell_number)
    {
        if (_spell_number != SpellNumber)
        {
           
            _mainObject.GetComponent<Animator>().Play("UI effect - appear");
        }

        CurrentSpell = DB.GetSpellByNumber(_spell_number);
        SpellNumber = _spell_number;
        SetNewSpell();
    }

    private void BlinkON()
    {
        blink_effect.SetActive(true);
        blink_outline.SetActive(true);
    }

    private void BlinkOFF()
    {
        blink_effect.SetActive(false);
        blink_outline.SetActive(false);
    }


    public string GetName()
    {
        return _name;
    }

    private void SetNewSpell()
    {
        if (isOnlyNames)
        {
            SpellIcon.sprite = CurrentSpell.Spell1_icon;
            SpellDescr.text = CurrentSpell.Spell1_name;
            SpellDescr.fontSize = 28;
            SpellDescr.fontStyle = FontStyles.Bold;
            ManaDescr.text = lang.ManaCostText + " " + CurrentSpell.Spell_manacost.ToString("f0") + " \t";
            ManaDescr.fontSize = 22;
        } 
        else
        {
            SpellIcon.sprite = CurrentSpell.Spell1_icon;
            SpellDescr.text = CurrentSpell.Spell1_full_description;
            SpellDescr.fontSize = 18;
            ManaDescr.text = lang.ManaCostText + " " + CurrentSpell.Spell_manacost.ToString("f0") + " \t";
            ManaDescr.fontSize = 17;
        }
    }

    public int GetSpellNumber()
    {
        return SpellNumber;
    }

    public string GetThisSpellName()
    {
        return _name;
    }

    public void PressedAction()
    {        
        isPressed = true;
        _mainObjectRect.localScale = new Vector3(1.05f, 1.05f, 1);
        BlinkON();
    }

    public void PressedOff()
    {   
        isPressed = false;
        _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
        BlinkOFF();
    }

    public bool IsPressed()
    {
        return isPressed;
    }

}


public class TalentsButton : MonoBehaviour
{
    public GameObject WholeButtonImage;
    private Button MainThemeImage;
        
    private int TalentNumber;
    private int CurrentTalents;
    private bool isActive, isPressed;
    private Image nonavailable, IfPressedColor, BackGroungImg;
    private Animator animation;

    public enum TalentTypes
    {
        simple = 0,
        meduim = 1,
        super = 2
    }

    public TalentsButton(int TalentNumb, Sprite MainTheme, int CurrTalents, string TalentName, Vector2 coords, TalentTypes _current_type )
    {
        WholeButtonImage = Instantiate(Resources.Load<GameObject>("prefabs/TalentDescription"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("talents").transform);
        WholeButtonImage.gameObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(coords.x, coords.y, 0);
        
        animation = WholeButtonImage.GetComponent<Animator>();
                
        MainThemeImage = WholeButtonImage.transform.GetChild(1).GetComponent<Button>();
        MainThemeImage.image.sprite = MainTheme;
        WholeButtonImage.name = TalentName;
        MainThemeImage.name = TalentName;
        nonavailable = WholeButtonImage.transform.GetChild(2).GetComponent<Image>();
        BackGroungImg = WholeButtonImage.transform.GetChild(0).GetComponent<Image>();
        
        

        IfPressedColor = WholeButtonImage.transform.GetChild(3).GetComponent<Image>();
        CurrentTalents = CurrTalents;
        TalentNumber = TalentNumb;

        isActive = true;
        nonavailable.gameObject.SetActive(false);
        IfPressedColor.gameObject.SetActive(false);

        switch ((int)_current_type)
        {
            case 0:
                BackGroungImg.sprite = Resources.Load<Sprite>("sprites/simple circle");
                IfPressedColor.sprite = Resources.Load<Sprite>("sprites/circle outliner");
                BackGroungImg.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                IfPressedColor.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                break;
            case 1:
                BackGroungImg.sprite = Resources.Load<Sprite>("sprites/romb square");
                IfPressedColor.sprite = Resources.Load<Sprite>("sprites/romb square outliner");
                BackGroungImg.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1);
                IfPressedColor.gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1);
                break;
            case 2:

                break;
        }

        if (CurrentTalents>0)
        {
            isPressed = true;
            IfPressedColor.gameObject.SetActive(true);
        }

        

    }

    public void AppearEffect()
    {
        animation.Play("appear");
    }

    public string GetName()
    {
        return MainThemeImage.name;
    }

    public void MakeInactive()
    {
        CurrentTalents = 0;
        isPressed = false;
        IfPressedColor.gameObject.SetActive(false);

        Color curcolor = MainThemeImage.image.color;
        curcolor.a = 0.2f;
        MainThemeImage.image.color = curcolor;

        Color curcolor1 = IfPressedColor.color;
        curcolor1.a = 0.2f;
        IfPressedColor.color = curcolor1;

        Color curcolor2 = BackGroungImg.color;
        curcolor2.a = 0.2f;
        BackGroungImg.color = curcolor2;

        
        isActive = false;
    }

    public void MakeActive()
    {
        //MainThemeImage.interactable = true;
        Color curcolor = MainThemeImage.image.color;
        curcolor.a = 1f;
        MainThemeImage.image.color = curcolor;

        Color curcolor1 = IfPressedColor.color;
        curcolor1.a = 1f;
        IfPressedColor.color = curcolor1;

        Color curcolor2 = BackGroungImg.color;
        curcolor2.a = 1f;
        BackGroungImg.color = curcolor2;

        isActive = true;
    }

    public int GetTalentNumber()
    {
        return TalentNumber;
    }

    public void ResetTalents()
    {
        CurrentTalents = 0;
        isPressed = false;
        IfPressedColor.gameObject.SetActive(false);
        
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
            if (CurrentTalents == 1)
            {
                CurrentTalents = 0;
                isPressed = false;
                IfPressedColor.gameObject.SetActive(false);
                
            }
            else
            {
                CurrentTalents = 1;
                isPressed = true;
                IfPressedColor.gameObject.SetActive(true);
                
                animation.Play("tremor");
                
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
                
            }
            else
            {
                CurrentTalents--;
                
            }
            isPressed = false;
            IfPressedColor.gameObject.SetActive(false);
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

    
}

public class PVPStatisticsPanel: MonoBehaviour
{
    //private GameObject WholePanel_left, WholePanel_right;
    private int HowManyPlayers, GameType, MaxPlayersInTeam;
    private string[] data;
    private List<_players_in_pvp> ListOfPlayers = new List<_players_in_pvp>();
    private struct _players_in_pvp
    {
        public string player_name;
        public int player_class;
        public int player_score;
        public int player_teamID;

        public _players_in_pvp(string _name, int _pl_class, int _teamID, int _score)
        {
            player_name = _name;
            player_class = _pl_class;
            player_teamID = _teamID;
            player_score = _score;
        }
    }
    public PVPStatisticsPanel(string _data, Transform _where_to_locate)
    {
        //parsing   0~7~game type~player_count~name-player_class-player_teamID-score~...
        //example 0~7~0~5~mage-2-0-0~barbarian-3-1-1~warrior-1-2-1~rogue-4-3-1~wizard-5-4-1~

        try
        {
            data = _data.Split('~');
            GameType = int.Parse(data[2]);
            HowManyPlayers = int.Parse(data[3]);

            for (int i = 0; i < HowManyPlayers; i++)
            {
                string[] _temp_data = data[4 + i].Split('-');
                
                ListOfPlayers.Add(new _players_in_pvp(_temp_data[0], int.Parse(_temp_data[1]), int.Parse(_temp_data[2]), int.Parse(_temp_data[3])));
            }
        }
        catch (System.Exception ex)
        {
            print(ex);
            return;
        }
        
        
        if (GameType==0)
        {
            int team_left_ID = ListOfPlayers[0].player_teamID;

            List<_players_in_pvp> team_left_players = new List<_players_in_pvp>();
            List<_players_in_pvp> team_right_players = new List<_players_in_pvp>();

            for (int i = 0; i < ListOfPlayers.Count; i++)
            {
                if (ListOfPlayers[i].player_teamID== team_left_ID)
                {
                    team_left_players.Add(ListOfPlayers[i]);
                } else
                {
                    team_right_players.Add(ListOfPlayers[i]);
                }
            }

            if (team_left_players.Count> team_right_players.Count)
            {
                MaxPlayersInTeam = team_left_players.Count;
            } else
            {
                MaxPlayersInTeam = team_right_players.Count;
            }

            PVPDoublePanelControl(new Vector2(-194,0), _where_to_locate, team_left_players, MaxPlayersInTeam);
            PVPDoublePanelControl(new Vector2( 194,0), _where_to_locate, team_right_players, MaxPlayersInTeam);
        }

        /*
        WholePanel_left = Instantiate(Resources.Load<GameObject>("prefabs/PVPStatisticsPanel"), new Vector3(0, 0, 0), Quaternion.identity, _where_to_locate);
        WholePanel_left.SetActive(true);
        WholePanel_left.GetComponent<RectTransform>().anchoredPosition = new Vector2(-194, 0);

        WholePanel_right = Instantiate(Resources.Load<GameObject>("prefabs/PVPStatisticsPanel"), new Vector3(0, 0, 0), Quaternion.identity, _where_to_locate);
        WholePanel_right.SetActive(true);
        WholePanel_right.GetComponent<RectTransform>().anchoredPosition = new Vector2(194, 0);
        */
        
    }

    private void PVPDoublePanelControl(Vector2 _location, Transform _where_to_locate, List<_players_in_pvp> current_players, int _size)
    {
        int score = current_players[0].player_score;
        int team = current_players[0].player_teamID;

        GameObject WholePanel = Instantiate(Resources.Load<GameObject>("prefabs/PVPStatisticsPanel"), new Vector3(0, 0, 0), Quaternion.identity, _where_to_locate);
        WholePanel.SetActive(true);
        WholePanel.GetComponent<RectTransform>().anchoredPosition = _location;

        GameObject panel = WholePanel.transform.GetChild(0).gameObject;
        print(panel.GetComponent<RectTransform>().sizeDelta.x + " - " + WholePanel.GetComponent<RectTransform>().sizeDelta.y);
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 150 +(_size-1)*45);

        panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{lang.PVPStatsTeamName} {team}:";
        panel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = score.ToString();
        panel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = lang.PVPStatsPlayerName;
        panel.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = lang.PVPStatsPlayerScore;
               
        for (int i = 0; i < current_players.Count; i++)
        {
            new PlayerLine(panel.transform.GetChild(5).gameObject, panel.transform, new Vector2(-5, -110 - 40 * i), current_players[i].player_class, current_players[i].player_name, current_players[i].player_score);
        }

        panel.transform.GetChild(5).gameObject.SetActive(false);

    }



    private class PlayerLine
    {
        public PlayerLine(GameObject _form, Transform _where_to_locate, Vector2 _location, int _player_class, string _player_name, int _player_score)
        {
            GameObject PlayerL = Instantiate(_form, Vector2.zero, Quaternion.identity, _where_to_locate);
            PlayerL.SetActive(true);
            PlayerL.GetComponent<RectTransform>().anchoredPosition = _location;

            PlayerL.transform.GetChild(1).GetComponent<Image>().sprite = DB.get_logo_by_number(_player_class);
            PlayerL.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _player_name;
            PlayerL.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _player_score.ToString();

        }
    }

}
