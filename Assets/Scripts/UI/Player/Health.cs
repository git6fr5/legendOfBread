using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public State state;

    public SpriteRenderer defaultHeartRenderer;
    SpriteRenderer[] heartRenderers;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Start() {
        heartRenderers = new SpriteRenderer[state.maxHealth];
        for (int i = 0; i < heartRenderers.Length; i++) {
            SpriteRenderer _heartRenderer = Instantiate(defaultHeartRenderer.gameObject, new Vector3(i, 0, 0), Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            _heartRenderer.transform.localPosition = new Vector3(i, 0, 0);
            heartRenderers[i] = _heartRenderer;
        }
    }

    void Update() {

        for (int i = 0; i < heartRenderers.Length; i++) {
            if (i < state.currHealth) {
                // full heart
                heartRenderers[i].sprite = fullHeart;
            }
            else {
                // emptyHeart
                heartRenderers[i].sprite = emptyHeart;
            }
        }

    }

}
