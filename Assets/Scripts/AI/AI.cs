using UnityEngine;
using System.Collections;

//2013年7月27日17:07:23，郭志程

public class AI : MonoBehaviour {

    // Public member data
    public MonoBehaviour behaviourOnSpotted;    // 发现玩家时的行为,这里绑定的是AIAttack脚本，也就是发现玩家后攻击玩家
    public AudioClip soundOnSpotted;           // 发现玩家时的声音
    public MonoBehaviour behaviourOnLostTrack;  // 失去对玩家追踪时的行为,这里绑定的是AIReturn脚本，也就是找不到玩家后回到怪原来的位置

    // Private memeber data
    private Transform character;              // 怪自己Transform的变量
    private Transform player;                 // 玩家Transform的变量
    private bool insideInterestArea = true;    // 玩家在AI攻击区域

    void Awake (){
	    character = transform;
	    player = GameObject.FindWithTag ("Player").transform;
        insideInterestArea = true;
    }

    //当对象变为可用或激活状态时此函数被调用。OnEnable不能用于协同程序。
    void OnEnable (){
	    behaviourOnLostTrack.enabled = true;
	    behaviourOnSpotted.enabled = false;
    }

    // 当玩家进入出发区域，调用发现玩家函数
    void OnTriggerEnter ( Collider other  ){
	    if (other.transform == player && CanSeePlayer ()) {
		    OnSpotted ();
	    }
    }

    // 不是U3D API函数。
    public void OnEnterInterestArea (){
	    insideInterestArea = true;
    }
    // 不是U3D API函数。
    public void OnExitInterestArea() {
	    insideInterestArea = false;
	    OnLostTrack ();
    }

    public void OnSpotted() {
	    if (!insideInterestArea)
		    return;
	    if (!behaviourOnSpotted.enabled) {
		    behaviourOnSpotted.enabled = true;      // 对玩家攻击
		    behaviourOnLostTrack.enabled = false;   // 关闭怪跑回原位的脚本
		
		    if (GetComponent<AudioSource>() && soundOnSpotted) {
			    GetComponent<AudioSource>().clip = soundOnSpotted;
			    GetComponent<AudioSource>().Play ();
		    }
	    }
    }

    /// <summary>
    /// 找不到玩家时的行为，关闭攻击的脚本，启用怪跑回原位的脚本
    /// </summary>
    
    public void OnLostTrack() {
	    if (!behaviourOnLostTrack.enabled) {
		    behaviourOnLostTrack.enabled = true;    // 启用跑回原位脚本
		    behaviourOnSpotted.enabled = false;     // 停止攻击脚本
	    }
    }
    
    /// <summary>
    /// 从怪的位置往玩家的位置发条射线，射线检测的碰撞物是玩家，则表示能看见玩家，否则是看不到玩家
    /// </summary>
    
    public bool CanSeePlayer (){
	    Vector3 playerDirection = (player.position - character.position);
	    RaycastHit hit;
	    Physics.Raycast (character.position, playerDirection, out hit, playerDirection.magnitude);
	    if (hit.collider && hit.collider.transform == player) {
		    return true;
	    }
	    return false;
    }

}