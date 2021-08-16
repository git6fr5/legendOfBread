using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Category = Item.Category;

public class Inventory : MonoBehaviour
{
    public int skrit = 0;

    public void Add(Item item) {

        switch (item.category) {
            case (Category.SKRIT):
                AddSkrit(item.value);
                break;
            default:
                break;
        }

        Destroy(item.gameObject);

    }

    public void AddSkrit(int value) {
        skrit += value;
    }

}
