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

    private bool _isRecording = false;
    private bool _isGrabbed = false;

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
                PlayRecording();
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

    private void PlayRecording()
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
