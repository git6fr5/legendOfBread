using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapChannel = Map.Channel;
using Challenge = Room.Challenge;

public class Tools : MonoBehaviour
{
    public Select emptyBrush;
    Select[] brushes = new Select[0];
    public ChallengeLayout challengeLayout;

    public Sprite[] shapeTags;
    public SpriteRenderer shapeTag;

    public Sprite[] pathTags;
    public SpriteRenderer pathTag;

    public Sprite[] challengeTags;
    public SpriteRenderer challengeTag;

    public Mob[] mobs;
    public Trap[] traps;

    void Start() {
        print(traps[0].ID);
    }

    public void SetTools(Challenge roomChannel) {

        for (int i = brushes.Length - 1; i >= 0; i--) {
            print(i);
            Destroy(brushes[i].gameObject);
        }

        if (roomChannel == Challenge.COMBAT) {
            print("Mob");
            brushes = new Select[mobs.Length];
        }

        else if (roomChannel == Challenge.TRAP) {
            print("Trap");
            brushes = new Select[traps.Length];
            for (int i = 0; i < traps.Length; i++) {
                Vector3 position = emptyBrush.transform.position + new Vector3(i % 3, Mathf.Floor(i / 3), 0);
                Select newBrush = Instantiate(emptyBrush.gameObject, position, Quaternion.identity, transform).GetComponent<Select>();
                newBrush.GetComponent<SpriteRenderer>().sprite = traps[i].state._renderer.defaultSprite;
                newBrush.index = traps[i].ID;
                brushes[i] = newBrush;
            }       
        }

        challengeLayout.SetChallengeLayout(brushes);

    }

}
