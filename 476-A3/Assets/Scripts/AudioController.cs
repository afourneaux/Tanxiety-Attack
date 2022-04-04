using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioClip> AudioClips;
    public GameObject AudioPrefab;
    public static AudioController instance;
    string musicID;

    static int counter = 1;

    void OnEnable() {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public string PlayMusic(int index) {
        if (musicID != null) {
            StopByID(musicID);
        }
        musicID = PlaySound(index, true);
        return musicID;
    }

    public void StopByID(string soundID) {
        if (string.IsNullOrEmpty(soundID)) {
            return;
        }
        Transform audioTransform = transform.Find(soundID);
        if (audioTransform != null) {
            Destroy(audioTransform.gameObject);
        }
    }

    public string PlaySound(int index, bool looping = false, float volume = 1f) {
        GameObject go = Instantiate(AudioPrefab, Vector3.zero, Quaternion.identity, transform);
        go.name = go.name + counter.ToString();
        counter++;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.clip = AudioClips[index];
        audio.loop = looping;
        audio.volume = volume;
        audio.Play();
        return go.name;
    }
}
