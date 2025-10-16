using System;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;




public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    private void Awake()
    {
        if(_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        foreach (Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    private void Start()
    {
        _audioSource.Play();
    }

    //For playing music on demand
    public void PlayMusic()
    {
        _audioSource.Play();
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
        /*for half beats -> _steps == 0.5
          for 1/4 beats -> _steps == 0.25
          ===============================
          using this function to get the desire elapsed time for different _steps and bpm
        */
    }
    
    public void CheckForNewInterval(float sampledtime)
    {
        if(Mathf.FloorToInt(sampledtime) != _lastInterval)
        {
            _lastInterval = Mathf.FloorToInt(sampledtime);
            _trigger.Invoke();
        }
    }
}