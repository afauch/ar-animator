using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.UnityEventHelper;

public class AF_Recordable : MonoBehaviour {

    VRTK_InteractableObject _vO;
    VRTK_ControllerEvents _vE;

    private bool _isRecording = false;

    List<Vector3> _positions = new List<Vector3>();
    List<Quaternion> _rotations = new List<Quaternion>();

    int _frameCount = 0;
    int _currentPlaybackFrame = 0;

    private void Start()
    {
        _vO = GetComponent<VRTK_InteractableObject>();
        _vO.InteractableObjectGrabbed += OnGrab;
    }

    private void OnGrab(object sender, InteractableObjectEventArgs e)
    {
        Debug.Log(e.interactingObject);
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
            PlayRecording();
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
