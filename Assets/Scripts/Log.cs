using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{

    public enum Priority {
        LOW,
        MID,
        HIGH,
        IO,
        ROOM,
        MAP,
        DUNGEON,
        COLLISION,
        SOLO
    }

    public static Priority filter = Priority.MID;

    public static Dictionary<Priority, string> PriorityColors = new Dictionary<Priority, string>() {
        {Priority.LOW, "#DD9900" },
        {Priority.MID, "#99FF00" },
        {Priority.HIGH, "#FF0000" },
        {Priority.IO, "#0099FF" },
        {Priority.SOLO, "#FFFFFF" },
        {Priority.ROOM, "#FFFF00" },
        {Priority.MAP, "#00FFFF" },
        {Priority.COLLISION, "#00FF00" },
        {Priority.DUNGEON, "#FF2222" }

    };

    public static void WriteFile(string fileName, Priority priority = Priority.IO, string debugTag = "[IO]: ") {
        if ((int)priority < (int)filter) { return; }

        string color = PriorityColors[priority];
        string message = string.Format("Writing to File: {0}", fileName);
        print($"<color=" + color + ">" + debugTag + message + "</color>");

    }

    public static void ReadFile(string fileName, Priority priority = Priority.IO, string debugTag = "[IO]: ") {
        if ((int)priority < (int)filter) { return; }

        string color = PriorityColors[priority];
        string message = string.Format( "Reading from File: {0}", fileName);
        print($"<color=" + color + ">" + debugTag + message + "</color>");

    }

    public static void Write(string message, Priority priority = Priority.LOW, string debugTag = "[UNTAGGED]: ") {
        if ((int)priority < (int)filter) { return; }

        string color = PriorityColors[priority];
        print($"<color=" + color + ">" + debugTag + message + "</color>");

    }

    public static void WriteValue(string message, int value, Priority priority = Priority.LOW, string debugTag = "[UNTAGGED]: ") {
        if ((int)priority < (int)filter) { return; }

        string color = PriorityColors[priority];
        print($"<color=" + color + ">" + debugTag + message + value.ToString() + "</color>");

    }

    public static void WriteValue(string message, float value, Priority priority = Priority.LOW, string debugTag = "[UNTAGGED]: ") {
        if ((int)priority < (int)filter) { return; }

        string color = PriorityColors[priority];
        print($"<color=" + color + ">" + debugTag + message + value.ToString() + "</color>");

    }

    public static string ID(int[] id) {
        return string.Format("({0}, {1})", id[0], id[1]);
    }


}
