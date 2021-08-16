using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skrit : MonoBehaviour
{
    public Sprite[] numbers;

    public SpriteRenderer _0;
    public SpriteRenderer _00;
    public SpriteRenderer _000;


    public Inventory inventory;


    void Update() {

        string skritString = ".000";
        if (inventory.skrit > 0) {
            skritString = (inventory.skrit / 1000f).ToString(".###");
        }

        if (inventory.skrit > 999) {
            inventory.skrit = 999;
        }
        int _a = (int)Mathf.Floor(inventory.skrit / 100f);
        int _b = (int)Mathf.Floor((inventory.skrit - _a * 100f) / 10f);
        int _c = (int)Mathf.Floor(inventory.skrit - _a * 100f -_b * 10f);

        print(skritString);
        print(_c);
        _0.sprite = numbers[_a];
        _00.sprite = numbers[_b];
        _000.sprite = numbers[_c];

    }

}
