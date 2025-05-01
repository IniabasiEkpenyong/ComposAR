using System.Collections.Generic;
using UnityEngine;

public class AutoArrangePianoKeys : MonoBehaviour
{
    public GameObject whiteKeyPrefab;
    public GameObject blackKeyPrefab;

    public float keyWidth = 0.1f;
    public float keyHeight = 0.5f;
    public float keySpacing = 0.01f;
    public int numberOfOctaves = 2;

    private Transform keysParent;
    public List<GameObject> allKeys = new List<GameObject>();
    void Start()
    {
        if (keysParent == null) { }
        keysParent = new GameObject("KeysParent").transform;
        keysParent.SetParent(transform);
        keysParent.localPosition = Vector3.zero;
        keysParent.localRotation = Quaternion.identity;
        keysParent.localScale = Vector3.one;
        CreatePianoKeys();
    }

    public void UpdateNumberOfOctaves(float newOctaveCount)
    {
        numberOfOctaves = Mathf.RoundToInt(newOctaveCount);
        RegeneratePianoKeys();
    }

    private void RegeneratePianoKeys()
    {
        foreach (Transform child in keysParent)
        {
            Destroy(child.gameObject);
        }
        allKeys.Clear();

        CreatePianoKeys();
    }

    private void CreatePianoKeys()
    {
        float totalWhiteKeys = numberOfOctaves * 7;
        float totalWidth = totalWhiteKeys * keyWidth + (totalWhiteKeys - 1) * keySpacing;
        float startX = -totalWidth / 2f;
        float currentXPosition = startX;
        string[] whiteKeyNames = { "C", "D", "E", "F", "G", "A", "B" };
        int startingOctave = 4 - (numberOfOctaves - 1) / 2;

        for (int octave = 0; octave < numberOfOctaves; octave++)
        {
            float octaveStartX = currentXPosition;
            int currentOctave = startingOctave + octave;

            for (int i = 0; i < 7; i++)
            {
                string keyName = whiteKeyNames[i] + currentOctave;
                CreateWhiteKey(currentXPosition, keyName);
                currentXPosition += keyWidth + keySpacing;
            }

            float spacing = keyWidth + keySpacing;

            CreateBlackKey(octaveStartX + spacing * 0.5f, "C#" + currentOctave); // C#
            CreateBlackKey(octaveStartX + spacing * 1.5f, "D#" + currentOctave); // D#
            CreateBlackKey(octaveStartX + spacing * 3.5f, "F#" + currentOctave); // F#
            CreateBlackKey(octaveStartX + spacing * 4.5f, "G#" + currentOctave); // G#
            CreateBlackKey(octaveStartX + spacing * 5.5f, "A#" + currentOctave); // A#
        }
    }

    private void CreateWhiteKey(float xPosition, string keyIndex)
    {
        GameObject key = Instantiate(whiteKeyPrefab, keysParent);
        key.transform.localPosition = new Vector3(xPosition, 0, 0);
        key.transform.localScale = new Vector3(keyWidth, keyHeight, keyWidth);
        key.name = "WhiteKey_" + keyIndex;
        allKeys.Add(key);
    }

    private void CreateBlackKey(float xPosition, string keyIndex)
    {
        GameObject key = Instantiate(blackKeyPrefab, keysParent);
        key.transform.localPosition = new Vector3(xPosition, 0.125f, -0.125f); // Slightly raised and pushed back
        key.transform.localScale = new Vector3(keyWidth * 0.5f, keyHeight * 0.6f, keyWidth);
        key.name = "BlackKey_" + keyIndex;
        allKeys.Add(key);
    }
}

