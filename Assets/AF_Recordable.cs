using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

[RequireComponent(typeof(VRTK_InteractableObject))]
public class AF_Recordable : MonoBehaviour {

    VRTK_InteractableObject _vO;
    VRTK_ControllerEvents _vE;

    // TODO: It would be very simple to create 
    // subscription events for quarter, half, triplets, etc
    // to create a time grid

    public bool _snapToMetronome = true;
    private bool _isRecording = false;
    private bool _isGrabbed = false;
    private bool _shouldPlay = false;

    List<Vector3> _positions = new List<Vector3>();
    List<Quaternion> _rotations = new List<Quaternion>();

    int _frameCount = 0;
    int _currentPlaybackFrame = 0;

    private void Start()
    {
        _vO = GetComponent<VRTK_InteractableObject>();
        _vO.InteractableObjectGrabbed += OnGrab;
        _vO.InteractableObjectUngrabbed += OnUngrab;
    }


    private void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        _isGrabbed = true;
        _shouldPlay = false;
        Debug.Log(e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>());
        _vE = e.interactingObject.GetComponentInParent<VRTK_ControllerEvents>();
        _vE.TriggerClicked += OnTriggerClicked;
        _vE.TriggerUnclicked += OnTriggerUnclicked;
    }

    private void OnUngrab(object sender, InteractableObjectEventArgs e)
    {
        _isGrabbed = false;
        _vE.TriggerClicked -= OnTriggerClicked;
        _vE.TriggerUnclicked -= OnTriggerUnclicked;
    }

    private void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("OnGripClicked Called");
        StartRecording();
    }

    private void OnTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        StopRecording();
        _vO.OnInteractableObjectUngrabbed(new InteractableObjectEventArgs());

        if(_snapToMetronome)
        {
            // subscribe to the next metronome event
            AF_MetronomePlayer._instance.Tick.AddListener(StartPlayback);
        } else
        {
            // By default, start playing right away
            _shouldPlay = true;
        }

    }

    private void Update()
    {
        if(_isRecording)
        {
            _positions.Add(this.transform.position);
            _rotations.Add(this.transform.rotation);
            _frameCount += 1;
        }
        else
        {
            if(!_isGrabbed)
            {
                if(_shouldPlay)
                {
                    ContinuePlayback();
                }
            }
        }
    }

    public void StartRecording()
    {

        _positions = new List<Vector3>();
        _rotations = new List<Quaternion>();
        _frameCount = 0;
        _currentPlaybackFrame = 0;

        Debug.Log("StartRecording called");
        _isRecording = true;
    }

    public void StopRecording()
    {
        Debug.Log("StopRecording called");
        _isRecording = false;
    }

    private void StartPlayback()
    {
        _currentPlaybackFrame = 0;
        _shouldPlay = true;

        // Unsubscribe from the Tick listener as soon as the animation starts
        // AF_MetronomePlayer._instance.Tick.RemoveListener(StartPlayback);
    }

    private void ContinuePlayback()
    {
        if (_frameCount == 0)
        {
            return;
        } else
        {
            this.transform.SetPositionAndRotation(_positions[_currentPlaybackFrame], _rotations[_currentPlaybackFrame]);
            if (_currentPlaybackFrame == (_frameCount - 1))
            {
                _currentPlaybackFrame = 0;
            } else
            {
                _currentPlaybackFrame += 1;
            }

        }
    }
}
