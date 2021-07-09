using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class languages : MonoBehaviour
{
    public static SystemLanguage CurrentLanguage;
    public static lang_general lang = new lang_general();
    // Start is called before the first frame update
    void Start()
    {
        CurrentLanguage = Application.systemLanguage;

        if (CurrentLanguage == SystemLanguage.Russian)
        {
            /*
            using (FileStream fs = new FileStream(Application.dataPath + "/Resources/user.json", FileMode.Open))
            {

                byte[] arr = new byte[fs.Length];
                fs.Read(arr, 0, arr.Length);

                lang = JsonUtility.FromJson<lang_general>(Encoding.UTF8.GetString(arr));
                print(lang.LoginText + " - " + lang.CreateNewText + lang.ConnectionErrorText);

            }
            */
            
        }
    }

   
}




public class lang_general
{
    
    //логин и вызов пользователя
    //public  string LoginText = "";
    public  string LoginText = "ВОЙТИ";
    public  string CreateNewText = "СОЗДАТЬ";
    public  string AwaitingText = "<size=200%>ЗАГРУЗКА!..<size=100%>";
    public  string ConnectionErrorText = "НЕТ СОЕДИНЕНИЯ";
    public  string CreateNewLoginPass = "создать новый логин-пароль";
    public  string EnterAsGuest = "войти в игру как гость";

    public  string TheLoginText = "ЛОГИН";
    public  string ThePasswordText = "ПАРОЛЬ";

    public  string SignForNameForEnter = "имя для ввода:";


    public  string wlltext = "неправильная длина логина";
    public  string wdstext = "недопустимые символы";
    public  string wlptext = "неправильная длина пароля";
    public  string uaetext = "такой логин уже существует";
    public  string ecutext = "ошибка при создании логина";
    public  string udetext = "нет такого пользователя";
    public  string wptext = "ошибка в пароле";
    public  string uctext = "пользователь создан";
    public  string egttext = "ошибка в получении тикета";
    public  string errtext = "произошла ошибка";
    public  string conerrtext = "ошибка соединения";
    public  string loadtext = "ЗАГРУЗКА...";
    public  string nsttext = "тикет не существует";
    public  string wcntext = "неправильное имя или пароль";
    public  string caetext = "такое имя уже есть";
    public  string taetext = "такой тип персонажа у вас уже есть";
    public  string nsctext = "такого персонажа нет";
    public  string eittext = "ошибка в талантах";
    public  string nsstext = "такого спела нет в книге";
    public  string rsntext = "спелы повторяются";
    public  string dbetext = "ошибка в БД";

    public  string WarriorText = "ВОИН";
    public  string ElemText = "ЕЛЕМЕНТАЛИСТ";
    public  string BarbarText = "ВАРВАР";
    public  string RogText = "РАЗБОЙНИК";
    public  string WizText = "ВОЛШЕБНИК";

    public  string WarriorText_descr = "Использует обычный меч и преимущества экипировки: щит и броню.";
    public  string ElemText_descr = "Применяет силу стихий для быстрого нанесения большого урона.";
    public  string BarbarText_descr = "Обладает крепким здоровьем и способен превращать свою ярость в мощные удары.";
    public  string RogText_descr = "Мастер скрытности, мобильности и крайне опасных ударов.";
    public  string WizText_descr = "Имеет самый большой арсенал заклинаний контроля и магических уловок.";

    public  string WarriorText_conspros = "- высокая выживаемость;\n- навык блокировки щитом;\n- мощная броня;\n- слабый удар и шанс критичес-\nкого удара;";
    public  string ElemText_conspros = "- быстрое нанесение значительного урона;\n- ускоренное восполнение энергии;\n- слабая броня и здоровье;";
    public  string BarbarText_conspros = "- большой запас здоровья;\n- увеличенная защита от магии;\n- слабая броня и медленное восста-\nновление энергии";
    public  string RogText_conspros = "- увеличенная скорость передвижения;\n- навык невидимости;\n- увеличенный шанс критических ударов;\n- низкий показатель здоровья и брони;";
    public  string WizText_conspros = "- аресенал заклинаний контроля;\n- высокая мобильность;\n- слабая броня и здоровье;";

    public  string CreateNewCharacter = "СОЗДАТЬ ГЕРОЯ";
    public  string back = "НАЗАД";
    public  string EnterCharName = "введите имя персонажа:";

    public  string EnterTheGameText = "ВОЙТИ В ИГРУ";
    public  string ChooseAnotherText = "ВЫБРАТЬ ДРУГОГО ГЕРОЯ";

    public  string SpeedText = "скорость";
    public  string HealthText = "здоровье";
    public  string HealthRegenText = "восст здоровья";
    public  string EnergyRegenText = "восст энергии";
    public  string WeaponAttackText = "атака оружия";
    public  string HitPowerText = "сила удара";
    public  string ArmorText = "броня";
    public  string ShieldBlockText = "блок щитом";
    public  string MagicResistanceText = "защита от магии";
    public  string DodgeText = "уклонение";
    public  string CastSpeedText = "скорость применения магии";
    public  string MeleeCritText = "шанс крит атаки";
    public  string MagicCritText = "шанс крит атаки";
    public  string SpellPowerText = "сила магии";

