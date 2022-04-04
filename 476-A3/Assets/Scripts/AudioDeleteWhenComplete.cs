using UnityEngine;

public class AudioDeleteWhenComplete : MonoBehaviour
{
    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Delete this object when its attached audio source completes
        if (audioSource.loop == false && audioSource.isPlaying == false) {
            Destroy(gameObject);
        }
    }
}
