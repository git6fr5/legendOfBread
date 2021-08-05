using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SelectorUI : MonoBehaviour {

    [HideInInspector] public float _outlineWidth;
    [HideInInspector] public Color _outlineColor;

    private Material currMaterial;
    [HideInInspector] public Color highlightColor = new Color(1, 1, 0, 1);
    [HideInInspector] public Color selectedColor = new Color(1, 1, 1, 1);

    /* --- UNITY --- */
    void Awake() {
        CopyMaterial();
    }

    public virtual void OnMouseDown() {
    }

    void OnMouseOver() {
        SetTempOutline(0.1f, highlightColor);
    }

    void OnMouseExit() {
        SetOutline(_outlineWidth, _outlineColor);
    }

    public void SetState(bool active) {
        if (active) {
            SetOutline(0.1f, selectedColor);
        }
        else {
            SetOutline(0f, selectedColor);
        }
    }

    protected void SetOutline(float width, Color color) {
        _outlineWidth = width;
        currMaterial.SetFloat("_OutlineWidth", _outlineWidth);
        _outlineColor = color;
        currMaterial.SetColor("_OutlineColor", _outlineColor);
    }

    protected void SetTempOutline(float width, Color color) {
        currMaterial.SetFloat("_OutlineWidth", width);
        currMaterial.SetColor("_OutlineColor", color);
    }

    protected void CopyMaterial() {
        Material newMaterial = new Material(GetComponent<SpriteRenderer>().material);
        GetComponent<SpriteRenderer>().material = newMaterial;
        currMaterial = newMaterial;
    }

}
