using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Note = Sheet.Note;
using Tone = Sheet.Tone;
using Value = Sheet.NoteLength;

public class MusicEditor : MonoBehaviour {
    public AudioSource audioSource;
    float sampleRate;
    int endPhase = 0;

    public Note root;
    public int bpm;
    float secondsPerQuarterNote;

    // the sheet
    List<Tone> notes = new List<Tone>();
    List<Value> lengths = new List<Value>();
    float startTime = 0;
    int index = 0;

    void Awake() {
        sampleRate = AudioSettings.outputSampleRate;
        print(sampleRate);
        secondsPerQuarterNote = 60f / bpm;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !audioSource.isPlaying) {
            startTime = (float)AudioSettings.dspTime;
            var _sheet = Sheet.GetBar();
            notes = _sheet.Item1; lengths = _sheet.Item2;
            audioSource.Play();
        }
        if (index >= notes.Count) {
            index = 0;
            audioSource.Stop();
        }
    }

    void OnAudioFilterRead(float[] data, int channels) {

        // increment the time
        float time = (float)AudioSettings.dspTime - startTime;
        // print(time);

        // check if we need to move to the next note
        Value length = lengths[index];
        float noteLength = Sheet.LengthMultipliers[length];
        if (time >= noteLength * secondsPerQuarterNote) {
            // reset the end phase of the previous note
            // endPhase = 0;
            startTime = (float)AudioSettings.dspTime;
            index++;
        }

        if (index >= notes.Count) {
            return;
        }

        // play the current note
        float freq = Sheet.NoteFrequencies[Note.A] * Sheet.MajorScale[notes[index]];
        print(freq);
        var _data = Wave.Overtones(sampleRate, data, channels, freq, endPhase);
        data = _data.Item1;
        endPhase = _data.Item2;
    }
}
