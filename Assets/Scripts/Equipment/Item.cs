using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public enum Category {
        SKRIT,
        WEAPON,
        POTION,
        categoryCount
    }
    
    // if this is not skrit then it is the value it can be sold for
    // otherwise it is just the value of skrit
    // if -1 then this can't be sold
    public int value;

    public virtual void OnCollected() {

    }

}
