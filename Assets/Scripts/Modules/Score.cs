using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score {

    public enum NoteLength {
        WHOLE, HALF, QUARTER, EIGTH, SIXTEENTH, noteLengthCount
    }

    public static Dictionary<NoteLength, float> LengthMultipliers = new Dictionary<NoteLength, float>() {
        {NoteLength.WHOLE, 4f },
        {NoteLength.HALF, 2f },
        {NoteLength.QUARTER, 1f },
        {NoteLength.EIGTH, 0.5f },
        {NoteLength.SIXTEENTH, 0.25f },
    };

    public enum Tone {
        REST, P1, m2, M2, m3, M3, P4, P5, m6, M6, m7, M7, P8, m9, M9, toneCount
    }

    public static Dictionary<Tone, float> MajorScale = new Dictionary<Tone, float>(){
        { Tone.REST, 0f },
        { Tone.P1 , 1f },
        { Tone.M2 , 9f/8f },
        { Tone.M3 , 5f/4f },
        { Tone.P4 , 4f/3f },
        { Tone.P5 , 3f/2f },
        { Tone.M6 , 5f/3f },
        { Tone.M7 , 15f/8f },
        { Tone.P8 , 2f }
    };

    public enum Note { A, B, C, D, E, F, G, noteCount }

    public static Dictionary<Note, float> NoteFrequencies = new Dictionary<Note, float>() {
        { Note.A, 440f },
        { Note.B, 493.88f},
        { Note.C, 523.25f},
        { Note.D, 587.33f},
        { Note.E, 659.25f},
        { Note.F, 698.46f},
        { Note.G, 783.99f },
    };

    public static (List<Tone>, List<NoteLength>) GetBar() {

        float barLengthLeft = 4f;
        List<Tone> tones = new List<Tone>();
        List<NoteLength> lengths = new List<NoteLength>();

        while (barLengthLeft > 0f) {
            
            Tone tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            while (!MajorScale.ContainsKey(tone)) {
                tone = (Tone)Random.Range(0, (int)Tone.toneCount);
            }
            tones.Add(tone);

            NoteLength noteLength = (NoteLength)Random.Range(2, (int)NoteLength.noteLengthCount);
            lengths.Add(noteLength);
            barLengthLeft -= LengthMultipliers[noteLength];//
        }

        return (tones, lengths);

    }

}
