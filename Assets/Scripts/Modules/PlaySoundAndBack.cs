using UnityEngine;
using System.Collections;

public class PlaySoundAndBack : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip sound;
    public AudioClip soundReverse;
    public float lengthWithoutTrailing = 0;

    private bool  back = false;
    private float normalizedTime = 0;

    void Awake (){
	    if (!audioSource && GetComponent<AudioSource>())
		    audioSource = GetComponent<AudioSource>();
	    if (lengthWithoutTrailing == 0)
		    lengthWithoutTrailing = Mathf.Min (sound.length, soundReverse.length);
    }

    void OnSignal (){
	    FixTime ();
	
	    PlayWithDirection ();
    }

    void OnPlay (){
	    FixTime ();
	
	    // Set the speed to be positive
	    back = false;
	
	    PlayWithDirection ();
    }

    void OnPlayReverse (){
	    FixTime ();
	
	    // Set the speed to be negative
	    back = true;
	
	    PlayWithDirection ();
    }

    private void PlayWithDirection (){
	
	    float playbackTime;
	
	    if (back) {
		    audioSource.clip = soundReverse;
		    playbackTime = (1 - normalizedTime) * lengthWithoutTrailing;
	    }
	    else {
		    audioSource.clip = sound;
		    playbackTime = normalizedTime * lengthWithoutTrailing;
	    }
	
	    audioSource.time = playbackTime;
	    audioSource.Play ();
	
	    back = !back;
    }

    private void FixTime (){
	    if (audioSource.clip) {
		    normalizedTime = 1.0f;
		    if (audioSource.isPlaying)
			    normalizedTime = Mathf.Clamp01 (audioSource.time / lengthWithoutTrailing);
		    if (audioSource.clip == soundReverse)
			    normalizedTime = 1 - normalizedTime;
	    }
    }

}