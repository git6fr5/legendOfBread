using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CutsceneEditor : MonoBehaviour
{

	public Cutscene cutscene;

	string path = "Assets/Resources/Dialogue/test.txt";

    // Start is called before the first frame update
    void Start() {
        StreamReader reader = new StreamReader(path);
        string scene = reader.ReadToEnd();
        print(scene);
        cutscene.Run(scene);
    }

    // Update is called once per frame
    void Update() {

    }
}
