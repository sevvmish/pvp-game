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
        if (!isPressed)
        {
            isPressed = true;
            _mainObjectRect.localScale = new Vector3(1.05f, 1.05f, 1);
            blink_effect.SetActive(true);
        } 
        else
        {
            isPressed = false;
            _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
            blink_effect.SetActive(false);
        }
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
