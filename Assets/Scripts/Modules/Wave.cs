using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

    public static (float[], int) Sine(float sampleRate, float[] data, int n, float frequency, int phaseIndex) {

        float volume = 0.05f;

        for (int i = 0; i < data.Length; i += n) {
            for (int j = 0; j < n; j++) {
                data[i + j] = volume * Mathf.Sin(2 * phaseIndex * Mathf.PI * frequency / sampleRate);
            }
            phaseIndex++;
            phaseIndex = phaseIndex % (int)(sampleRate / frequency);

        }
        return (data, phaseIndex);
    }

    public static (float[], int) Basic(float sampleRate, float[] data, int n, float frequency, int phaseIndex) {

        float volume = 0.05f;

        for (int i = 0; i < data.Length; i += n) {
            for (int j = 0; j < n; j++) {
                float root = volume * Mathf.Sin(2 * phaseIndex * Mathf.PI * frequency / sampleRate);
                float octave = volume / 2 * Mathf.Sin(2 * phaseIndex * Mathf.PI * 2 * frequency / sampleRate);
                float fifth = volume / 3 * Mathf.Sin(2 * phaseIndex * Mathf.PI * 3 * frequency / sampleRate);
                data[i + j] = 1 / (1 + 1/2 + 1/3) * (root + octave + fifth);
            }
            phaseIndex++;
            phaseIndex = phaseIndex % (int)(sampleRate / frequency);

        }
        return (data, phaseIndex);
    }

    public static (float[], int) Overtones(float sampleRate, float[] data, int n, float frequency, int phaseIndex) {

        float volume = 0.05f;

        for (int i = 0; i < data.Length; i += n) {
            for (int j = 0; j < n; j++) {
                float val = 0;
                float factor = 0;
                for (int k = 1; k < 20; k++) {
                    val += volume * Mathf.Exp(-(k-1)) * Mathf.Sin(2 * phaseIndex * Mathf.PI * k * frequency / sampleRate);
                    factor += Mathf.Exp(-(k - 1));
                }
                data[i + j] = factor * val;
            }
            phaseIndex++;
            phaseIndex = phaseIndex % (int)(sampleRate / frequency);

        }
        return (data, phaseIndex);
    }

}