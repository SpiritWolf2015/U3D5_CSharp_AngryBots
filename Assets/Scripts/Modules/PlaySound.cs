using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip sound;

    void Awake (){
	    if (!audioSource && GetComponent<AudioSource>())
		    audioSource = GetComponent<AudioSource>();
    }

    void OnSignal (){
	    if (sound)
		    audioSource.clip = sound;
	    audioSource.Play ();
    }

}