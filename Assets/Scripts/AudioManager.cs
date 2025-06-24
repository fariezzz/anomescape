using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource patrolBGMSource;
    public AudioSource chaseBGMSource;

    public bool patrolBGMIsPlaying;
    public bool chaseBGMIsPlaying;

    void Awake() => Instance = this;

    public void TransitionBGM(AudioSource from, AudioSource to, float fadeDuration = 1f)
    {
        if (to.isPlaying && from != to) return;

        StartCoroutine(FadeBGM(from, to, fadeDuration));
    }

    private IEnumerator FadeBGM(AudioSource from, AudioSource to, float duration)
    {
        float t = 0f;
        float startVol = from.volume;

        // Cegah overlap kalau sudah jalan
        if (!to.isPlaying) to.Play();
        to.volume = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            from.volume = Mathf.Lerp(startVol, 0f, t / duration);
            to.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        from.Stop();
        from.volume = startVol;
        to.volume = 1f;
    }
}