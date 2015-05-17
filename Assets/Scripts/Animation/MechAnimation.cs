using UnityEngine;
using System.Collections;

public class MechAnimation : MonoBehaviour {

    public Rigidbody rigid;
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip turnLeft;
    public AnimationClip turnRight;
    public SignalSender footstepSignals;

    private Transform tr;
    private float lastFootstepTime = 0;
    private float lastAnimTime = 0;

    void OnEnable (){
	    tr = rigid.transform;
	
	    GetComponent<Animation>()[idle.name].layer = 0;
	    GetComponent<Animation>()[idle.name].weight = 1;
	    GetComponent<Animation>()[idle.name].enabled = true;
	
	    GetComponent<Animation>()[walk.name].layer = 1;
	    GetComponent<Animation>()[turnLeft.name].layer = 1;
	    GetComponent<Animation>()[turnRight.name].layer = 1;
	
	    GetComponent<Animation>()[walk.name].weight = 1;
	    GetComponent<Animation>()[turnLeft.name].weight = 0;
	    GetComponent<Animation>()[turnRight.name].weight = 0;
	
	    GetComponent<Animation>()[walk.name].enabled = true;
	    GetComponent<Animation>()[turnLeft.name].enabled = true;
	    GetComponent<Animation>()[turnRight.name].enabled = true;
	
	    //animation.SyncLayer (1);
    }

    void FixedUpdate (){
	    float turningWeight = Mathf.Abs (rigid.angularVelocity.y) * Mathf.Rad2Deg / 100.0f;
	    float forwardWeight = rigid.velocity.magnitude / 2.5f;
	    float turningDir = Mathf.Sign (rigid.angularVelocity.y);
	
	    // Temp, until we get the animations fixed
	    GetComponent<Animation>()[walk.name].speed = Mathf.Lerp (1.0f, GetComponent<Animation>()[walk.name].length / GetComponent<Animation>()[turnLeft.name].length * 1.33f, turningWeight);
	    GetComponent<Animation>()[turnLeft.name].time = GetComponent<Animation>()[walk.name].time;
	    GetComponent<Animation>()[turnRight.name].time = GetComponent<Animation>()[walk.name].time;
	
	    GetComponent<Animation>()[turnLeft.name].weight = Mathf.Clamp01 (-turningWeight * turningDir);
	    GetComponent<Animation>()[turnRight.name].weight = Mathf.Clamp01 (turningWeight * turningDir);
	    GetComponent<Animation>()[walk.name].weight = Mathf.Clamp01 (forwardWeight);
	
	    if (forwardWeight + turningWeight > 0.1f) {
		    float newAnimTime= Mathf.Repeat (GetComponent<Animation>()[walk.name].normalizedTime * 2 + 0.1f, 1);
		    if (newAnimTime < lastAnimTime) {
			    if (Time.time > lastFootstepTime + 0.1f) {
				    footstepSignals.SendSignals (this);
				    lastFootstepTime = Time.time;
			    }
		    }
		    lastAnimTime = newAnimTime;
	    }
    }

}