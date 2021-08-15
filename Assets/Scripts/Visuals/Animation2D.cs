using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation2D : MonoBehaviour
{
    /* --- COMPONENTS --- */
    public Sprite[] frames;

    /* --- VARIABLES --- */
    public float frameRate;
    public bool isLooping = true;

    public int frameIndex = 0;
    public float timer = 0f;
    public bool isPlaying = false;
    public Sprite frame;
    // for derived classes
    public int startIndex = 0; // for derived classes
    [HideInInspector] public int frameCount; // for derived classes

    /* --- UNITY --- */
    void Start() {
        SetLength();
    }

    void Update() {
        if (isPlaying) { Animate(); }
    }

    /* --- VIRTUAL --- */
    public virtual void SetLength() {
        frameCount = frames.Length;
        frame = frames[startIndex];
    }

    /* --- METHODS --- */
    public virtual void Play() {
        frameIndex = startIndex;
        timer = 0f;
        isPlaying = true;
    }

    public void Stop() {
        frameIndex = startIndex;
        timer = 0f;
        isPlaying = false;
    }

    void Animate() {
        timer = timer + Time.deltaTime;
        if (timer > (1f / frameRate)) {
            NextFrame();
            timer = 0f;
        }
    }

    void NextFrame(){
        frameIndex = (frameIndex + 1) % (startIndex + frameCount);
        if (frameIndex == 0) {
            frameIndex += startIndex;
            if (!isLooping) {
                Stop();
            }
        }
        SetFrame();
    }

    void SetFrame() {
        frame = frames[frameIndex];
    }

    public void SnapToFrame(int _frameIndex) {
        frameIndex = _frameIndex;
        frame = frames[frameIndex];
    }
}
