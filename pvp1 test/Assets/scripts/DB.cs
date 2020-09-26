﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB : MonoBehaviour
{
    public static spellsIDs spell1 = new spellsIDs(lang.Spell1Name, 1, Resources.Load<Sprite>("sprites/spell1"), lang.Spell1ShortDescri, lang.Spell1FullDescri, spellsIDs.spell_types.direct_melee);
    public static spellsIDs spell2 = new spellsIDs(lang.Spell2Name, 2, Resources.Load<Sprite>("sprites/spell2"), lang.Spell2ShortDescri, lang.Spell2FullDescri, spellsIDs.spell_types.DOT_melee);
    public static spellsIDs spell3 = new spellsIDs(lang.Spell3Name, 3, Resources.Load<Sprite>("sprites/spell3"), lang.Spell3ShortDescri, lang.Spell3FullDescri, spellsIDs.spell_types.positive_buff);

    public static spellsIDs GetSpellByNumber(int SpellNumber)
    {
        spellsIDs result = spell1;
        switch(SpellNumber)
        {         
            case 1:
                result = spell1;
                break;
            case 2:
                result = spell2;
                break;
            case 3:
                result = spell3;
                break;
        }

        return result;
    }

}

public struct spellsIDs
{
    public enum spell_types
    {
        direct_melee = 1,
        direct_magic,
        DOT_melee,
        DOT_magic,
        positive_buff
    }

    public string Spell1_name;
    public int Spell1_number;
    public Sprite Spell1_icon;
    public string Spell1_short_description;
    public string Spell1_full_description;
    public spell_types spell_type;

    public spellsIDs(string name, int number, Sprite sprt, string descri_short, string descri_full, spell_types spell_ty)
    {
        Spell1_name = name;
        Spell1_number = number;
        Spell1_icon = sprt;
        Spell1_short_description = descri_short;
        Spell1_full_description = descri_full;
        spell_type = spell_ty;
    }
}

public static class lang
{
    //envir===================================
    public static string Canceled = "ОТМЕНА";


    //all spells===============================
    public static string Spell1Name = "простой удар";
    public static string Spell1ShortDescri = "простой удар одноручным оружием";
    public static string Spell1FullDescri = "простой удар одноручным оружием";

    public static string Spell2Name = "кровотечение";
    public static string Spell2ShortDescri = "легкое повреждение в течение 5 сек";
    public static string Spell2FullDescri = "наносит легкое повреждение каждую секунду в течение 5 секунд";

    public static string Spell3Name = "крепкое здоровье";
    public static string Spell3ShortDescri = "здоровье +20%, восстановление жизни +1 на 60 сек";
    public static string Spell3FullDescri = "увеличивает запас здоровья на 20% и регенерацию здоровья на 1 на 60 сек";

    public static string Spell4Name = "удар щитом";
    public static string Spell4ShortDescri = "удар щитом и оглушение на 3 сек";
    public static string Spell4FullDescri = "удар щитом наносит легкое повреждение и оглушает противника на 3 секунды";

    public static string Spell5Name = "за щитом";
    public static string Spell5ShortDescri = "блок всех атак на 5 сек";
    public static string Spell5FullDescri = "блокирует все прямые удары и заклинания в течение 5 сек, но понижает скорость передвижения";
}

public static class UNlang
{
    //all spells===============================
    public static string Spell1Name = "simple hit";
    public static string Spell1ShortDescri = "simple hit with a 1H weapon";
    public static string Spell1FullDescri = "simple hit with a 1H weapon";

    public static string Spell2Name = "bleeding";
    public static string Spell2ShortDescri = "light damage for 5 sec";
    public static string Spell2FullDescri = "deal light damage every second for 5 seconds";

    public static string Spell3Name = "strong health";
    public static string Spell3ShortDescri = "health +20%, HP regen +1 for 60 sec";
    public static string Spell3FullDescri = "increases HP amount for 20% and HP regen for 1 for 60 seconds";

    public static string Spell4Name = "shield slam";
    public static string Spell4ShortDescri = "slam with shield and stuns for 3 sec";
    public static string Spell4FullDescri = "shield slam deal light damage and stuns enemy for 3 seconds";

    public static string Spell5Name = "shield on";
    public static string Spell5ShortDescri = "blocks all attacks for 5 sec";
    public static string Spell5FullDescri = "blocks all direct hits and spells for 5 sec but slows movement speed";
}

