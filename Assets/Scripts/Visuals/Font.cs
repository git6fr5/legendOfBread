using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelFont : MonoBehaviour
{

    public Sprite[] fontSprites;


    public string ascii = "#ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public Dictionary<char, Sprite> fontDict;

    void Awake() {

        fontDict = new Dictionary<char, Sprite>();

        int length = (int)Mathf.Min(ascii.Length, fontSprites.Length);
        print(ascii.Length);

        for (int i = 0; i < length; i++) {

            print(ascii[i]);
            fontDict.Add(ascii[i], fontSprites[i]);

        }

    }

}
