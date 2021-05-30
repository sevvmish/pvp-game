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
    private GameObject _mainObject;
    private RectTransform _mainObjectRect;
    private spellsIDs CurrentSpell;
    private int SpellNumber;
    private Image SpellIcon;
    private string _name;
    private TextMeshProUGUI SpellDescr, ManaDescr;
    private bool isPressed;


    public SpellDescription(int _spell_number, Vector2 _place_coords, Transform _where_to_add, string curr_name)
    {
        _mainObject = Instantiate(Resources.Load<GameObject>("prefabs/SpellDescription"), _place_coords, Quaternion.identity, _where_to_add);
        _mainObjectRect = _mainObject.GetComponent<RectTransform>();
        _mainObjectRect.anchoredPosition = _place_coords;
        _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
        _name = curr_name;
        _mainObject.name = _name;

        SpellNumber = _spell_number;
        SpellIcon = _mainObject.transform.GetChild(0).GetComponent<Image>();
        SpellDescr = _mainObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        ManaDescr = _mainObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        CurrentSpell = DB.GetSpellByNumber(_spell_number);
        SetNewSpell();
    }

    private void SetNewSpell()
    {
        SpellIcon.sprite = CurrentSpell.Spell1_icon;
        SpellDescr.text = CurrentSpell.Spell1_full_description;
        ManaDescr.text = lang.ManaCostText + " " + CurrentSpell.Spell_manacost.ToString("f0");
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
        } 
        else
        {
            isPressed = false;
            _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);
        }
    }

    public void PressedOff()
    {   
        isPressed = false;
        _mainObjectRect.localScale = new Vector3(0.95f, 0.95f, 1);        
    }

    public bool IsPressed()
    {
        return isPressed;
    }

}
