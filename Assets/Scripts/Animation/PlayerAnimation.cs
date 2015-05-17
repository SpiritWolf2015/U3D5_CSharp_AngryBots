using UnityEngine;
using System.Collections;

[System.Serializable]
public class MoveAnimation {
	// The animation clip
    public AnimationClip clip;
	
	// The velocity of the walk or run cycle in this clip
    public Vector3 velocity;
	
	// Store the current weight of this animation
	[HideInInspector]
	public float weight;
	
	// Keep track of whether this animation is currently the best match
	[HideInInspector]
	public bool  currentBest = false;
	
	// The speed and angle is directly derived from the velocity,
	// but since it's slightly expensive to calculate them
	// we do it once in the beginning instead of in every frame.
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float angle;
	
	public void Init (){
		velocity.y = 0;
		speed = velocity.magnitude;
		angle = PlayerAnimation.HorizontalAngle (velocity);
	}
}

public class PlayerAnimation : MonoBehaviour {

    public Rigidbody rigid;
    public Transform rootBone;
    public Transform upperBodyBone;
    public float maxIdleSpeed = 0.5f;
    public float minWalkSpeed = 2.0f;
    public AnimationClip idle;
    public AnimationClip turn;
    public AnimationClip shootAdditive;
    public MoveAnimation[] moveAnimations;
    public SignalSender footstepSignals;

    private Transform tr;
    private Vector3 lastPosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 localVelocity = Vector3.zero;
    private float speed = 0;
    private float angle = 0;
    private float lowerBodyDeltaAngle = 0;
    private float idleWeight = 0;
    private Vector3 lowerBodyForwardTarget = Vector3.forward;
    private Vector3 lowerBodyForward = Vector3.forward;
    private MoveAnimation bestAnimation = null;
    private float lastFootstepTime = 0;
    private float lastAnimTime = 0;

    public Animation animationComponent;

    void Awake (){
	    tr = rigid.transform;
	    lastPosition = tr.position;
	
	    foreach(MoveAnimation moveAnimation in moveAnimations) {
		    moveAnimation.Init ();
		    animationComponent[moveAnimation.clip.name].layer = 1;
		    animationComponent[moveAnimation.clip.name].enabled = true;
	    }
	    animationComponent.SyncLayer (1);
	
	    animationComponent[idle.name].layer = 2;
	    animationComponent[turn.name].layer = 3;
	    animationComponent[idle.name].enabled = true;
	
	    animationComponent[shootAdditive.name].layer = 4;
	    animationComponent[shootAdditive.name].weight = 1;
	    animationComponent[shootAdditive.name].speed = 0.6f;
	    animationComponent[shootAdditive.name].blendMode = AnimationBlendMode.Additive;
	
	    //animation[turn.name].enabled = true;
    }

    void OnStartFire (){
	    if (Time.timeScale == 0)
		    return;
	
	    animationComponent[shootAdditive.name].enabled = true;
    }

    void OnStopFire (){
	    animationComponent[shootAdditive.name].enabled = false;
    }

    void FixedUpdate (){
	    velocity = (tr.position - lastPosition) / Time.deltaTime;
	    localVelocity = tr.InverseTransformDirection (velocity);
	    localVelocity.y = 0;
	    speed = localVelocity.magnitude;
	    angle = HorizontalAngle (localVelocity);
	
	    lastPosition = tr.position;
    }

