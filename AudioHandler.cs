using System.Collections;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public float musicVolumeValue = 0.1f;
    [SerializeField]
    private AudioClip[] audioClips;

    private AudioClip currentAudioClip;

    [SerializeField]
    private float fadeDuration = 1f;

    public static AudioHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }      
    }
    void Start()
    {
        if (audioClips.Length == 0 || audioSource == null)
        {
            Debug.LogError("AudioClips array or AudioSource is not assigned.");
            return;
        }

        AssignRandomClip();
        audioSource.volume = 0f;
        StartCoroutine(PlayAudioWithFades());
    }
    public void StopMusic()
    {
        StopAllCoroutines();
        audioSource.Pause();
    }

    public void StartMusic()
    {
        audioSource.volume = 0f;
        StartCoroutine(PlayAudioWithFades());
    }
    private void AssignRandomClip()
    {
        currentAudioClip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.clip = currentAudioClip;
    }

    private IEnumerator PlayAudioWithFades()
    {
        while (true)
        {
            audioSource.Play();
            yield return StartCoroutine(LerpVolume(musicVolumeValue, fadeDuration));
            yield return new WaitForSeconds(currentAudioClip.length - fadeDuration);
            yield return StartCoroutine(LerpVolume(0f, fadeDuration));
            AssignRandomClip();
        }
    }

    private IEnumerator LerpVolume(float targetVolume, float duration)
    {
        float initialVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float percentageComplete = elapsedTime / duration;
            audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, percentageComplete);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
        if (targetVolume == 0f)
        {
            StartCoroutine(PlayAudioWithFades());
        }
    }
}
