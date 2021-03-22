using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class camera_shake : MonoBehaviour
{
    public static camera_shake Instance { get; set; }
    private CinemachineVirtualCamera snm;
    private CinemachineBasicMultiChannelPerlin BaseShaker;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        snm = this.GetComponent<CinemachineVirtualCamera>();
        BaseShaker = snm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();        
    }

    public void SimpleHit(float time_delay, float how_long, float distance_koeff, float amplitude, float frequency)
    {
        StartCoroutine(DoShake(time_delay, how_long, distance_koeff, amplitude, frequency));
    }

    IEnumerator DoShake(float time_delay, float how_long, float distance_koeff, float amplitude, float frequency)
    {
        print(distance_koeff.ToString("f1") + " = koef");
        if (distance_koeff>=1)
        {
            yield break;
        }

        yield return new WaitForSeconds(time_delay);
        
        amplitude = (1f - distance_koeff) * amplitude;
        frequency = (1f - distance_koeff) * frequency;

        float delta = 0.2f;
        float cur_ampl = amplitude, cur_freq = frequency;
        for (float i = 0; i < how_long; i+=delta)
        {
            BaseShaker.m_AmplitudeGain = cur_ampl;
            BaseShaker.m_FrequencyGain = cur_freq;
            
            cur_ampl -= amplitude * delta / how_long;
            cur_freq -= frequency * delta / how_long;
            yield return new WaitForSeconds(delta);
        }

        BaseShaker.m_AmplitudeGain = 0;
        BaseShaker.m_FrequencyGain = 0;
                
    }

}
