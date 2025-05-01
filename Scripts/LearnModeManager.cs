using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;

public class LearnModeManager : MonoBehaviour
{
    [Header("References")]
    public AutoArrangePianoKeys pianoKeyArranger;
    public TTSSpeaker ttsSpeaker;

    [Header("Timing Settings")]
    public float flashStartTime = 3f;
    public float responseTime = 10f;

    [Header("UI Elements")]
    public TMP_Text timerText;
    public GameObject correctPanel;
    public GameObject wrongPanel;
    public GameObject instructionPanel;
    public TMP_Text instructionText;

    private GameObject currentTargetKey;
    private Coroutine flashingCoroutine;
    private bool isAwaitingResponse = false;
    private bool gameActive = true;

    void Start()
    {
        HideAllPanels();
        StartCoroutine(StartLearnMode());
    }

    IEnumerator StartLearnMode()
    {
        yield return new WaitForSeconds(2f); // small startup delay

        while (gameActive)
        {
            yield return PromptNextNote();
        }
    }

    IEnumerator PromptNextNote()
    {
        ResetAllKeys();
        HideAllPanels();

        currentTargetKey = pianoKeyArranger.allKeys[Random.Range(0, pianoKeyArranger.allKeys.Count)];
        string note = currentTargetKey.name.Split('_')[1];

        // Display instruction
        instructionPanel.SetActive(true);

        if (Random.value < 0.5f)
        {
            instructionText.text = $"Please play the note: <b>{note}</b>";
            ttsSpeaker.Speak("Please play the note " + note);
        }
        else
        {
            instructionText.text = "Play the note you hear";
            ttsSpeaker.Speak("Play the note you hear");
            currentTargetKey.GetComponent<AudioSource>().Play();
        }

        isAwaitingResponse = true;
        flashingCoroutine = StartCoroutine(ResponseCountdown(note));

        while (isAwaitingResponse)
            yield return null;

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ResponseCountdown(string expectedNote)
    {
        float elapsed = 0f;
        bool flashStarted = false;

        while (elapsed < responseTime)
        {
            timerText.text = $"Time: {Mathf.Ceil(responseTime - elapsed)}";

            if (!flashStarted && elapsed >= flashStartTime)
            {
                StartCoroutine(FlashKey(currentTargetKey, Color.yellow));
                flashStarted = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isAwaitingResponse)
        {
            isAwaitingResponse = false;
            StartCoroutine(FlashAllKeys(Color.red));
            ShowWrongPanel();
            yield return new WaitForSeconds(2f);
        }
    }

    public void OnKeyPressed(string notePressed)
    {
        if (!isAwaitingResponse || !gameActive) return;

        string expected = currentTargetKey.name.Split('_')[1];

        isAwaitingResponse = false;
        if (notePressed == expected)
        {
            if (flashingCoroutine != null) StopCoroutine(flashingCoroutine);
            StartCoroutine(FlashKey(currentTargetKey, Color.green));
            ShowCorrectPanel();
        }
        else
        {
            if (flashingCoroutine != null) StopCoroutine(flashingCoroutine);
            StartCoroutine(FlashAllKeys(Color.red));
            ShowWrongPanel();
        }
    }

    private IEnumerator FlashKey(GameObject key, Color flashColor)
    {
        Renderer renderer = key.GetComponent<Renderer>();
        if (renderer == null) yield break;

        Color original = renderer.material.color;
        for (int i = 0; i < 4; i++)
        {
            renderer.material.color = flashColor;
            yield return new WaitForSeconds(0.25f);
            renderer.material.color = original;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator FlashAllKeys(Color flashColor)
    {
        List<Renderer> renderers = new List<Renderer>();
        List<Color> originals = new List<Color>();

        foreach (GameObject key in pianoKeyArranger.allKeys)
        {
            Renderer renderer = key.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderers.Add(renderer);
                originals.Add(renderer.material.color);
                renderer.material.color = flashColor;
            }
        }

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material.color = originals[i];
        }
    }

    void ResetAllKeys()
    {
        foreach (GameObject key in pianoKeyArranger.allKeys)
        {
            Renderer renderer = key.GetComponent<Renderer>();
            PianoKeyTouch pTouch = key.GetComponent<PianoKeyTouch>();
            if (renderer != null && pTouch != null)
                renderer.material.color = pTouch.GetOriginalColor();
        }
    }

    void HideAllPanels()
    {
        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
        instructionPanel.SetActive(false);
    }

    void ShowCorrectPanel()
    {
        correctPanel.SetActive(true);
        wrongPanel.SetActive(false);
        instructionPanel.SetActive(false);
    }

    void ShowWrongPanel()
    {
        wrongPanel.SetActive(true);
        correctPanel.SetActive(false);
        instructionPanel.SetActive(false);
    }
}
