using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using System;
using System.Text;
using System.Security.Cryptography;

public class encryption
{    
    public RSAParameters publicKey;    
    public string encoded_secret_key_instring;
      

    public void ProcessInitDataFromServerTCP(string data)
    {
        
        //return $"{RawDataArray[0]}~{RawDataArray[1]}~0~ code ~ {public key string}";
        string[] string_data = data.Split('~');

        general.SecretKey = GetRandom256Code();
        Debug.Log("secret key is - " + FromByteToString(general.SecretKey));
        general.PacketID = string_data[3];

        //getting back real public key by public key string
        var sr = new System.IO.StringReader(string_data[4]);        
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));        
        publicKey = (RSAParameters)xs.Deserialize(sr);

        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
        csp.ImportParameters(publicKey);

        byte [] encoded_secret_key = csp.Encrypt(general.SecretKey, false);

        //conver bytes to normal string
        var sw1 = new System.IO.StringWriter();        
        var xs1 = new System.Xml.Serialization.XmlSerializer(typeof(byte[]));        
        xs1.Serialize(sw1, encoded_secret_key);        
        encoded_secret_key_instring = sw1.ToString();

        csp.Dispose();
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


    public static bool InitEncodingConnection(general.Ports ports)
    {
        //port 2326 for setup
        if ((int)ports == 2326)
        {
            if (general.PacketID!=null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2326, general.SetupServerIP, false);
                general.PacketID = "";
            }
            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2326, general.SetupServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.encoded_secret_key_instring}", general.Ports.tcp2326, general.SetupServerIP , false);
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
                general.PacketID = "";
            }

            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2324, general.LoginServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.encoded_secret_key_instring}", general.Ports.tcp2324, general.LoginServerIP, false);
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
            /*
            if (general.PacketID != null && general.PacketID != "")
            {
                connection.SendAndGetTCP($"0~6~2~{general.PacketID}", general.Ports.tcp2323, general.GameServerIP, false);
                general.PacketID = "";
            }
            */
            general.PlayerEncryption.ProcessInitDataFromServerTCP(connection.SendAndGetTCP("0~6~0", general.Ports.tcp2323, general.GameServerIP, false));
            string res = connection.SendAndGetTCP($"0~6~1~{general.PacketID}~{general.PlayerEncryption.encoded_secret_key_instring}", general.Ports.tcp2323, general.GameServerIP, false);
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

    public static byte[] GetRandom256Code()
    {
        SHA256 sh = new SHA256Managed();
        return sh.ComputeHash(Encoding.UTF8.GetBytes(general.get_random_set_of_symb(256)));
    }


}
