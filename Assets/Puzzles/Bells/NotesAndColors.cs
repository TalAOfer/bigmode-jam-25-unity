using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Notes And Colors")]
public class NotesAndColors : ScriptableObject
{
    public List<Note> value;
}

[System.Serializable]
public class Note
{
    public string letter;
    public Color color;
}
