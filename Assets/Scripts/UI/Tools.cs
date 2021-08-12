using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapChannel = Map.Channel;
using RoomChannel = Room.Channel;
using Challenge = Room.Challenge;

public class Tools : MonoBehaviour
{
    public Select emptyBrush;
    Select[] brushes = new Select[0];
    public ChallengeLayout challengeLayout;

    public SpriteRenderer shapeTag;
    public Sprite[] shapeTags;

    public SpriteRenderer pathTag;
    public Sprite[] pathTags;

    public SpriteRenderer challengeTag;
    public Sprite[] challengeTags;

    public Mob[] mobs;
    public Trap[] traps;

    void Start() {
    }

    public void SetTools(Challenge challenge) {

        for (int i = brushes.Length - 1; i >= 0; i--) {
            print(i);
            Destroy(brushes[i].gameObject);
        }

        if (challenge == Challenge.COMBAT) {
            print("Mob");
            brushes = new Select[mobs.Length];
            for (int i = 0; i < mobs.Length; i++) {
                Vector3 position = emptyBrush.transform.position + new Vector3(i % 3, Mathf.Floor(i / 3), 0);
                Select newBrush = Instantiate(emptyBrush.gameObject, position, Quaternion.identity, transform).GetComponent<Select>();
                newBrush.GetComponent<SpriteRenderer>().sprite = mobs[i].state._renderer.defaultSprite;
                newBrush.index = mobs[i].id;
                brushes[i] = newBrush;
            }
        }

        else if (challenge == Challenge.TRAP) {
            print("Trap");
            brushes = new Select[traps.Length];
            for (int i = 0; i < traps.Length; i++) {
                Vector3 position = emptyBrush.transform.position + new Vector3(i % 3, Mathf.Floor(i / 3), 0);
                Select newBrush = Instantiate(emptyBrush.gameObject, position, Quaternion.identity, transform).GetComponent<Select>();
                newBrush.GetComponent<SpriteRenderer>().sprite = traps[i].state._renderer.defaultSprite;
                newBrush.index = traps[i].id;
                brushes[i] = newBrush;
            }       
        }

        challengeLayout.SetChallengeLayout(brushes);

    }

    public GameObject[] GetChallengeObjects(Challenge challenge) {

        GameObject[] objects = new GameObject[0];
        if (challenge == Challenge.COMBAT) {
            objects = new GameObject[99];
            for (int i = 0; i < mobs.Length; i++) {
                objects[mobs[i].id] = mobs[i].gameObject;
            }
        }
        if (challenge == Challenge.TRAP) {
            objects = new GameObject[99];
            for (int i = 0; i < traps.Length; i++) {
                objects[traps[i].id] = traps[i].gameObject;
            }
        }

        return objects;
    }
}
