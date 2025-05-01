using UnityEngine;
using System.Collections;

public class PianoKeyTouch : MonoBehaviour
{
    private Renderer keyRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    private Vector3 originalPosition;
    private float spaceOutAmount = 0.02f; // how far keys move away
    private float spaceOutSpeed = 3f;
    private static float spacingRange = 0.1f; // smaller range for nearby keys

    private float noteStartTime;
    private string noteName;
    private float initialVolume;

    private void Start()
    {
        keyRenderer = GetComponent<Renderer>();
        if (keyRenderer != null)
            originalColor = keyRenderer.material.color;

        originalPosition = transform.localPosition;

        AudioSource keyAudio = GetComponent<AudioSource>();
        if (keyAudio == null)
            keyAudio = gameObject.AddComponent<AudioSource>();

        string keyName = gameObject.name;
        int lastUnderScore = keyName.LastIndexOf("_");
        noteName = keyName.Substring(lastUnderScore + 1);
        keyAudio.clip = Resources.Load<AudioClip>("Notes/" + noteName);
        initialVolume = keyAudio.volume;
    }

    public void OnTriggerEnter(Collider other)
    {
        HighlightKey(true);
        PlaySound();

        var cm = CompositionManager.Instance;
        if (cm != null)
            noteStartTime = cm.GetCurrentTime();

        StartCoroutine(MoveAdjacentKeys(true));

        FindAnyObjectByType<LearnModeManager>()?.OnKeyPressed(noteName);
    }

    public void OnTriggerExit(Collider other)
    {
        HighlightKey(false);

        var cm = CompositionManager.Instance;
        if (cm != null)
        {
            float noteEndTime = cm.GetCurrentTime();
            cm.RecordNote(noteName, noteStartTime, noteEndTime - noteStartTime);
            FindAnyObjectByType<ABCDisplay>()?.UpdateScoreDisplay();
        }

        ReleaseSound();
        StartCoroutine(MoveAdjacentKeys(false));
    }

    private void HighlightKey(bool isHighlighted)
    {
        if (keyRenderer != null)
            keyRenderer.material.color = isHighlighted ? highlightColor : originalColor;
    }

    private void PlaySound()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.volume = initialVolume;
            audio.Play();
        }
    }

    private void ReleaseSound()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
            StartCoroutine(FadeOutCoroutine(audio, 1.1f));
    }

    private IEnumerator FadeOutCoroutine(AudioSource audio, float time)
    {
        float startVolume = audio.volume;

        while (audio.volume > 0)
        {
            audio.volume -= startVolume * Time.deltaTime / time;
            yield return null;
        }

        audio.Stop();
        audio.volume = startVolume;
    }

    private IEnumerator MoveAdjacentKeys(bool isSpacingOut)
    {
        var keys = FindObjectsByType<PianoKeyTouch>(FindObjectsSortMode.None);

        foreach (var key in keys)
        {
            if (key == this)
                continue;

            if (Vector3.Distance(transform.position, key.transform.position) <= spacingRange)
            {
                Vector3 offsetDir = (key.transform.position - transform.position).normalized;
                Vector3 offset = isSpacingOut ? offsetDir * spaceOutAmount : Vector3.zero;
                Vector3 targetPosition = key.originalPosition + offset;

                key.StopAllCoroutines();
                key.StartCoroutine(key.MoveToPosition(targetPosition));
            }
        }

        yield return null;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * spaceOutSpeed);
            yield return null;
        }
        transform.localPosition = targetPosition;
    }

    public Color GetOriginalColor()
    {
        return originalColor;
    }
}

