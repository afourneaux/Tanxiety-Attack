using UnityEngine;

public class AudioDeleteWhenComplete : MonoBehaviour
{
    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (audioSource.loop == false && audioSource.isPlaying == false) {
            Destroy(gameObject);
        }
    }
}
