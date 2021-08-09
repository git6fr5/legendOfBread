using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer2D : MonoBehaviour
{
    /* --- COMPONENTS --- */
    public SpriteRenderer spriteRenderer;
    // defaults
    [Space(5)] [Header("Default")]
    public Sprite defaultSprite;
    public Material defaultMaterial;

    /* --- VARIABLES --- */
    public Animation2D currAnimation;
    public bool render = true;

    /* --- UNITY --- */
    void Start() {
    }

    void Update() {
        SetSprite(null);
    }

    /* --- METHODS --- */
    public void SetSprite(Sprite _sprite) {
        if (_sprite == spriteRenderer.sprite && _sprite != null){
            return;
        }
        if (_sprite != null && currAnimation != null) {
            currAnimation.Stop();
            spriteRenderer.sprite = _sprite;
            return;
        }
        else if (currAnimation != null && currAnimation.isPlaying) {
            spriteRenderer.sprite = currAnimation.frame;
            return;
        }
        else if (spriteRenderer.sprite == null) {
            spriteRenderer.sprite = defaultSprite;
            return;
        }
    }

    // new animation should not be null
    public void PlayAnimation(Animation2D newAnimation) {
        // only change animation if we're getting a new animation
        if (newAnimation != currAnimation) {
            currAnimation = newAnimation;
        }
        // if the animation we're in is stopped, then we kick start it
        if (!currAnimation.isPlaying) {
            currAnimation.Play();
        }
    }

    public void StopAnimation() {
        if (currAnimation!= null) {
            currAnimation.Stop();
        }
    }

    public void SetMaterial(Material _material) {
        if (_material != null) { 
            spriteRenderer.material = _material;
            return;
        }
        spriteRenderer.material = defaultMaterial;
    }
}
