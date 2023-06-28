using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pellet : MonoBehaviour
{
    public AudioClip clip;

    public virtual void OnTriggerEnter(Collider collider)
    {
        // Check for collision with player and destroy this object
        if (collider.CompareTag("Player"))
        {
            PlayPelletSoundInPosition(collider.transform.position);
            Destroy(this.gameObject);
        }
    }

    protected void PlayPelletSoundInPosition(Vector3 pos)
    {
        AudioSource source = PelletManager.Instance.audioSource;

        if (source == null)
            return;

        source.transform.position = pos;
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(clip);
    }
}
