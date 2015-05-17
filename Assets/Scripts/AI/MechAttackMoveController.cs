using UnityEngine;
using System.Collections;

// 2013年7月30日10:29:47，郭志程

/// <summary>
/// 大机器人攻击移动攻击控制, 绑定在大机器人的子物体AIAttack上的脚本
/// </summary>

public class MechAttackMoveController : MonoBehaviour {

    #region 公共变量
        public MovementMotor motor;
        public Transform head;

        public float targetDistanceMin = 3.0f;                      // 攻击目标的最小距离
        public float targetDistanceMax = 4.0f;                     // 攻击目标的最大距离

        public MonoBehaviour[] weaponBehaviours;          // 武器的行为，用数组是因为大机器人有左右2个发导弹的武器
        public float fireFrequency = 2;                                 // 开火的频率
    #endregion

    #region 私有变量
        private AI ai;
        private Transform character;
        private Transform player;

        private bool inRange = false;
        private float nextRaycastTime = 0;
        private float lastRaycastSuccessfulTime = 0;
        private float noticeTime = 0;

        private bool firing = false;
        private float lastFireTime = -1;
        private int nextWeaponToFire = 0;
    #endregion

    void Awake (){
	    character = motor.transform;
	    player = GameObject.FindWithTag ("Player").transform;
	    ai = transform.parent.GetComponentInChildren<AI> ();
        inRange = firing = false; nextRaycastTime = lastRaycastSuccessfulTime = noticeTime = nextWeaponToFire = 0; lastFireTime = -1;
    }

    void OnEnable () {
	    inRange = false;
	    nextRaycastTime = Time.time + 1;
	    lastRaycastSuccessfulTime = Time.time;
	    noticeTime = Time.time;
    }

    void OnDisable () {
	    Shoot (false);
    }

    public void Shoot(bool state) {
	     firing = state;
    }

    public void Fire () {
	    if (weaponBehaviours[nextWeaponToFire]) {
		    weaponBehaviours[nextWeaponToFire].SendMessage ("Fire");
		    nextWeaponToFire = (nextWeaponToFire + 1) % weaponBehaviours.Length;
		    lastFireTime = Time.time;
	    }
    }

    // 1.设置大机器人朝向玩家。2.在最开始看到玩家时，机器人只是围观并不攻击。
    // 3.判断玩家是否在攻击范围内，如果在攻击范围内并能看见玩家，则开火攻击。4.对开火攻击的频率设置
    void Update () {
	    // 计算从玩家到这个角色间的方向
	    Vector3 playerDirection = (player.position - character.position);
	    playerDirection.y = 0;
	    float playerDist = playerDirection.magnitude;
	    playerDirection /= playerDist;

        // 1.设置这个角色面朝玩家	
	    motor.facingDirection = playerDirection;

        // 2.在很短的时间内看着玩家，只是看着玩家并不向前走或攻击	
	    if (Time.time < noticeTime + 1.5f) {
		    motor.movementDirection = Vector3.zero;
		    return;
	    }
	
	    if (inRange && playerDist > targetDistanceMax)
		    inRange = false;
	    if (!inRange && playerDist < targetDistanceMin)
		    inRange = true;
	
	    if (inRange)
		    motor.movementDirection = Vector3.zero;
	    else
		    motor.movementDirection = playerDirection;
	
	    if (Time.time > nextRaycastTime) {
		    nextRaycastTime = Time.time + 1;
		    if (ai.CanSeePlayer ()) {                                   // 能看见玩家，则表示设置射线追踪成功
			    lastRaycastSuccessfulTime = Time.time;
                if (IsAimingAtPlayer())                               // 3.已经瞄准玩家，则机器人开火攻击玩家 
				    Shoot (true);
			    else
				    Shoot (false);
		    } else {
			    Shoot (false);
			    if (Time.time > lastRaycastSuccessfulTime + 5) {
				    ai.OnLostTrack ();
			    }
		    }
	    }
	
	    if (firing) {
            // 4.开火攻击的频率 
            if (Time.time > lastFireTime + 1 / fireFrequency) {     
			    Fire ();
		    }
	    }
    }

    // 正瞄准玩家中
    public bool IsAimingAtPlayer (){
	     Vector3 playerDirection = (player.position - head.position);
	    playerDirection.y = 0;
	    return Vector3.Angle (head.forward, playerDirection) < 15;
    }

}