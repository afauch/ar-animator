using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Crayon;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class AF_Metronome : MonoBehaviour
{

    public static AF_Metronome _instance;

    // Metronome logic adapted from Javon Harper
    // Github: javonharper

    public double _bpm = 120.0d;
    private bool _isGrabbed;
    private bool _isListening;

    private double MILLISECONDS_IN_A_MINUTE = 60.0d;
	public List<double> _times;

    VRTK_InteractableObject _vO;
    VRTK_ControllerEvents _vE;
    AF_MetronomePlayer _metronomePlayer;

    private void Awake()
    {
        if(!_instance)
            _instance = this;
    }

    private void Start()
    {

        _times = new List<double>();
        _isListening = false;

        _vO = GetComponent<VRTK_InteractableObject>();
        _vO.InteractableObjectGrabbed += OnGrab;
        _vO.InteractableObjectUngrabbed += OnUngrab;

        _metronomePlayer = GetComponent<AF_MetronomePlayer>();

    }

    private void Update()
    {
    }

    private void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        // init the times list
        _metronomePlayer.StopMetronomePlayback();
        clearTimes();
        _isGrabbed = true;
        Debug.Log(e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>());
        _vE = e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>();
        _vE.TriggerClicked += OnTriggerClicked;
        _vE.TriggerUnclicked += OnTriggerUnclicked;
    }

    private void OnUngrab(object sender, InteractableObjectEventArgs e)
    {
        _metronomePlayer.ResumeMetronomeIndicator();
        _isGrabbed = false;
        _vE.TriggerClicked -= OnTriggerClicked;
        _vE.TriggerUnclicked -= OnTriggerUnclicked;
    }

    private void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        double t = recordTime();
        Debug.Log("OnTriggerClicked Called at time " + t);
        _bpm = getBpm();
        _metronomePlayer.ResumeMetronomePlayback(_bpm);
    }

    private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
    }

    public double recordTime()
    {
        double time = (double)Time.time;
        _times.Add(time);
        return time;
    }

    public double getBpm()
    {
        List<double> deltas = getDeltas();
        return calculateBpm(deltas);
    }

    public void clearTimes()
    {
        _times.Clear();
    }

    private List<double> getDeltas()
    {
        List<double> deltas = new List<double>();

        for (int i = 0; i < _times.Count - 1; i++)
        {
            double delta = _times[i + 1] - _times[i];
            deltas.Add(delta);
        }

        return deltas;
    }

    private double calculateBpm(List<double> deltas)
    {
        double sum = 0L;

        foreach(double delta in deltas)
        {
            sum = sum + delta;
        }

        double average = sum / deltas.Count;

        double bpm = (double)(MILLISECONDS_IN_A_MINUTE / average);

        Debug.Log("Calculated BPM as " + bpm);

        return bpm;
    }

}
