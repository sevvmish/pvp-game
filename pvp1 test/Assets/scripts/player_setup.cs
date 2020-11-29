using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;

public class player_setup : MonoBehaviour
{
    public GameObject ConnectionError;
    public character_data CurrentCharacterData;

    // Start is called before the first frame update
    void Start()
    {
        sr.isConnectionError = false;

        Screen.SetResolution(1280, 720, true);
        Camera.main.aspect = 16f / 9f;
        ConnectionError.SetActive(false);
        
        string result = sr.SendAndGetLoginSetup("2~0~" + general.CurrentTicket + "~" + general.CharacterName);
        CurrentCharacterData = new character_data(result);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.isConnectionError)
        {
            StartCoroutine(ConnectionErr());
        }
    }


    IEnumerator ConnectionErr()
    {
        ConnectionError.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("player_setup");
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

        
    }
}