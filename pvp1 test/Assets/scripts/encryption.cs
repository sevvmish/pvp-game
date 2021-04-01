using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;

public class encryption: IDisposable
{
    public int base_part_pow, mod_part;

    //close keys
    public int close_key_forclient_one;
    public int close_key_forclient_two;
    public int close_key_forclient_three;

    //open keys
    public BigInteger open_key_from_client_one;
    public BigInteger open_key_from_client_two;
    public BigInteger open_key_from_client_three;
    

    //secret keys
    public int[] presecret_key = new int[3];


    // A(open key) = base_part_pow^close_key mod mod_part
    // close_key - smaler   mod_part - bigger  base_part_pow - just 2-9

    public void CreateOpenKeys(int base_part, int mod_par)
    {
        base_part_pow = base_part;
        mod_part = mod_par;

        close_key_forclient_one = GetPrimeNumber(10000, 30000);
        close_key_forclient_two = GetPrimeNumber(10000, 30000);
        close_key_forclient_three = GetPrimeNumber(10000, 30000);
        Debug.Log($"close keys: {close_key_forclient_one} - {close_key_forclient_two} - {close_key_forclient_three}");

        open_key_from_client_one = get_result_open_key(base_part_pow, close_key_forclient_one, mod_part);
        open_key_from_client_two = get_result_open_key(base_part_pow, close_key_forclient_two, mod_part);
        open_key_from_client_three = get_result_open_key(base_part_pow, close_key_forclient_three, mod_part);
        Debug.Log($"opne  keys for srerver: {open_key_from_client_one} - {open_key_from_client_two} - {open_key_from_client_three}");

    }

    public byte[] GetSecretKey(int open_key_one, int open_key_two, int open_key_three)
    {
        BigInteger result = get_result_open_key((int)open_key_one, close_key_forclient_one, mod_part);
        presecret_key[0] = (int)result;
        result = get_result_open_key((int)open_key_two, close_key_forclient_two, mod_part);
        presecret_key[1] = (int)result;
        result = get_result_open_key((int)open_key_three, close_key_forclient_three, mod_part);
        presecret_key[2] = (int)result;
        string preres = $"{presecret_key[0]}{presecret_key[1]}{presecret_key[2]}";
        Debug.Log(preres + " - secret key");
        return GetByteArrFromCharByChar(preres);
    }

    public void ProcessInitDataFromServer(string data)
    {
        //return $"{RawDataArray[0]}~{RawDataArray[1]}~0~{player.PlayerEncryption.base_part_pow}~{player.PlayerEncryption.mod_part}~{player.PlayerEncryption.open_key_from_server_one}~{player.PlayerEncryption.open_key_from_server_two}~{player.PlayerEncryption.open_key_from_server_three}";
        string[] string_data = data.Split('~');

        CreateOpenKeys(int.Parse(string_data[3]), int.Parse(string_data[4]));
        general.SecretKey = GetSecretKey(int.Parse(string_data[5]), int.Parse(string_data[6]), int.Parse(string_data[7]));
        
    }



    private int get_random_two_nine()
    {
        int result = 0;

        System.Random rnd = new System.Random();
        result = rnd.Next(2, 9);

        return result;
    }

    private BigInteger get_result_open_key(int base_part_of_pow, int close_key, int mod_part)
    {
        return BigInteger.Pow(base_part_of_pow, close_key) % mod_part;
    }

    private int GetPrimeNumber(int start_case, int size)
    {
        //System.Random rnd = new System.Random();
        //int random_in_start_case = rnd.Next(start_case, size - 1000);

        int random_in_start_case = UnityEngine.Random.Range(start_case, size - 1000);

        for (int i = random_in_start_case; i < size; i++)
        {
            int sqrt = (int)Math.Sqrt(i);
            
            bool isOK = true;
            for (int ii = 2; ii < sqrt; ii++)
            {
                
                if (i % ii == 0)
                {
                    isOK = false;
                    break;
                }
            }
            if (isOK)
            {
                return i;
            }
        }

        return random_in_start_case;
    }


    public void Dispose()
    {
        //this.Dispose();
    }

    public byte[] GetByteArrFromCharByChar(string key_in_string)
    {
        List<byte> result = new List<byte>();

        for (int i = 0; i < key_in_string.Length; i++)
        {
            result.Add(Byte.Parse(key_in_string.Substring(i, 1)));
        }

        return result.ToArray();
    }

    public static void Encode(ref byte[] source, byte[] key)
    {
        int index = 0;

        for (int i = 0; i < source.Length; i++)
        {
            source[i] = (byte)(source[i] + key[index]);

            if ((index + 1) == key.Length)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
    }

    public static void Decode(ref byte[] source, byte[] key)
    {
        int index = 0;

        for (int i = 0; i < source.Length; i++)
        {
            source[i] = (byte)(source[i] - key[index]);

            if ((index + 1) == key.Length)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }


    }
}
