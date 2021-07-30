using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour {

    /* --- COMPONENTS --- */
    [Space(5)]
    [Header("Dialogue")]
    public Text dialogueText;

    /* --- VARIABLES --- */
    IEnumerator runCoroutine = null; // will be null if no cutscene running
    public bool isRunningCommand = false; // will be false if no individual command is running

    char commandBreak = '\n'; // for new lines in dialogue
    private Dictionary<string, Func<string, bool>> cutsceneDict; // holds the commands
    private float talkDelay = 0.075f; // time between letters appearing;
    private float talkBuffer = 1.0f; // time before the next line starts
    public int charactersPerLine = 40; // characters per line in the dialogue box

    /* --- UNITY --- */
    // runs once on compilation
    void Awake() {
        InitializeCutsceneDictionary();
    }

    /* --- METHODS --- */
    // initialize the commands in the cut scene 
    void InitializeCutsceneDictionary() {
        cutsceneDict = new Dictionary<string, Func<string, bool>>() {
            { "talk", Talk  }
        };
    }

    // run a cut scene
    public void Run(string cutsceneString) {
        // check if a cut scene is already running
        if (runCoroutine == null) {
            // get the commands from the cut scene text
            List<string> commandList = SplitCommands(cutsceneString);
            // run the cut scene
            runCoroutine = IERun(commandList);
            StartCoroutine(runCoroutine);
        }
    }

    // run the commands in a cut scene
    private IEnumerator IERun(List<string> commandList) {
        for (int i = 0; i < commandList.Count; i++) {
            // set running command to true if running a command
            isRunningCommand = Command(commandList[i]);
            // wait until the command is over to start the next command
            yield return new WaitUntil(() => isRunningCommand == false);
        }

        yield return null;
    }

    // figures out which command to run using the dictionary
    bool Command(string commandString) {
        string actionString = SubString(commandString, 0, 4);
        string outputString = SubString(commandString, 5, commandString.Length);
        return cutsceneDict[actionString](outputString);
    }

    // run through the talk command
    bool Talk(string outputString) {
        IEnumerator talkCoroutine = IETalk(talkDelay, outputString);
        StartCoroutine(talkCoroutine);
        return true;
    }

    // run through each letter of the talk command
    private IEnumerator IETalk(float delay, string outputString) {
        // the list of characters that have been printed
        List<char> partialCharList = new List<char>();
        // store the time in between letters locally
        // so that we can adjust its rate easily
        float localDelay = delay;

        // print each letter one at a time
        for (int i = 0; i < outputString.Length; i++) {

            if (outputString[i] == '[') {
                // get the string in between the square brackets
                string outputTrail = SubString(outputString, i + 1, outputString.Length);
                int endIndex = FindInString(outputTrail, ']');
                string delayString = SubString(outputTrail, 0, endIndex);

                // get the delay factor from that string
                float delayRate = float.Parse(delayString);
                localDelay = delay / delayRate;

                // skip over printing the square brackets
                i = (i + 1) + (endIndex + 1);
            }

            // add the next letter to the string
            partialCharList.Add(outputString[i]);
            string partialString = new string(partialCharList.ToArray());

            // skip waiting on spaces
            if (outputString[i] == ' ') {
                yield return new WaitForSeconds(0);
            }
            // wait before printing the next letter
            else {
                yield return new WaitForSeconds(localDelay);
            }

            // print the string to the dialogue box
            dialogueText.text = partialString;
        }

        // wait a bit before exiting the current text 
        // to give some time to read
        yield return new WaitForSeconds(talkBuffer);

        // when this command has completed
        // switch this to inform the overhead
        // to run the next command
        isRunningCommand = false;
        yield return null;
    }

    // split the cut scene string into individual strings
    List<string> SplitCommands(string cutsceneString) {
        List<string> commandList = new List<string>();
        List<char> commandCharList = new List<char>();

        for (int i = 0; i < cutsceneString.Length; i++) {
            if (cutsceneString[i] != commandBreak)
                commandCharList.Add(cutsceneString[i]);
            else {
                string commandString = new string(commandCharList.ToArray());
                commandList.Add(commandString);
                commandCharList = new List<char>();
            }
        }

        return commandList;
    }

    // gets a section of the input string
    string SubString(string inputString, int start, int end) {
        List<char> outputCharList = new List<char>();
        for (int i = start; i < end; i++) {
            outputCharList.Add(inputString[i]);
        }
        return new string(outputCharList.ToArray());
    }

    // finds the index of the first occurrence of a character
    int FindInString(string inputString, char character) {
        for (int i = 0; i < inputString.Length; i++) {
            if (inputString[i] == character) {
                return i;
            }
        }
        return -1;
    }
}
