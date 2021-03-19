using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;


static public class R_SerializationHelper
{

    // Serilization
    // ====================================================================================================================================

    static public void SerializeString     (List<byte> bytes, string v)    { bytes.AddRange(BitConverter.GetBytes(v.Length)); bytes.AddRange(Encoding.ASCII.GetBytes(v)); }
    static public void SerializeFloat      (List<byte> bytes, float v)     { bytes.AddRange(BitConverter.GetBytes(v)); }
    static public void SerializeInt        (List<byte> bytes, int v)       { bytes.AddRange(BitConverter.GetBytes(v)); }
    static public void SerializeBool       (List<byte> bytes, bool v)      { bytes.Add(v ? (byte)1 : (byte)0); }
    static public void SerializeVector2     (List<byte> bytes, Vector2 v)   {  bytes.AddRange(BitConverter.GetBytes(v.x)); bytes.AddRange(BitConverter.GetBytes(v.y)); }
    static public void SerializeVector3     (List<byte> bytes, Vector3 v)   { bytes.AddRange(BitConverter.GetBytes(v.x)); bytes.AddRange(BitConverter.GetBytes(v.y)); bytes.AddRange(BitConverter.GetBytes(v.z)); }
    static public void SerializeVector4     (List<byte> bytes, Vector4 v)   { bytes.AddRange(BitConverter.GetBytes(v.x)); bytes.AddRange(BitConverter.GetBytes(v.y)); bytes.AddRange(BitConverter.GetBytes(v.z)); bytes.AddRange(BitConverter.GetBytes(v.w)); }
    static public void SerializeQuaternion  (List<byte> bytes, Quaternion v) { bytes.AddRange(BitConverter.GetBytes(v.x)); bytes.AddRange(BitConverter.GetBytes(v.y)); bytes.AddRange(BitConverter.GetBytes(v.z)); bytes.AddRange(BitConverter.GetBytes(v.w)); }
    static public void SerializeTransform  (List<byte> bytes, Transform t) { SerializeVector3(bytes, t.position); SerializeVector4(bytes, new Vector4(t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w)); SerializeVector3(bytes, t.localScale); }
    static public void SerializeColor (List<byte> bytes, Color color) { bytes.AddRange(BitConverter.GetBytes(color.r)); bytes.AddRange(BitConverter.GetBytes(color.g)); bytes.AddRange(BitConverter.GetBytes(color.b)); bytes.AddRange(BitConverter.GetBytes(color.a)); }

