using System.Collections.Generic;
using UnityEngine;

public class CompositionManager : MonoBehaviour
{
    public static CompositionManager Instance { get; private set; }

    private List<NoteEvent> noteEvents = new List<NoteEvent>();
    private float compositionStartTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            compositionStartTime = Time.time;
        }
        else {
            Destroy(gameObject);
        }
    }

    public float GetCurrentTime()
    {
        return Time.time - compositionStartTime;
    }

    public void RecordNote(string noteName, float startTime, float duration)
    {
        noteEvents.Add(new NoteEvent
        {
            NoteName = noteName,
            StartTime = startTime,
            Duration = duration
        });
    }

    public List<NoteEvent> GetNoteEvents()
    {
        return noteEvents;
    }
}

public class NoteEvent
{
    public string NoteName;
    public float StartTime;
    public float Duration;
}


