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

    public void SetAnimation(Animation2D newAnimation) {

        // only change the animation if passing in
        // a different animation
        if (currAnimation != newAnimation) {
            if (currAnimation != null) {
                currAnimation.Stop();
            }
            if (newAnimation != null) {
                currAnimation = newAnimation;
                currAnimation.Play();
            }
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
