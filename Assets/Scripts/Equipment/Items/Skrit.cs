using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skrit : Item {

    /* --- Components --- */
    public Transform shadow;
    public float depth;

    /* --- Variables --- */
    public Vector3 origin;
    [Range(0, 0.005f)] public float speed = 0.002f;

    /* --- Unity --- */
    void Awake() {
        origin = transform.position;
    }

    void Update() {
        Bob();
        Shadow();
    }

    /* --- Methods --- */
    void Bob() {
        if (transform.position.y > origin.y + 0.2f || transform.position.y < origin.y) {
            speed *= -1f;
        }
        transform.position = transform.position + Vector3.up * speed;
    }

    void Shadow() {
        shadow.transform.localPosition = new Vector3(0, origin.y - transform.position.y, 0);
    }

    public override void OnCollected() {
        Destroy(gameObject);
    }

}
