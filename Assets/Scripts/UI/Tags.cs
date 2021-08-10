using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapChannel = Map.Channel;

public class Tags : MonoBehaviour
{
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

}
