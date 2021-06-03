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
    private GameObject _mainObject, blink_effect;
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
        blink_effect = _mainObject.transform.GetChild(4).gameObject;
        _name = curr_name;
        _mainObject.name = _name;
        isOnlyNames = _isonlynames;
        SpellNumber = _spell_number;
        SpellIcon = _mainObject.transform.GetChild(0).GetComponent<Image>();
        SpellDescr = _mainObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        ManaDescr = _mainObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        CurrentSpell = DB.GetSpellByNumber(_spell_number);
        SetNewSpell();
    }

    public void SetNewSpell(int _spell_number)
    {
        CurrentSpell = DB.GetSpellByNumber(_spell_number);
        SpellNumber = _spell_number;
        SetNewSpell();
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
            ManaDescr.text = lang.ManaCostText + " " + CurrentSpell.Spell_manacost.ToString("f0");
            ManaDescr.fontSize = 22;
        } else
        {
            SpellIcon.sprite = CurrentSpell.Spell1_icon;
            SpellDescr.text = CurrentSpell.Spell1_full_description;
            SpellDescr.fontSize = 18;
            ManaDescr.text = lang.ManaCostText + " " + CurrentSpell.Spell_manacost.ToString("f0");
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
        blink_effect.SetActive(true);       
    }

    public void PressedOff()
    {   
        isPressed = false;
        _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
        blink_effect.SetActive(false);
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
