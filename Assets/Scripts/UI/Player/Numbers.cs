using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numbers : MonoBehaviour {
    
    /* --- Components --- */
    public Sprite[] numbers;
    public SpriteRenderer ones;
    public SpriteRenderer tenths;
    public SpriteRenderer hundredths;

    /* --- Methods --- */
    void SetValue(int value) {

        if (value > 999) {
            value = 999;
        }

        int a = (int)Mathf.Floor(inventory.skrit / 100f);
        int b = (int)Mathf.Floor((inventory.skrit - a * 100f) / 10f);
        int c = (int)Mathf.Floor(inventory.skrit - a * 100f - b * 10f);

        ones.sprite = numbers[a];
        tehths.sprite = numbers[b];
        hundredths.sprite = numbers[c];

    }

}