    void Update (){
	    idleWeight = Mathf.Lerp (idleWeight, Mathf.InverseLerp (minWalkSpeed, maxIdleSpeed, speed), Time.deltaTime * 10);
	    animationComponent[idle.name].weight = idleWeight;
	
	    if (speed > 0) {
		    float smallestDiff = Mathf.Infinity;
		    foreach(MoveAnimation moveAnimation in moveAnimations) {
			    float angleDiff = Mathf.Abs(Mathf.DeltaAngle (angle, moveAnimation.angle));
			    float speedDiff = Mathf.Abs (speed - moveAnimation.speed);
			    float diff = angleDiff + speedDiff;
			    if (moveAnimation == bestAnimation)
				    diff *= 0.9f;
			
			    if (diff < smallestDiff) {
				    bestAnimation = moveAnimation;
				    smallestDiff = diff;
			    }
		    }
		
		    animationComponent.CrossFade (bestAnimation.clip.name);
	    }
	    else {
		    bestAnimation = null;
	    }
	
	    if (lowerBodyForward != lowerBodyForwardTarget && idleWeight >= 0.9f)
		    animationComponent.CrossFade (turn.name, 0.05f);
	
	    if (bestAnimation != null && idleWeight < 0.9f) {
		    float newAnimTime= Mathf.Repeat (animationComponent[bestAnimation.clip.name].normalizedTime * 2 + 0.1f, 1);
		    if (newAnimTime < lastAnimTime) {
			    if (Time.time > lastFootstepTime + 0.1f) {
				    footstepSignals.SendSignals (this);
				    lastFootstepTime = Time.time;
			    }
		    }
		    lastAnimTime = newAnimTime;
	    }
    }

    void LateUpdate (){
	    float idle = Mathf.InverseLerp (minWalkSpeed, maxIdleSpeed, speed);
	
	    if (idle < 1) {
		    // Calculate a weighted average of the animation velocities that are currently used
		    Vector3 animatedLocalVelocity = Vector3.zero;
		    foreach(MoveAnimation moveAnimation in moveAnimations) {
			    // Ignore this animation if its weight is 0
			    if (animationComponent[moveAnimation.clip.name].weight == 0)
				    continue;
			
			    // Ignore this animation if its velocity is more than 90 degrees away from current velocity
			    if (Vector3.Dot (moveAnimation.velocity, localVelocity) <= 0)
				    continue;
			
			    // Add velocity of this animation to the weighted average
			    animatedLocalVelocity += moveAnimation.velocity * animationComponent[moveAnimation.clip.name].weight;
		    }
		
		    // Calculate target angle to rotate lower body by in order
		    // to make feet run in the direction of the velocity
		    float lowerBodyDeltaAngleTarget = Mathf.DeltaAngle (
			    HorizontalAngle (tr.rotation * animatedLocalVelocity),
			    HorizontalAngle (velocity)
		    );
		
		    // Lerp the angle to smooth it a bit
		    lowerBodyDeltaAngle = Mathf.LerpAngle (lowerBodyDeltaAngle, lowerBodyDeltaAngleTarget, Time.deltaTime * 10);
		
		    // Update these so they're ready for when we go into idle
		    lowerBodyForwardTarget = tr.forward;
		    lowerBodyForward = Quaternion.Euler (0, lowerBodyDeltaAngle, 0) * lowerBodyForwardTarget;
	    }
	    else {
		    // Turn the lower body towards it's target direction
		    lowerBodyForward = Vector3.RotateTowards (lowerBodyForward, lowerBodyForwardTarget, Time.deltaTime * 520 * Mathf.Deg2Rad, 1);
		
		    // Calculate delta angle to make the lower body stay in place
		    lowerBodyDeltaAngle = Mathf.DeltaAngle (
			    HorizontalAngle (tr.forward),
			    HorizontalAngle (lowerBodyForward)
		    );
		
		    // If the body is twisted more than 80 degrees,
		    // set a new target direction for the lower body, so it begins turning
		    if (Mathf.Abs(lowerBodyDeltaAngle) > 80)
			    lowerBodyForwardTarget = tr.forward;
	    }
	
	    // Create a Quaternion rotation from the rotation angle
	    Quaternion lowerBodyDeltaRotation = Quaternion.Euler (0, lowerBodyDeltaAngle, 0);
	
	    // Rotate the whole body by the angle
	    rootBone.rotation = lowerBodyDeltaRotation * rootBone.rotation;
	
	    // Counter-rotate the upper body so it won't be affected
	    upperBodyBone.rotation = Quaternion.Inverse (lowerBodyDeltaRotation) * upperBodyBone.rotation;
	
    }

    public static float HorizontalAngle ( Vector3 direction  ){
	    return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
    }

}