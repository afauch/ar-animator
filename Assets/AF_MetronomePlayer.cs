using UnityEngine;
using Crayon;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class AF_MetronomePlayer : MonoBehaviour
{
    public static AF_MetronomePlayer _instance;
    public UnityEvent Tick;

    private bool _indicatorOn = true;

    public double bpm = 120.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    public double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    private void Awake()
    {
        if (!_instance)
            _instance = this;
    }


    void Start()
    {
        accent = signatureHi;
    }

    public void ResumeMetronomePlayback(double newBpm)
    {
        bpm = newBpm;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;
    }

    public void ResumeMetronomeIndicator()
    {
        _indicatorOn = true;
    }

    public void StopMetronomePlayback()
    {
        _indicatorOn = false;
        running = false;
    }

    private void Update()
    {
        // Debug.Log(string.Format("Next Tick: {0}", nextTick));
    }

    // TODO: This is working but is offset from the beat, rather than ON the beat
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;

        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;

                if (_indicatorOn)
                {
                    amp = 1.0F;
                    if (++accent > signatureHi)
                    {
                        accent = 1;
                        amp *= 2.0F;
                    }
                }
                RunIndicator();
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }

    private void RunIndicator()
    {
        Debug.Log("Tick: " + accent + "/" + signatureHi);
        UnityMainThreadDispatcher.Instance().Enqueue(Call());
    }

    private IEnumerator Call()
    {
        if(Tick != null) {
            Tick.Invoke();
        }

        this.gameObject.SetColor(Random.ColorHSV(), 0.0f);
        yield return null;
    }

}