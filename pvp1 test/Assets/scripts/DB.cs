
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB : MonoBehaviour
{
    public static spellsIDs spell0 = new spellsIDs("", 0, Resources.Load<Sprite>("sprites/spell0"), "", "", spellsIDs.spell_types.direct_melee, 0);
    //WARRIOR
    public static spellsIDs spell1 = new spellsIDs(lang.Spell1Name, 1, Resources.Load<Sprite>("sprites/spell1"), lang.Spell1ShortDescri, lang.Spell1FullDescri, spellsIDs.spell_types.direct_melee, 10);
    public static spellsIDs spell2 = new spellsIDs(lang.Spell2Name, 2, Resources.Load<Sprite>("sprites/spell2"), lang.Spell2ShortDescri, lang.Spell2FullDescri, spellsIDs.spell_types.DOT_melee, 20);
    public static spellsIDs spell3 = new spellsIDs(lang.Spell3Name, 3, Resources.Load<Sprite>("sprites/spell3"), lang.Spell3ShortDescri, lang.Spell3FullDescri, spellsIDs.spell_types.positive_buff, 10);
    public static spellsIDs spell4 = new spellsIDs(lang.Spell4Name, 4, Resources.Load<Sprite>("sprites/spell4"), lang.Spell4ShortDescri, lang.Spell4FullDescri, spellsIDs.spell_types.direct_melee, 25);
    public static spellsIDs spell5 = new spellsIDs(lang.Spell5Name, 5, Resources.Load<Sprite>("sprites/spell5"), lang.Spell5ShortDescri, lang.Spell5FullDescri, spellsIDs.spell_types.positive_eff, 20);
    public static spellsIDs spell6 = new spellsIDs(lang.Spell6Name, 6, Resources.Load<Sprite>("sprites/spell6"), lang.Spell6ShortDescri, lang.Spell6FullDescri, spellsIDs.spell_types.direct_melee, 20);

    //MAGE
    public static spellsIDs spell51 = new spellsIDs(lang.Spell51Name, 51, Resources.Load<Sprite>("sprites/spell51"), lang.Spell51ShortDescri, lang.Spell51FullDescri, spellsIDs.spell_types.direct_magic, 10);
    public static spellsIDs spell52 = new spellsIDs(lang.Spell52Name, 52, Resources.Load<Sprite>("sprites/spell52"), lang.Spell52ShortDescri, lang.Spell52FullDescri, spellsIDs.spell_types.direct_magic, 10);
    public static spellsIDs spell53 = new spellsIDs(lang.Spell52Name, 53, Resources.Load<Sprite>("sprites/spell52"), lang.Spell52ShortDescri, lang.Spell52FullDescri, spellsIDs.spell_types.direct_magic, 10);
    public static spellsIDs spell54 = new spellsIDs(lang.Spell52Name, 54, Resources.Load<Sprite>("sprites/spell52"), lang.Spell52ShortDescri, lang.Spell52FullDescri, spellsIDs.spell_types.direct_magic, 10);

    //BARBARIAN
    public static spellsIDs spell101 = new spellsIDs(lang.Spell51Name, 101, Resources.Load<Sprite>("sprites/spell51"), lang.Spell51ShortDescri, lang.Spell51FullDescri, spellsIDs.spell_types.direct_melee, 10);


    public static spellsIDs spell1002 = new spellsIDs(lang.Spell1002Name, 1002, Resources.Load<Sprite>("sprites/spell1002"), lang.Spell1002ShortDescri, lang.Spell1002FullDescri, spellsIDs.spell_types.negative_eff,0);

    public static spellsIDs GetSpellByNumber(int SpellNumber)
    {
        spellsIDs result = spell1;
        switch(SpellNumber)
        {
            case 0:
                result = spell0;
                break;
            case 1:
                result = spell1;
                break;
            case 2:
                result = spell2;
                break;
            case 3:
                result = spell3;
                break;
            case 4:
                result = spell4;
                break;
            case 5:
                result = spell5;
                break;
            case 6:
                result = spell6;
                break;
            case 51:
                result = spell51;
                break;
            case 52:
                result = spell52;
                break;
            case 53:
                result = spell53;
                break;
            case 54:
                result = spell54;
                break;
            case 101:
                result = spell101;
                break;
            case 1002:
                result = spell1002;
                break;
        }

        return result;
    }

    public static talentsIDs GetTalentByNumber(int TalentNumber)
    {
        talentsIDs result = talent1;
        switch (TalentNumber)
        {
            
            case 1:
                result = talent1;
                break;
            case 2:
                result = talent2;
                break;
            case 3:
                result = talent3;
                break;
            case 4:
                result = talent4;
                break;
            case 5:
                result = talent5;
                break;
            case 6:
                result = talent6;
                break;
            case 7:
                result = talent7;
                break;
            case 8:
                result = talent8;
                break;
            case 9:
                result = talent9;
                break;
            case 10:
                result = talent10;
                break;
            case 11:
                result = talent11;
                break;
            case 12:
                result = talent12;
                break;
            case 13:
                result = talent13;
                break;
            case 14:
                result = talent14;
                break;
            case 15:
                result = talent15;
                break;
            case 16:
                result = talent16;
                break;
            case 17:
                result = talent17;
                break;
            case 18:
                result = talent18;
                break;
            case 19:
                result = talent19;
                break;
            case 20:
                result = talent20;
                break;
            case 21:
                result = talent21;
                break;
            case 22:
                result = talent22;
                break;

        }

        return result;
    }

    //WARRIOR
    public static talentsIDs talent1 = new talentsIDs(lang.Talent1Name, 1, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent2 = new talentsIDs(lang.Talent1Name, 2, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent3 = new talentsIDs(lang.Talent1Name, 3, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent4 = new talentsIDs(lang.Talent1Name, 4, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent5 = new talentsIDs(lang.Talent1Name, 5, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent6 = new talentsIDs(lang.Talent1Name, 6, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent7 = new talentsIDs(lang.Talent1Name, 7, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent8 = new talentsIDs(lang.Talent1Name, 8, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent9 = new talentsIDs(lang.Talent1Name, 9, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent10 = new talentsIDs(lang.Talent1Name, 10, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent11 = new talentsIDs(lang.Talent1Name, 11, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent12 = new talentsIDs(lang.Talent1Name, 12, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent13 = new talentsIDs(lang.Talent1Name, 13, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent14 = new talentsIDs(lang.Talent1Name, 14, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent15 = new talentsIDs(lang.Talent1Name, 15, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent16 = new talentsIDs(lang.Talent1Name, 16, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent17 = new talentsIDs(lang.Talent1Name, 17, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent18 = new talentsIDs(lang.Talent1Name, 18, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent19 = new talentsIDs(lang.Talent1Name, 19, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent20 = new talentsIDs(lang.Talent1Name, 20, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent21 = new talentsIDs(lang.Talent1Name, 21, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);
    public static talentsIDs talent22 = new talentsIDs(lang.Talent1Name, 22, Resources.Load<Sprite>("sprites/spell1"), lang.Talent1Description);



}




public struct talentsIDs
{
    
    public string Talent_name;
    public int Talent_number;
    public Sprite Talent_icon;
    public string Talent_description;
    

    public talentsIDs(string name, int number, Sprite sprt, string descri)
    {
        Talent_name = name;
        Talent_number = number;
        Talent_icon = sprt;
        Talent_description = descri;
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
        positive_buff,
        positive_eff,
        negative_eff,
        healing
    }

    public string Spell1_name;
    public int Spell1_number;
    public Sprite Spell1_icon;
    public string Spell1_short_description;
    public string Spell1_full_description;
    public spell_types spell_type;
    public float Spell_manacost;

    public spellsIDs(string name, int number, Sprite sprt, string descri_short, string descri_full, spell_types spell_ty, float manacost)
    {
        Spell1_name = name;
        Spell1_number = number;
        Spell1_icon = sprt;
        Spell1_short_description = descri_short;
        Spell1_full_description = descri_full;
        spell_type = spell_ty;
        Spell_manacost = manacost;
    }
}


public static class langu
{
    public static string kkk;

    public static void SetLangu(string whichh)
    {
        if (whichh == "ru")
        {
            kkk = "на русском";
        }

        if (whichh == "eng")
        {
            kkk = "in english";
        }
    }
}


public static class lang
{


    //логин и вызов пользователя
    //public static string LoginText = "";
    public static string LoginText = "ВОЙТИ";
    public static string CreateNewText = "СОЗДАТЬ";
    public static string AwaitingText = "ЗАГРУЗКА...";
    public static string ConnectionErrorText = "НЕТ СОЕДИНЕНИЯ";
    public static string CreateNewLoginPass = "создать новый логин-пароль";
    public static string EnterAsGuest = "войти в игру как гость";

    public static string TheLoginText = "ЛОГИН";
    public static string ThePasswordText = "ПАРОЛЬ";

    public static string SignForNameForEnter = "имя для ввода:";


    public static string wlltext = "неправильная длина логина";
    public static string wdstext = "недопустимые символы";
    public static string wlptext = "неправильная длина пароля";
    public static string uaetext = "такой логин уже существует";
    public static string ecutext = "ошибка при создании логина";
    public static string udetext = "нет такого пользователя";
    public static string wptext = "ошибка в пароле";
    public static string uctext = "пользователь создан";
    public static string egttext = "ошибка в получении тикета";
    public static string errtext = "произошла ошибка";
    public static string conerrtext = "ошибка соединения";
    public static string loadtext = "ЗАГРУЗКА...";
    public static string nsttext = "тикет не существует";
    public static string wcntext = "неправильное имя или пароль";
    public static string caetext = "такое имя уже есть";
    public static string taetext = "такой тип персонажа у вас уже есть";
    public static string nsctext = "такого персонажа нет";
    public static string eittext = "ошибка в талантах";
    public static string nsstext = "такого спела нет в книге";
    public static string rsntext = "спелы повторяются";
    public static string dbetext = "ошибка в БД";

    public static string WarriorText = "ВОИН";
    public static string ElemText = "ЕЛЕМЕНТАЛИСТ";
    public static string BarbarText = "ВАРВАР";
    public static string RogText = "РАЗБОЙНИК";
    public static string WizText = "ВОЛШЕБНИК";

    public static string CreateNewCharacter = "СОЗДАТЬ НОВОГО ГЕРОЯ";
    public static string back = "НАЗАД";

    public static string EnterTheGameText = "ВОЙТИ В ИГРУ";
    public static string ChooseAnotherText = "ВЫБРАТЬ ДРУГОГО ГЕРОЯ";

    public static string SpeedText = "скорость";
    public static string HealthText = "здоровье";
    public static string HealthRegenText = "восст здоровья";
    public static string EnergyRegenText = "восст энергии";
    public static string WeaponAttackText = "атака оружия";
    public static string HitPowerText = "сила удара";
    public static string ArmorText = "броня";
    public static string ShieldBlockText = "блокировка щитом";
    public static string MagicResistanceText = "защита от магии";
    public static string DodgeText = "уклонение";
    public static string CastSpeedText = "скорость заклинания";
    public static string MeleeCritText = "крит ближней атаки";
    public static string MagicCritText = "крит магической атаки";
    public static string SpellPowerText = "сила магии";

    public static string SpeedTextHint = "скорость";
    public static string HealthTextHint = "здоровье";
    public static string HealthRegenTextHint = "восст здоровья";
    public static string EnergyRegenTextHint = "восст энергии";
    public static string WeaponAttackTextHint = "атака оружия";
    public static string HitPowerTextHint = "сила удара";
    public static string ArmorTextHint = "броня";
    public static string ShieldBlockTextHint = "блокировка щитом";
    public static string MagicResistanceTextHint = "защита от магии";
    public static string DodgeTextHint = "уклонение";
    public static string CastSpeedTextHint = "скорость заклинания";
    public static string MeleeCritTextHint = "крит ближней атаки";
    public static string MagicCritTextHint = "крит магической атаки";
    public static string SpellPowerTextHint = "сила магии";

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

    public static string Spell6Name = "серия с щитом";
    public static string Spell6ShortDescri = "серия из трех ударов с растущей силой удара";
    public static string Spell6FullDescri = "серия из трех ударов, сила каждого последующего удара в связке вырастает";

    public static string Spell51Name = "огненный выстрел";
    public static string Spell51ShortDescri = "выстрел огненным шаром";
    public static string Spell51FullDescri = "выстрел огненным шаром, который попадает в первого противника и наносит повреждение в небольшом радиусе";

    public static string Spell52Name = "метеор";
    public static string Spell52ShortDescri = "метеор приземляется в мага, наносит повтерждение и огрушает противников";
    public static string Spell52FullDescri = "метеор приземляется в точку нахождения мага, наносит повреждение и оглушает всех противников по области";

    public static string Spell1002Name = "оглушение";
    public static string Spell1002ShortDescri = "оглушение";
    public static string Spell1002FullDescri = "невозможно двигаться, атаковать и произносить заклинания";

    //ТАЛАНТЫ======================
    public static string Talent1Name = "оглушение";
    public static string Talent1Description = "оглушение gvbrffgd egerg  egref";
}

public static class ENlang
{

    //логин и вызов пользователя
    //public static string LoginText = "";
    public static string LoginText = "LOGIN";
    public static string CreateNewText = "CREATE";
    public static string AwaitingText = "LOADING...";
    public static string ConnectionErrorText = "CONNECTION ERROR";

    public static string TheLoginText = "LOGIN";
    public static string ThePasswordText = "PASSWORD";

    public static string SignForNameForEnter = "name to enter:";

    public static string wlltext = "wrong length for login";
    public static string wlptext = "wrong length for password";
    public static string uaetext = "login allready exists";
    public static string ecutext = "error creating new login";
    public static string udetext = "username doesn't exist";
    public static string wptext = "wrong password";
    public static string uctext = "login created";

    public static string WarriorText = "WARRIOR";
    public static string ElemText = "ELEMENTALIST";
    public static string BarbarText = "BARBARIAN";
    public static string RogText = "ROGUE";
    public static string WizText = "WIZARD";

    public static string CreateNewCharacter = "CREATE NEW HERO";
    public static string back = "BACK";

    public static string EnterTheGameText = "ENTER THE GAME";
    public static string ChooseAnotherText = "CHOOSE ANOTHER HERO";

    public static string SpeedText = "speed";
    public static string HealthText = "health";
    public static string HealthRegenText = "health regen";
    public static string EnergyRegenText = "energy energy";
    public static string WeaponAttackText = "weapon attack";
    public static string HitPowerText = "hit power";
    public static string ArmorText = "armor";
    public static string ShieldBlockText = "shield block";
    public static string MagicResistanceText = "magic resistance";
    public static string DodgeText = "dodge";
    public static string CastSpeedText = "cast speed";
    public static string MeleeCritText = "melee crit";
    public static string MagicCritText = "magic crit";
    public static string SpellPowerText = "spell power";

    //envir===================================
    public static string Canceled = "CANCELED";


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

    public static string Spell6Name = "shield series";
    public static string Spell6ShortDescri = "series of three hits with increasing power of hit";
    public static string Spell6FullDescri = "series of three hits, one after another, with each hit the power of hit increases for 50%";

    public static string Spell51Name = "fireball";
    public static string Spell51ShortDescri = "shot with a fire ball";
    public static string Spell51FullDescri = "shot with a fire ball, which hits first enemy and inflicts damage in a small radius";

    public static string Spell52Name = "meteor";
    public static string Spell52ShortDescri = "meteor lands on mage, inflicts damage and stuns enemies";
    public static string Spell52FullDescri = "meteor lands in mage place, inflicts damage and stuns all enemies in range of hit";

    public static string Spell1002Name = "stun";
    public static string Spell1002ShortDescri = "stunned";
    public static string Spell1002FullDescri = "unable to move, attack or cast spells";

    //ТАЛАНТЫ======================
    public static string Talent1Name = "stuunn";
    public static string Talent1Description = "kbdsfks fuiw f frf";
}

