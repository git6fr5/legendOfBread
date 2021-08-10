using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Widget[] widgets;

    public void DeactivateAll() {
        print("Deactivating all");
        for (int i = 0; i < widgets.Length; i++) {
            widgets[i].Deactivate();
        }
    }
}
