using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TerrainComposer2
{
    static public class Mathw
    {
        static public readonly byte[] bit8 = { 1, 2, 4, 8, 16, 32, 64, 128 };

        static public float Clamp01(float v)
        {
            if (v < 0) v = 0;
            else if (v > 1) v = 1;
            return v;
        }

        static public float Clamp(float v, float min, float max)
        {
            if (v < min) v = min;
            else if (v > max) v = max;
            return v;
        }

        static public int Clamp(int v, int min, int max)
        {
            if (v < min) v = min;
            else if (v > max) v = max;
            return v;
        }

        static public byte Clamp(byte v, byte min, byte max)
        {
            if (v < min) v = min;
            else if (v > max) v = max;
            return v;
        }

        static public float Abs(float v)
        {
            return v < 0 ? v * -1 : v;
        }

        static public int Abs(int v)
        {
            return v < 0 ? v * -1 : v;
        }

        static public double Abs(double v)
        {
            return v < 0 ? v * -1 : v;
        }

        static public float GetColorBrightness(Color color)
        {
            return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        }

        static public float Frac(float v)
        {
            return v - Mathf.Floor(v);
        }

        static public bool ArrayContains<T>(T[] array, T obj) where T : class
        {
            for (int i = 0; i < array.Length; i++) if (array[i] == obj) return true;
            return false;
        }

        static public UnityEngine.Object[] AddToArray(UnityEngine.Object[] array, UnityEngine.Object obj)
        {
            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
            objs.AddRange(array);
            objs.Add(obj);
            return objs.ToArray();
        }

        static public T[] AddToArray<T>(T[] array, T t)
        {
            T[] temp = new T[array.Length + 1];

            for (int i = 0; i < array.Length; i++) temp[i] = array[i];
            temp[array.Length] = t;
            return temp;
        }

        static public T[] ResizeArray<T>(T[] array, int length)
        {
            // Debug.Log("Resize array");
            if (array == null) return new T[length];

            T[] temp = new T[length];

            int copyLength = array.Length > length ? length : array.Length;

            for (int i = 0; i < copyLength; i++) temp[i] = array[i];
            return temp;
        }

        static public UnityEngine.Object[] RemoveFromArray(UnityEngine.Object[] array, UnityEngine.Object obj)
        {
            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
            objs.AddRange(array);
            int index = objs.IndexOf(obj);
            if (index != -1) objs.RemoveAt(index);
            return objs.ToArray();
        }

        static public AnimationCurve InvertCurve(AnimationCurve curve)
        {
            Keyframe[] keys = new Keyframe[curve.keys.Length];
            for (int i = 0; i < curve.keys.Length; i++)
            {
                keys[i] = new Keyframe(curve.keys[i].time, 1 - curve.keys[i].value, curve.keys[i].inTangent * -1, curve.keys[i].outTangent * -1);
            }
            return new AnimationCurve(keys);
        }

        static public void EncapsulteRect(ref Rect rect, Rect rect2)
        {
            rect.xMin = Mathf.Min(rect.xMin, rect2.xMin);
            rect.yMin = Mathf.Min(rect.yMin, rect2.yMin);
            rect.xMax = Mathf.Max(rect.xMax, rect2.xMax);
            rect.yMax = Mathf.Max(rect.yMax, rect2.yMax);
        }

		public static Rect ClampRect (Rect baseRect, Rect clampRect)
		{
			Rect rect = new Rect ();
			rect.xMin = Mathf.Max (baseRect.xMin, clampRect.xMin);
			rect.xMax = Mathf.Min (baseRect.xMax, clampRect.xMax);
			rect.yMin = Mathf.Max (baseRect.yMin, clampRect.yMin);
			rect.yMax = Mathf.Min (baseRect.yMax, clampRect.yMax);
			return rect;
		}

		public static bool OverlapRect (Rect baseRect, Rect testRect, out Rect overlapRect)
		{
			overlapRect = new Rect(0, 0, 0, 0);
			if (testRect.xMax > baseRect.xMin && testRect.xMin < baseRect.xMax && testRect.yMax > baseRect.yMin && testRect.yMin < baseRect.yMax)
			{
				overlapRect = ClampRect (baseRect, testRect);
				return true;
			}
			return false;
		}

		public static Rect UniformRectToResolution (Rect rect, Int2 targetRes, Int2 sampleRes, out Int2 samplePos)
		{
			Vector2 ratio = new Vector2 ((float)targetRes.x/sampleRes.x, (float)targetRes.y/sampleRes.y);

			samplePos = new Int2 (Mathf.FloorToInt (rect.x*sampleRes.x), Mathf.FloorToInt (rect.y*sampleRes.y));

			Vector2 size = new Vector2 (Mathf.Ceil (rect.width*sampleRes.x)*ratio.x, Mathf.Ceil (rect.height*sampleRes.y)*ratio.y);
			Vector2 pos = new Vector2 (samplePos.x*ratio.x, samplePos.y*ratio.y);

			return new Rect (pos, size);
		}

        static public AnimationCurve SetAnimationCurveLinear(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve();
            float inTangent, outTangent;
            bool inTangentSet, outTangentSet;
            Vector2 point1, point2, deltaPoint;
            Keyframe key;

            for (int count_key = 0; count_key < curve.keys.Length; ++count_key)
            {
                inTangent = 0.0f;
                outTangent = 0.0f;
                inTangentSet = false;
                outTangentSet = false;
                point1 = Vector2.zero;
                point2 = Vector2.zero;
                deltaPoint = Vector2.zero;
                key = curve[count_key];

                if (count_key == 0) { inTangent = 0.0f; inTangentSet = true; }
                if (count_key == curve.keys.Length - 1) { outTangent = 0.0f; outTangentSet = true; }

                if (!inTangentSet)
                {
                    point1.x = curve.keys[count_key - 1].time;
                    point1.y = curve.keys[count_key - 1].value;
                    point2.x = curve.keys[count_key].time;
                    point2.y = curve.keys[count_key].value;

                    deltaPoint = point2 - point1;

                    inTangent = deltaPoint.y / deltaPoint.x;
                }

                if (!outTangentSet)
                {
                    point1.x = curve.keys[count_key].time;
                    point1.y = curve.keys[count_key].value;
                    point2.x = curve.keys[count_key + 1].time;
                    point2.y = curve.keys[count_key + 1].value;

                    deltaPoint = point2 - point1;

                    outTangent = deltaPoint.y / deltaPoint.x;
                }

                key.inTangent = inTangent;
                key.outTangent = outTangent;
                newCurve.AddKey(key);
            }
            return newCurve;
        }

        static public Vector2 VectorMul(Vector2 p, float v)
        {
            return new Vector2(p.x * v, p.y * v);
        }

        static public Vector3 VectorMul(Vector3 p, float v)
        {
            return new Vector3(p.x * v, p.y * v, p.z * v);
        }

        static public Vector2 VectorDiv(Vector2 p, float v)
        {
            return new Vector2(p.x / v, p.y / v);
        }

        static public Vector3 VectorDiv(Vector3 p, float v)
        {
            return new Vector3(p.x / v, p.y / v, p.z / v);
        }

        static public Vector4[] ColorsToVector4(Color[] colors)
        {
            Vector4[] vColors = new Vector4[colors.Length];

            for (int i = 0; i < colors.Length; i++) vColors[i] = colors[i];
            return vColors;
        }

        static public float Snap(float v, float snapValue)
        {
            return ((int)(v / snapValue) * snapValue);
        }

        static public Vector2 SnapVector2(Vector2 v, float snapValue)
        {
            v.x = ((int)(v.x / snapValue)) * snapValue;
            v.y = ((int)(v.y / snapValue)) * snapValue;
            return v;
        }

        static public Vector3 SnapVector3(Vector3 v, float snapValue)
        {
            v.x = ((int)(v.x / snapValue)) * snapValue;
            v.y = ((int)(v.y / snapValue)) * snapValue;
            v.z = ((int)(v.z / snapValue)) * snapValue;
            return v;
        }

        static public Vector3 SnapRoundVector3(Vector3 v, float snapValue)
        {
            v.x = Mathf.Round(v.x / snapValue) * snapValue;
            v.y = Mathf.Round(v.y / snapValue) * snapValue;
            v.z = Mathf.Round(v.z / snapValue) * snapValue;
            return v;
        }

        static public Vector3 SnapVector3xz(Vector3 v, float snapValue)
        {
            v.x = ((int)(v.x / snapValue)) * snapValue;
            v.z = ((int)(v.z / snapValue)) * snapValue;
            return v;
        }

        static public bool BitSwitch(int v, int index)
        {
            int compareValue = (int)Mathf.Pow(2, index);

            return (v & compareValue) == compareValue;
        }

        static public int SetBitSwitch(int v, int index)
        {
            return (v & (int)Mathf.Pow(2, index));
        }

        static public string CutString(string name, int length)
        {
            if (length > name.Length) return name; else return name.Substring(0, length);
        }
    }

    [Serializable]
    public struct Int2
    {
        public int x, y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Int2(float x, float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public Int2(Vector2 v)
        {
            x = (int)v.x;
            y = (int)v.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        static public Int2 One = new Int2(1, 1);

        public override string ToString()
        {
            return x.ToString() + "x" + y.ToString();
        }
    }

    [Serializable]
    public struct Int3
    {
        public int x, y, z;

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Int3(float x, float y, float z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
        }

        public Int3(Vector3 v)
        {
            x = (int)v.x;
            y = (int)v.y;
            z = (int)v.z;
        }
    }
}