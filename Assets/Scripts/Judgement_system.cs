using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class JudgementSystem : MonoBehaviour
{
    [Header("Beat / Timing")]
    public BeatClock clock = new BeatClock();   // instance-based clock
    [Tooltip("On-beat tolerance (ms) for quantization & combo gating")]
    public double toleranceMs = 70.0;

    [Header("Combo / Tricks")]
    public TrickMatcher matcher = new TrickMatcher();
    public FourBeatGroup group = new FourBeatGroup();

    [Header("Input (keyboard lanes)")]
    public KeyCode lane0Key = KeyCode.W;
    public KeyCode lane1Key = KeyCode.S;
    public KeyCode lane2Key = KeyCode.A;
    public KeyCode lane3Key = KeyCode.D;

    [Header("Audio")]
    public AudioSource music;          // assign in Inspector
    public double startLeadIn = 0.25;  // seconds before playback
    public double bpm = 120.0;         // song tempo

    void Start()
    {
        // Ensure audio data is ready
        if (music != null && music.clip != null)
            music.clip.LoadAudioData();

        // Schedule start on the DSP clock
        clock.dspStart = AudioSettings.dspTime + startLeadIn;
        if (music != null) music.PlayScheduled(clock.dspStart);

        // Optional: subscribe in code to respond to any combo
        if (matcher != null)
        {
            matcher.OnTrick = (c) =>
            {
                Debug.Log($"TRICK triggered: {c.name}");
                // TODO: call your score system, play SFX, trigger VFX, etc.
                // Example: FindObjectOfType<ScoreSystem>()?.Add( c.name, c.laneSeq.Length * 100 );
            };
        }
    }

    void Awake()
    {
        // Keep BeatClock's internal tolerance in sync (Quantize uses clock.tolMs)
        clock.beatDur = 60.0 / bpm;
        clock.tolMs = toleranceMs;
    }

    void Update()
    {
        // Example keyboard polling; if you use the new Input System, call OnLaneInput from your action callbacks instead.
        if (Input.GetKeyDown(lane0Key)) OnLaneInput(0);
        if (Input.GetKeyDown(lane1Key)) OnLaneInput(1);
        if (Input.GetKeyDown(lane2Key)) OnLaneInput(2);
        if (Input.GetKeyDown(lane3Key)) OnLaneInput(3);
    }

    // Call this from keyboard/touch/controller events. Do NOT poll "every beat"—we only judge when input happens.
    public void OnLaneInput(int lane)
    {
        double t = clock.SongTime();                        // use instance clock (DSP-based)
        var (ok, bi, errMs) = clock.Quantize(t);            // Quantize uses clock.tolMs internally                               
        // off-beat -> no combo progress
        // enforce beat alignment for current step

        if (!ok) return;
        if (bi != group.anchorBeat + group.step)
        {
            // strict: reset and start from this beat as new anchor
            matcher.ResetGroup(group, bi);
            Debug.Log("Reset");
        }

        matcher.Accept(group, lane, bi);  // records lane, advances step, fires OnTrick at step==4
    }
}

[System.Serializable]
public class TrickMatcher
{
    public List<Combos> tricks = new();
    public Action<Combos> OnTrick;
    [SerializeField] private Character_Controller controller;


    // Keep a rolling buffer of last 4 lanes within the group:
    private int[] last4 = new int[4];

    public void ResetGroup(FourBeatGroup g, int anchorBeat)
    {
        g.anchorBeat = anchorBeat;
        g.step = 0;
        // clear last4 so a new group starts clean
        for (int i = 0; i < 4; i++) last4[i] = -1;
    }

    // Call on each on-beat input; returns true if it consumed the step
    public bool Accept(FourBeatGroup g, int lane, int bi /*for resetting*/)
    {

        // record lane at current step index, then advance
        last4[g.step] = lane;
        g.step++;
        controller.Move(lane);

        if (g.step == 4)
        {
            // finalize: check against all tricks
            foreach (var tr in tricks)
            {
                bool ok = true;
                for (int i = 0; i < 4; i++)
                {
                    int need = tr.laneSeq[i];
                    if (need >= 0 && need != /* your recorded lanes[i] */ last4[i]) { ok = false; break; }
                }
                if (ok) {
                    // Code-driven callback
                    OnTrick?.Invoke(tr);
                    // Inspector-driven actions (VFX/SFX/Score/etc.)
                    tr.onTriggered?.Invoke();
                }
            }
            ResetGroup(g, bi); // group complete
        }
        return true;
    }
}

public class FourBeatGroup
{
    public int anchorBeat = 1;     // b0
    public int step = 0;           // 0..3 (which beat inside the group)
}

[System.Serializable]
public class BeatClock {
    // Feed from your tempo map (handle BPM changes by swapping beatDur on the fly)
    [SerializeField] private double bpm;   // seconds per beat (e.g., 120 BPM => 0.5)
    public double beatDur;
    public double dspStart;                // set when you PlayScheduled (AudioSettings.dspTime + leadIn)
    public double offsetMs;                // calibration (positive delays judgement clock)
    public double tolMs;            // on-beat tolerance in milliseconds

    public double SongTime() => AudioSettings.dspTime - dspStart + offsetMs / 1000.0;


    public int NearestBeatIndex(double t) => (int)Math.Round(t / beatDur);
    public double BeatTime(int idx) => idx * beatDur;

    // returns (isOnBeat, beatIndex, errorMs)
    public (bool, int, double) Quantize(double t) {
        int bi = NearestBeatIndex(t);
        double err = t - BeatTime(bi);
        double ms = Math.Abs(err) * 1000.0;
        return (ms <= tolMs, bi, err * 1000.0);
    }
}

[System.Serializable]
public class Combos {
    public string name;
    public int[] laneSeq;        // length 4, e.g., {0,1,2,3}; use -1 as wildcard
    [Tooltip("Invoked when this combo is successfully matched")]
    public UnityEvent onTriggered;
}