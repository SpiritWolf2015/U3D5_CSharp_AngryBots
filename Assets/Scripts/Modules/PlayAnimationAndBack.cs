using UnityEngine;
using System.Collections;

public class PlayAnimationAndBack : MonoBehaviour {



string clip = "MyAnimation";
float speed = 1.0f;

void OnSignal (){
	FixTime ();
	
	PlayWithSpeed ();
}

void OnPlay (){
	FixTime ();
	
	// Set the speed to be positive
	speed = Mathf.Abs (speed);
	
	PlayWithSpeed ();
}

void OnPlayReverse (){
	FixTime ();
	
	// Set the speed to be negative
	speed = Mathf.Abs (speed) * -1;
	
	PlayWithSpeed ();
}

private void PlayWithSpeed (){	
	// Play the animation with the desired speed.
	GetComponent<Animation>()[clip].speed = speed;
	GetComponent<Animation>()[clip].weight = 1;
	GetComponent<Animation>()[clip].enabled = true;
	
	// Reverse the speed so it's ready for playing the other way next time.
	speed = -speed;
}

private void FixTime (){
	// If the animation played to the end last time, it got automatically rewinded.
	// We don't want that here, so set the time back to 1.
	if (speed < 0 && GetComponent<Animation>()[clip].time == 0)
		GetComponent<Animation>()[clip].normalizedTime = 1;
	
	// In other cases, just clamp the time so it doesn't exceed the bounds of the animation.
	else
		GetComponent<Animation>()[clip].normalizedTime = Mathf.Clamp01 (GetComponent<Animation>()[clip].normalizedTime);
}

}