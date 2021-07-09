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
    
    //����� � ����� ������������
    //public  string LoginText = "";
    public  string LoginText = "�����";
    public  string CreateNewText = "�������";
    public  string AwaitingText = "<size=200%>��������!..<size=100%>";
    public  string ConnectionErrorText = "��� ����������";
    public  string CreateNewLoginPass = "������� ����� �����-������";
    public  string EnterAsGuest = "����� � ���� ��� �����";

    public  string TheLoginText = "�����";
    public  string ThePasswordText = "������";

    public  string SignForNameForEnter = "��� ��� �����:";


    public  string wlltext = "������������ ����� ������";
    public  string wdstext = "������������ �������";
    public  string wlptext = "������������ ����� ������";
    public  string uaetext = "����� ����� ��� ����������";
    public  string ecutext = "������ ��� �������� ������";
    public  string udetext = "��� ������ ������������";
    public  string wptext = "������ � ������";
    public  string uctext = "������������ ������";
    public  string egttext = "������ � ��������� ������";
    public  string errtext = "��������� ������";
    public  string conerrtext = "������ ����������";
    public  string loadtext = "��������...";
    public  string nsttext = "����� �� ����������";
    public  string wcntext = "������������ ��� ��� ������";
    public  string caetext = "����� ��� ��� ����";
    public  string taetext = "����� ��� ��������� � ��� ��� ����";
    public  string nsctext = "������ ��������� ���";
    public  string eittext = "������ � ��������";
    public  string nsstext = "������ ����� ��� � �����";
    public  string rsntext = "����� �����������";
    public  string dbetext = "������ � ��";

    public  string WarriorText = "����";
    public  string ElemText = "������������";
    public  string BarbarText = "������";
    public  string RogText = "���������";
    public  string WizText = "���������";

    public  string WarriorText_descr = "���������� ������� ��� � ������������ ����������: ��� � �����.";
    public  string ElemText_descr = "��������� ���� ������ ��� �������� ��������� �������� �����.";
    public  string BarbarText_descr = "�������� ������� ��������� � �������� ���������� ���� ������ � ������ �����.";
    public  string RogText_descr = "������ ����������, ����������� � ������ ������� ������.";
    public  string WizText_descr = "����� ����� ������� ������� ���������� �������� � ���������� ������.";

    public  string WarriorText_conspros = "- ������� ������������;\n- ����� ���������� �����;\n- ������ �����;\n- ������ ���� � ���� ��������-\n���� �����;";
    public  string ElemText_conspros = "- ������� ��������� ������������� �����;\n- ���������� ����������� �������;\n- ������ ����� � ��������;";
    public  string BarbarText_conspros = "- ������� ����� ��������;\n- ����������� ������ �� �����;\n- ������ ����� � ��������� ������-\n�������� �������";
    public  string RogText_conspros = "- ����������� �������� ������������;\n- ����� �����������;\n- ����������� ���� ����������� ������;\n- ������ ���������� �������� � �����;";
    public  string WizText_conspros = "- �������� ���������� ��������;\n- ������� �����������;\n- ������ ����� � ��������;";

    public  string CreateNewCharacter = "������� �����";
    public  string back = "�����";
    public  string EnterCharName = "������� ��� ���������:";

    public  string EnterTheGameText = "����� � ����";
    public  string ChooseAnotherText = "������� ������� �����";

    public  string SpeedText = "��������";
    public  string HealthText = "��������";
    public  string HealthRegenText = "����� ��������";
    public  string EnergyRegenText = "����� �������";
    public  string WeaponAttackText = "����� ������";
    public  string HitPowerText = "���� �����";
    public  string ArmorText = "�����";
    public  string ShieldBlockText = "���� �����";
    public  string MagicResistanceText = "������ �� �����";
    public  string DodgeText = "���������";
    public  string CastSpeedText = "�������� ���������� �����";
    public  string MeleeCritText = "���� ���� �����";
    public  string MagicCritText = "���� ���� �����";
    public  string SpellPowerText = "���� �����";

    public  string SpeedTextHint = "<size=120%>��������<size=100%> ����������, ��� ������ �� ������ �������- ������. ����������� ���������� - ��� 1 �������";
    public  string HealthTextHint = "<size=120%>��������<size=105%> ����������, ����� ������������ ���������� ����������� ����� �������� ��������";
    public  string HealthRegenTextHint = "<size=120%>����� ��������<size=100%> ���������� ���������� ��������, ������- ������������ �� 1 ������� ���- ����";
    public  string EnergyRegenTextHint = "<size=120%>����� �������<size=100%> ���������� ���������� �������, ������- ����������� �� 1 ������� ���- ����";
    public  string WeaponAttackTextHint = "<size=120%>����� ������<size=95%> - ��� ������� ����������� �������, ���������� �������� ����� ������� � ������� ����������";
    public  string HitPowerTextHint = "<size=120%>���� �����<size=100%> ����������, ��������� ������������� ������� ����������� ������� �� ����� ������";
    public  string ArmorTextHint = "<size=120%>�����<size=90%> ������� ���������� ����������� �����. ������������ �������� � 1000 ������ ��������, ��� ���� ����� ������ �� ����";
    public  string ShieldBlockTextHint = "<size=120%>���� �����<size=85%> ���������� ����������� ������������� �������� ���� �� ����� ��� �����, ������������ �������. ������������ �������� - 100%";
    public  string MagicResistanceTextHint = "<size=120%>������ �� �����<size=95%> ��������� ���� � ���������� ������� ����� ����������. ������������ �������� - 100%";
    public  string DodgeTextHint = "<size=120%>���������<size=87%> ���������� ������- ����� ���������� �� ����������� �����������, ������������� � ��- ��� �������. ������������ �������� - 100%";
    public  string CastSpeedTextHint = "<size=110%>�������� ���������� �����<size=95%> ���������� �������� ������ ����������. ������������ �������� - 100% ��� ���������";
    public  string MeleeCritTextHint = "<size=120%>���� ���� �����<size=95%> ���������� ����������� ������� ����������� �����������. ������������ �������� - 100%";
    public  string MagicCritTextHint = "<size=120%>���� ���� �����<size=95%> ���������� ����������� ������� ����������� �����������. ������������ �������� - 100%";
    public  string SpellPowerTextHint = "<size=120%>���� �����<size=105%> - ��� ������� �������� ��������� ����� ������";

    //envir===================================
    public  string Canceled = "������";
    public  string ManaCostText = "������� ����:";

    //all spells=============================== 36 symbols in 3 rows "dddddddddddddddddddddddddddddddddddd"
    public  string Spell1Name = "������� ����";
    public  string Spell1ShortDescri = "������� ���� ���������� �������";
    public  string Spell1FullDescri = "������� ���� ���������� �������";

    public  string Spell2Name = "������������";
    public  string Spell2ShortDescri = "������ ����������� � ������� 5 ���";
    public  string Spell2FullDescri = "������� ������ ����������� ������ ������� � ������� 10 ������";

    public  string Spell3Name = "������� ��������";
    public  string Spell3ShortDescri = "�������� +20%, �������������� ����� +1 �� 60 ���";
    public  string Spell3FullDescri = "�� 60 ������ ����������� ����� ���- ����� �� 20% � ����������� �������� �� 1 ���� � ��������� ���������";

    public  string Spell4Name = "���� �����";
    public  string Spell4ShortDescri = "���� ����� � ��������� �� 3 ���";
    public  string Spell4FullDescri = "���� ����� ������� ������ ����������� � �������� ���������� �� 3 �������";

    public  string Spell5Name = "�� �����";
    public  string Spell5ShortDescri = "���� ���� ���� �� 5 ���";
    public  string Spell5FullDescri = "��������� ��� ������ ����� � ���������� � ������� 5 ���, �� �������� �������� ������������";

    public  string Spell6Name = "����� � �����";
    public  string Spell6ShortDescri = "����� �� ���� ������ � �������� ����� �����";
    public  string Spell6FullDescri = "����� �� ���� ������, ���� ������� ������������ ����� � ������ ���������";

    public  string Spell51Name = "�������� �������";
    public  string Spell51ShortDescri = "������� �������� �����";
    public  string Spell51FullDescri = "������� �������� �����, ������� �������� � ������� ���������� � ������� ����������� � ��������� �������";

    public  string Spell52Name = "������";
    public  string Spell52ShortDescri = "������ ������������ � ����, ������� ������������ � �������� �����������";
    public  string Spell52FullDescri = "������ ������������ � ����� ���������� ����, ������� ����������� � �������� ���� ����������� �� �������";

    public  string Spell1002Name = "���������";
    public  string Spell1002ShortDescri = "���������";
    public  string Spell1002FullDescri = "���������� ���������, ��������� � ����������� ����������";

    //�������======================
    public  string Talent1Name = "���������";
    public  string Talent1Description = "��������� gvbrffgd egerg  egref";

    public  string PVPStatsTeamName = "�������";
    public  string PVPStatsPlayerName = "�����:";
    public  string PVPStatsPlayerScore = "����:";
}