    static public void SerializeIntArray(List<byte> bytes, int[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeInt(bytes, array[i]); }
    static public void SerializeFloatArray(List<byte> bytes, float[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeFloat(bytes, array[i]); }
    static public void SerializeBoolArray(List<byte> bytes, bool[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeBool(bytes, array[i]); }
    static public void SerializeVector2Array(List<byte> bytes, Vector2[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeVector2(bytes, array[i]); }
    static public void SerializeVector3Array(List<byte> bytes, Vector3[] array) { if (array == null) { return; } SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeVector3(bytes, array[i]); }
    static public void SerializeVector4Array(List<byte> bytes, Vector4[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeVector4(bytes, array[i]); }
    static public void SerializeTransformArray(List<byte> bytes, Transform[] array) { SerializeInt(bytes, array.Length); for (int i = 0; i < array.Length; i++) SerializeTransform(bytes, array[i]); }

    static public void SerializeAnimationCurve (List<byte> bytes, AnimationCurve curve) { SerializeInt(bytes, curve.length); for (int i = 0; i < curve.length; i++) { SerializeVector2(bytes, new Vector2(curve.keys[i].time, curve.keys[i].value)); } }
    static public void SerializeAnimationCurveExact (List<byte> bytes, AnimationCurve curve)
    {
        SerializeInt(bytes, curve.length);
        for (int i = 0; i < curve.length; i++)
        {
            Keyframe key = curve.keys[i];
            SerializeFloat(bytes, key.time);
            SerializeFloat(bytes, key.value);
            SerializeFloat(bytes, key.inTangent);
            SerializeFloat(bytes, key.outTangent);
            // SerializeInt(bytes, key.tangentMode);
        }
    }

    static public void Serialize2DFloatArray(List <byte> bytes, float[,] array)
    {
        int yLength = array.GetLength(0);
        int xLength = array.GetLength(1);

        bytes.AddRange(BitConverter.GetBytes(yLength));
        bytes.AddRange(BitConverter.GetBytes(xLength));
        
        for (int y = 0; y < yLength; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                bytes.AddRange(BitConverter.GetBytes(array[y,x]));
            }
        }
    }

    static public void Serialize2DIntArrayToBytes(List<byte> bytes, int[,] array)
    {
        int yLength = array.GetLength(0);
        int xLength = array.GetLength(1);

        bytes.AddRange(BitConverter.GetBytes(yLength));
        bytes.AddRange(BitConverter.GetBytes(xLength));

        for (int y = 0; y < yLength; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                bytes.Add((byte)array[y,x]);
            }
        }
    }

    // Deserialization
    // ====================================================================================================================================

    static public string DeserializeString(byte[] bytes, ref int index)
    {
        int length = BitConverter.ToInt32(bytes, index); index += 4;
        string v = Encoding.ASCII.GetString(bytes, index, length);
        index += length;
        return v;
    }

    static public float DeserializeFloat(byte[] bytes, ref int index)
    {
        float v = BitConverter.ToSingle(bytes, index); index += 4;
        return v;
    }

    static public int DeserializeInt(byte[] bytes, ref int index)
    {
        int v = BitConverter.ToInt32(bytes, index); index += 4;
        return v;
    }

    static public bool DeserializeBool(byte[] bytes, ref int index) { return (bytes[index++] == 1 ? true : false); }
        
    static public Vector2 DeserializeVector2(byte[] bytes, ref int index)
    {
        Vector2 v;
        v.x = BitConverter.ToSingle(bytes, index); index += 4;
        v.y = BitConverter.ToSingle(bytes, index); index += 4;
        return v;
    }

    static public Vector3 DeserializeVector3(byte[] bytes, ref int index)
    {
        Vector3 v;
        v.x = BitConverter.ToSingle(bytes, index); index += 4;
        v.y = BitConverter.ToSingle(bytes, index); index += 4;
        v.z = BitConverter.ToSingle(bytes, index); index += 4;
        return v;
    }

    static public Vector4 DeserializeVector4(byte[] bytes, ref int index)
    {
        Vector4 v;
        v.x = BitConverter.ToSingle(bytes, index); index += 4;
        v.y = BitConverter.ToSingle(bytes, index); index += 4;
        v.z = BitConverter.ToSingle(bytes, index); index += 4;
        v.w = BitConverter.ToSingle(bytes, index); index += 4;
        return v;
    }

    static public Quaternion DeserializeQuaternion(byte[] bytes, ref int index)
    {
        Quaternion v;
        v.x = BitConverter.ToSingle(bytes, index); index += 4;
        v.y = BitConverter.ToSingle(bytes, index); index += 4;
        v.z = BitConverter.ToSingle(bytes, index); index += 4;
        v.w = BitConverter.ToSingle(bytes, index); index += 4;
        return v;
    }

    static public Color DeserializeColor(byte[] bytes, ref int index)
    {
        Color color;
        color.r = BitConverter.ToSingle(bytes, index); index += 4;
        color.g = BitConverter.ToSingle(bytes, index); index += 4;
        color.b = BitConverter.ToSingle(bytes, index); index += 4;
        color.a = BitConverter.ToSingle(bytes, index); index += 4;
        return color;
    }

    static public void DeserializeTransform(byte[] bytes, ref int index, Transform t)
    {
        t.position = DeserializeVector3(bytes, ref index);
        Vector4 rotation = DeserializeVector4(bytes, ref index);
        t.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        t.localScale = DeserializeVector3(bytes, ref index);
    }

    static public void DeserializeAnimationCurve(byte[] bytes, ref int index, AnimationCurve curve)
    {
        Vector2 v2;
        int length = DeserializeInt(bytes, ref index);
        for (int i = 0; i < length; i++) { v2 = DeserializeVector2(bytes, ref index); curve.AddKey(v2.x, v2.y); }
    }

    static public void DeserializeAnimationCurve2(byte[] bytes, ref int index, AnimationCurve curve)
    {
        Vector2 v2;
        int length = DeserializeInt(bytes, ref index);
        for (int i = 0; i < length; i++) { v2 = DeserializeVector2(bytes, ref index); curve.AddKey(new Keyframe(v2.x, v2.y, Mathf.Infinity, Mathf.Infinity)); }
    }

    static public Keyframe[] DeserializeAnimationCurveExact(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        Keyframe[] keys = new Keyframe[length];
        
        for (int i = 0; i < length; i++)
        {
            float time = DeserializeFloat(bytes, ref index);
            float value = DeserializeFloat(bytes, ref index);
            float inTangent = DeserializeFloat(bytes, ref index);
            float outTangent = DeserializeFloat(bytes, ref index);
            keys[i] = new Keyframe(time, value, inTangent, outTangent);
            // keys[i].tangentMode = DeserializeInt(bytes, ref index);
        }

        return keys;
    }

    static public int[] DeserializeIntArray(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        int[] array = new int[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeInt(bytes, ref index);
        return array;
    }

    static public float[] DeserializeFloatArray(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        float[] array = new float[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeFloat(bytes, ref index);
        return array;
    }
    
    static public bool[] DeserializeBoolArray(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        bool[] array = new bool[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeBool(bytes, ref index);
        return array;
    }

    static public Vector2[] DeserializeVector2Array(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        Vector2[] array = new Vector2[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeVector2(bytes, ref index);
        return array;
    }

    static public Vector3[] DeserializeVector3Array(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        Vector3[] array = new Vector3[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeVector3(bytes, ref index);
        return array;
    }

    static public Vector4[] DeserializeVector4Array(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        Vector4[] array = new Vector4[length];
        for (int i = 0; i < length; i++) array[i] = DeserializeVector4(bytes, ref index);
        return array;
    }

    static public Transform[] DeserializeTransformArray(byte[] bytes, ref int index)
    {
        int length = DeserializeInt(bytes, ref index);
        Transform[] array = new Transform[length];
        for (int i = 0; i < length; i++) DeserializeTransform(bytes, ref index, array[i]);
        return array;
    }

    static public float[,] Deserialize2DFloatArray(byte[] bytes, ref int index)
    {
        int yLength = BitConverter.ToInt32(bytes, index); index += 4;
        int xLength = BitConverter.ToInt32(bytes, index); index += 4;

        float[,] v = new float[yLength, xLength];

        for (int y = 0; y < yLength; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                v[y, x] = BitConverter.ToSingle(bytes, index); index += 4;
            }
        }
        return v;
    }

    static public int[,] Deserialize2DByteArrayToInt(byte[] bytes, ref int index)
    {
        int yLength = BitConverter.ToInt32(bytes, index); index += 4;
        int xLength = BitConverter.ToInt32(bytes, index); index += 4;

        int[,] v = new int[yLength, xLength];

        for (int y = 0; y < yLength; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                v[y, x] = bytes[index++];
            }
        }
        return v;
    }

}

