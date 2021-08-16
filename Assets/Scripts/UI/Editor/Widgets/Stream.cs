using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : Widget {

    public bool isActive = false;

    public PixelFont font;

    public string text;

    public SpriteRenderer[] characters;

    public Material textMaterial;

    void Awake() {

        for (int i = 0; i < characters.Length; i++) {

            characters[i].transform.localPosition = new Vector3(0.5f * i, 0f, 0f);
            characters[i].material = textMaterial;
        }

    }

    void Update() {

        if (isActive) {

            foreach (char c in Input.inputString) {
                print(c);

                if (c == '\b' && text.Length != 0) {
                    text = text.Substring(0, text.Length - 1);
                }
                else if (text.Length < characters.Length && font.fontDict.ContainsKey(c)) {

                    text = text + c;

                }

            }

            if (Input.GetMouseButtonDown(1)) {

                isActive = false;

            }

        }

        for (int i = 0; i < characters.Length; i++) {
            if (i < text.Length) {
                characters[i].sprite = font.fontDict[text[i]];

            }
            else {

                characters[i].sprite = font.fontDict['#'];

            }

        }

    }

    public override void Activate() {

        isActive = true;

    }
}