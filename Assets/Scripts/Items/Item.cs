using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    public enum Category {

        SKRIT,
        WEAPON,
        POTION,

    }

    public Category category;

    Vector3 origin;
    float s = 0.002f;

    public Transform shadow;
    public Vector3 shadowPos;

    // if this is not skrit then it is the value it can be sold for
    // otherwise it is just the value of skrit
    // if -1 then this can't be sold
    public int value;

    void Awake() {

        origin = transform.position;
        shadowPos = shadow.transform.position;

    }

    void Update() {

        if (transform.position.y > origin.y + 0.2f || transform.position.y < origin.y) {

            s *= -1f;

        }

        transform.position = transform.position + Vector3.up * s;
        shadow.transform.position = shadowPos;

    }

}
