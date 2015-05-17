using UnityEngine;
using System.Collections;

// 2013年7月28日0:22:33，郭志程

public class SpiderAnimation : MonoBehaviour {

    public MovementMotor motor;
    public AnimationClip activateAnim;
    public AnimationClip forwardAnim;
    public AnimationClip backAnim;
    public AnimationClip leftAnim;
    public AnimationClip rightAnim;
    public AudioSource audioSource;
    public SignalSender footstepSignals;
    public bool skiddingSounds = true;
    public bool footstepSounds = true;

    private Transform tr;
    private float lastFootstepTime = 0;
    private float lastAnimTime = 0;

    void Awake() {
        lastFootstepTime = lastAnimTime = 0;
        tr = motor.transform;
    }

    void OnEnable (){
	    tr = motor.transform;
	
	    GetComponent<Animation>()[activateAnim.name].enabled = true;
	    GetComponent<Animation>()[activateAnim.name].weight = 1;
	    GetComponent<Animation>()[activateAnim.name].time = 0;
	    GetComponent<Animation>()[activateAnim.name].speed = 1;
	
	    GetComponent<Animation>()[forwardAnim.name].layer = 1;
	    GetComponent<Animation>()[forwardAnim.name].enabled = true;
	    GetComponent<Animation>()[forwardAnim.name].weight = 0;
	    GetComponent<Animation>()[backAnim.name].layer = 1;
	    GetComponent<Animation>()[backAnim.name].enabled = true;
	    GetComponent<Animation>()[backAnim.name].weight = 0;
	    GetComponent<Animation>()[leftAnim.name].layer = 1;
	    GetComponent<Animation>()[leftAnim.name].enabled = true;
	    GetComponent<Animation>()[leftAnim.name].weight = 0;
	    GetComponent<Animation>()[rightAnim.name].layer = 1;
	    GetComponent<Animation>()[rightAnim.name].enabled = true;
	    GetComponent<Animation>()[rightAnim.name].weight = 0;
	
    }

    void OnDisable (){
	    GetComponent<Animation>()[activateAnim.name].enabled = true;
	    GetComponent<Animation>()[activateAnim.name].weight = 1;
	    GetComponent<Animation>()[activateAnim.name].normalizedTime = 1;
	    GetComponent<Animation>()[activateAnim.name].speed = -1;
	    GetComponent<Animation>().CrossFade (activateAnim.name, 0.3f, PlayMode.StopAll);
    }

    void Update (){
	    Vector3 direction = motor.movementDirection;        // 得到怪的移动方向
	    direction.y = 0;

        // walkWeight 走路权重值，这里把方向向量的长度值赋给walkWeight，在之后的if判断中，如果walkWeight大于0.01表示要往前走
	    float walkWeight = direction.magnitude;

        GetComponent<Animation>()[forwardAnim.name].speed = walkWeight;     // 把walkWeight赋值给动画的速度，表示走的距离越长，怪行走的速度也就越快，所以对应的行走动画效果也非常快
	    GetComponent<Animation>()[rightAnim.name].speed = walkWeight;
	    GetComponent<Animation>()[backAnim.name].speed = walkWeight;
	    GetComponent<Animation>()[leftAnim.name].speed = walkWeight;

        // 得到横向转动的角度。Mathf.DeltaAngle : u3d api, 计算给定的两个角之间最短的差异。
	    float angle = Mathf.DeltaAngle (
		    HorizontalAngle (tr.forward),
		    HorizontalAngle (direction)
	    );
	
	    if (walkWeight > 0.01f) {
		    float w;
		    if (angle < -90) {
                w = Mathf.InverseLerp(-180, -90, angle);   // Mathf.InverseLerp : U3D API, 计算两个值之间的Lerp参数。也就是value在from和to之间的比例值。返回的float值位于0到1之间
			    GetComponent<Animation>()[forwardAnim.name].weight = 0;    // 根据怪的移动方向角度值设置怪动画的权重值，以播放往哪个方向走的动画，比如小于-90就是往左走
			    GetComponent<Animation>()[rightAnim.name].weight = 0;
			    GetComponent<Animation>()[backAnim.name].weight = 1 - w;
			    GetComponent<Animation>()[leftAnim.name].weight = 1;
		    }
		    else if (angle < 0) {
			    w = Mathf.InverseLerp (-90, 0, angle);
			    GetComponent<Animation>()[forwardAnim.name].weight = w;
			    GetComponent<Animation>()[rightAnim.name].weight = 0;
			    GetComponent<Animation>()[backAnim.name].weight = 0;
			    GetComponent<Animation>()[leftAnim.name].weight = 1 - w;
		    }
		    else if (angle < 90) {
			    w = Mathf.InverseLerp (0, 90, angle);
			    GetComponent<Animation>()[forwardAnim.name].weight = 1 - w;
			    GetComponent<Animation>()[rightAnim.name].weight = w;
			    GetComponent<Animation>()[backAnim.name].weight = 0;
			    GetComponent<Animation>()[leftAnim.name].weight = 0;
		    }
		    else {
			    w = Mathf.InverseLerp (90, 180, angle);
			    GetComponent<Animation>()[forwardAnim.name].weight = 0;
			    GetComponent<Animation>()[rightAnim.name].weight = 1 - w;
			    GetComponent<Animation>()[backAnim.name].weight = w;
			    GetComponent<Animation>()[leftAnim.name].weight = 0;
		    }
	    }
	
        // 播放侧滑音效
	    if (skiddingSounds) {
		    if (walkWeight > 0.2f && !audioSource.isPlaying)
			    audioSource.Play ();
		    else if (walkWeight < 0.2f && audioSource.isPlaying)
			    audioSource.Pause ();
	    }
	
	    if (footstepSounds && walkWeight > 0.2f) {
            float newAnimTime = Mathf.Repeat(GetComponent<Animation>()[forwardAnim.name].normalizedTime * 4 + 0.1f, 1);     // Mathf.Repeat : u3d api, 循环数值t，0到length之间。t值永远不会大于length的值，也永远不会小于0。

		    if (newAnimTime < lastAnimTime) {
			    if (Time.time > lastFootstepTime + 0.1f) {
                    footstepSignals.SendSignals(this);     // 这个挂的怪根物体，所以表示往怪根物体发消息，发的消息执行的行动是OnFootstep，所以也就是怪根物体上的FootstepHandler脚本会来响应，执行OnFootstep函数，播放类型为Spider（蜘蛛）类型的走路声音
				    lastFootstepTime = Time.time;
			    }
		    }
		    lastAnimTime = newAnimTime;
	    }
    }

    static float HorizontalAngle ( Vector3 direction  ){
	    return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
    }

}