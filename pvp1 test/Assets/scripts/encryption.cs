using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;
using System.Text;
using System.Security.Cryptography;

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
        //return GetByteArrFromCharByChar(preres);
        return Encoding.UTF8.GetBytes(preres);
    }

    public void ProcessInitDataFromServerUDP(string data)
    {
        //return $"{RawDataArray[0]}~{RawDataArray[1]}~0~{player.PlayerEncryption.base_part_pow}~{player.PlayerEncryption.mod_part}~{player.PlayerEncryption.open_key_from_server_one}~{player.PlayerEncryption.open_key_from_server_two}~{player.PlayerEncryption.open_key_from_server_three}";
        string[] string_data = data.Split('~');

        CreateOpenKeys(int.Parse(string_data[3]), int.Parse(string_data[4]));
        general.SecretKey = GetSecretKey(int.Parse(string_data[5]), int.Parse(string_data[6]), int.Parse(string_data[7]));
        general.PacketID = string_data[8];
    }

    public void ProcessInitDataFromServerTCP(string data)
    {
        
        //return $"{RawDataArray[0]}~{RawDataArray[1]}~0~ code ~ {player.PlayerEncryption.base_part_pow}~{player.PlayerEncryption.mod_part}~{player.PlayerEncryption.open_key_from_server_one}~{player.PlayerEncryption.open_key_from_server_two}~{player.PlayerEncryption.open_key_from_server_three}";
        string[] string_data = data.Split('~');

        CreateOpenKeys(int.Parse(string_data[4]), int.Parse(string_data[5]));
        general.SecretKey = GetSecretKey(int.Parse(string_data[6]), int.Parse(string_data[7]), int.Parse(string_data[8]));
        general.PacketID = string_data[3];
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

        for (int i = 6; i < source.Length; i++)
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

        for (int i = 6; i < source.Length; i++)
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


    private static void KillKey(string packetID)
    {

    }


    public static bool InitEncodingConnection(general.Ports ports)
    {

        //port 2326 for setup
        if ((int)ports == 2326)
        {
            if (general.PacketID!=null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2326, general.SetupServerIP, false);
            }
            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2326, general.SetupServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.open_key_from_client_one}~{general.PlayerEncryption.open_key_from_client_two}~{general.PlayerEncryption.open_key_from_client_three}", general.Ports.tcp2326, general.SetupServerIP , false);
            string[] result = res.Split('~');

            if (result[3] == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //port 2324 for login
        if ((int)ports == 2324)
        {
            if (general.PacketID != null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2324, general.LoginServerIP, false);
            }

            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2324, general.LoginServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.open_key_from_client_one}~{general.PlayerEncryption.open_key_from_client_two}~{general.PlayerEncryption.open_key_from_client_three}", general.Ports.tcp2324, general.LoginServerIP, false);
            string[] result = res.Split('~');

            if (result[3] == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //port 2323 for gameplay
        if ((int)ports == 2323)
        {
            if (general.PacketID != null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2323, general.GameServerIP, false);
            }

            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2323, general.GameServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.open_key_from_client_one}~{general.PlayerEncryption.open_key_from_client_two}~{general.PlayerEncryption.open_key_from_client_three}", general.Ports.tcp2323, general.GameServerIP, false);
            string[] result = res.Split('~');

            if (result[3] == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public static byte[] GetHash384(string data)
    {
        SHA384 create_hash = SHA384.Create();
        return create_hash.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    public static string FromByteToString(byte[] data)
    {
        StringBuilder d = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            d.Append(data[i]);
        }

        return d.ToString();
    }


}