    public  string SpeedTextHint = "<size=120%>скорость<size=100%> определяет, как быстро Вы можете передви- гаться. Стандартный показатель - это 1 единица";
    public  string HealthTextHint = "<size=120%>здоровье<size=105%> показывает, какое максимальное количество повреждений может получить персонаж";
    public  string HealthRegenTextHint = "<size=120%>восст здоровья<size=100%> показывает количество здоровья, восста- навливаемого за 1 секунду вре- мени";
    public  string EnergyRegenTextHint = "<size=120%>восст энергии<size=100%> показывает количество энергии, восста- навливаемой за 1 секунду вре- мени";
    public  string WeaponAttackTextHint = "<size=120%>атака оружия<size=95%> - это базовое повреждение оружием, выбирается случайно между меньшим и большим значениями";
    public  string HitPowerTextHint = "<size=120%>сила удара<size=100%> показывает, насколько увеличивается базовое повреждение оружием из атаки оружия";
    public  string ArmorTextHint = "<size=120%>броня<size=90%> снижает количество получаемого урона. Максимальное значение в 1000 единиц означает, что урон будет снижен до нуля";
    public  string ShieldBlockTextHint = "<size=120%>блок щитом<size=85%> показывает вероятность заблокировать входящий урон от атаки или магии, направленной спереди. Максимальное значение - 100%";
    public  string MagicResistanceTextHint = "<size=120%>защита от магии<size=95%> уменьшает урон и негативное влияние любых заклинаний. Максимальное значение - 100%";
    public  string DodgeTextHint = "<size=120%>уклонение<size=87%> определяет вероят- ность увернуться от физического повреждения, направленного с лю- бой стороны. Максимальное значение - 100%";
    public  string CastSpeedTextHint = "<size=110%>скорость применения магии<size=95%> показывает скорость чтения заклинаний. Максимальное значение - 100% или мгновенно";
    public  string MeleeCritTextHint = "<size=120%>шанс крит атаки<size=95%> показывает веротяность нанести увеличенное повреждение. Максимальное значение - 100%";
    public  string MagicCritTextHint = "<size=120%>шанс крит атаки<size=95%> показывает веротяность нанести увеличенное повреждение. Максимальное значение - 100%";
    public  string SpellPowerTextHint = "<size=120%>сила магии<size=105%> - это базовое значение нанесения урона магией";

    //envir===================================
    public  string Canceled = "ОТМЕНА";
    public  string ManaCostText = "затраты маны:";

    //all spells=============================== 36 symbols in 3 rows "dddddddddddddddddddddddddddddddddddd"
    public  string Spell1Name = "простой удар";
    public  string Spell1ShortDescri = "простой удар одноручным оружием";
    public  string Spell1FullDescri = "простой удар одноручным оружием";

    public  string Spell2Name = "кровотечение";
    public  string Spell2ShortDescri = "легкое повреждение в течение 5 сек";
    public  string Spell2FullDescri = "наносит легкое повреждение каждую секунду в течение 10 секунд";

    public  string Spell3Name = "крепкое здоровье";
    public  string Spell3ShortDescri = "здоровье +20%, восстановление жизни +1 на 60 сек";
    public  string Spell3FullDescri = "на 60 секунд увеличивает запас здо- ровья на 20% и регенерацию здоровья на 1 себе и ближайшим союзникам";

    public  string Spell4Name = "удар щитом";
    public  string Spell4ShortDescri = "удар щитом и оглушение на 3 сек";
    public  string Spell4FullDescri = "удар щитом наносит легкое повреждение и оглушает противника на 3 секунды";

    public  string Spell5Name = "за щитом";
    public  string Spell5ShortDescri = "блок всех атак на 5 сек";
    public  string Spell5FullDescri = "блокирует все прямые удары и заклинания в течение 5 сек, но понижает скорость передвижения";

    public  string Spell6Name = "серия с щитом";
    public  string Spell6ShortDescri = "серия из трех ударов с растущей силой удара";
    public  string Spell6FullDescri = "серия из трех ударов, сила каждого последующего удара в связке вырастает";

    public  string Spell51Name = "огненный выстрел";
    public  string Spell51ShortDescri = "выстрел огненным шаром";
    public  string Spell51FullDescri = "выстрел огненным шаром, который попадает в первого противника и наносит повреждение в небольшом радиусе";

    public  string Spell52Name = "метеор";
    public  string Spell52ShortDescri = "метеор приземляется в мага, наносит повтерждение и огрушает противников";
    public  string Spell52FullDescri = "метеор приземляется в точку нахождения мага, наносит повреждение и оглушает всех противников по области";

    public  string Spell1002Name = "оглушение";
    public  string Spell1002ShortDescri = "оглушение";
    public  string Spell1002FullDescri = "невозможно двигаться, атаковать и произносить заклинания";

    //ТАЛАНТЫ======================
    public  string Talent1Name = "оглушение";
    public  string Talent1Description = "оглушение gvbrffgd egerg  egref";

    public  string PVPStatsTeamName = "КОМАНДА";
    public  string PVPStatsPlayerName = "игрок:";
    public  string PVPStatsPlayerScore = "очки:";
}
