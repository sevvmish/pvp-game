using UnityEngine;

[ExecuteInEditMode]
public class TC_Reporter : MonoBehaviour
{
    static public TC_Reporter instance;
    static bool hasReported;

    int frame;
    public bool report;

    public bool[] channels;
    public float[] timeStart;

    public TC_Reporter()
    {
        if (instance == null) instance = this;
    }
    
    void OnEnable()
    {
        if (instance == null) instance = this;
    }

    void OnDisable()
    {
        if (instance == this) instance = null;
    }
    
    private void LateUpdate()
    {
        if (hasReported)
        {
            UnityEngine.Debug.Log("----------------------------------------------------------> " + frame.ToString() + "\n");
            frame++;
            hasReported = false;
        }
    }
    public static string GetInclination()
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        string text = string.Empty;
        for (int i = 0; i < stackTrace.FrameCount - 2; i++) text += "   ";
        return text;
    }

    public static void Log(string text, int channelIndex = 0)
    {
        if (instance == null) return;
        if (!instance.report) return; 
        
        if (instance.channels == null) instance.channels = new bool[5];
        if (instance.channels.Length != 5) instance.channels = new bool[5];

        if (instance.channels[channelIndex] || channelIndex == -1)
        {
            UnityEngine.Debug.Log(GetInclination() + text + "\n");
            hasReported = true;
        }
    }
    
    public static void BenchmarkStart(int channel = 0)
    {
        if (instance.timeStart == null) instance.timeStart = new float[5];
        if (instance.timeStart.Length != 5) instance.timeStart = new float[5];

        instance.timeStart[channel] = Time.realtimeSinceStartup;
    }

    public static string BenchmarkStop(string text = "", bool logToConsole = true, int channel = 0)
    {
        float time = Time.realtimeSinceStartup - instance.timeStart[channel];
        
        if (logToConsole)
        {
            text = text + " time " + time + " frame " + (1 / time);
            UnityEngine.Debug.Log(text);
        }
        else { text = text + (1 / time).ToString("F0"); }

        return text;
    }
}
