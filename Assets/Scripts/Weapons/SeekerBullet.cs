using UnityEngine;
using System.Collections;


/// <summary>
/// 搜查人子弹，大机器人打的导弹脚本
/// </summary>
public class SeekerBullet : MonoBehaviour {

    public float speed = 15.0f;
    public float lifeTime = 1.5f;
    public float damageAmount = 5;      // 损毁数量
    public float forceAmount = 5;           // 力的数量
    public float radius = 1.0f;                  // 检测碰撞的半径
    public float seekPrecision = 1.3f;      // 搜索精度
    public LayerMask ignoreLayers;
    public float noise = 0.0f;
    public GameObject explosionPrefab;

    private Vector3 dir;
    private float spawnTime;
    private GameObject targetObject;
    private Transform tr;
    private float sideBias;                         // 偏移

    void Awake() {
        dir = Vector3.zero; spawnTime = sideBias = 0f; targetObject = null; tr = null;
    }

    void OnEnable () {
	    tr = transform;
	    dir = transform.forward;
	    targetObject = GameObject.FindWithTag ("Player");
	    spawnTime = Time.time;
	    sideBias = Mathf.Sin (Time.time * 5);
    }

    void Update () {	
        // 超过子弹的生命时间就将子弹销毁
	    if (Time.time > spawnTime + lifeTime) {
		    Spawner.Destroy (gameObject);
	    }
	
	    if (targetObject) {
		    Vector3 targetPos = targetObject.transform.position;        // 玩家所在的位置
		    targetPos += transform.right * (Mathf.PingPong (Time.time, 1.0f) - 0.5f) * noise;
		    Vector3 targetDir = (targetPos - tr.position);		float targetDist = targetDir.magnitude;     // 玩家的方向，距离
		    targetDir /= targetDist;
		    if (Time.time - spawnTime < lifeTime * 0.2f && targetDist > 3)
			    targetDir += transform.right * 0.5f * sideBias;
		
		    dir = Vector3.Slerp (dir, targetDir, Time.deltaTime * seekPrecision);
	
		    tr.rotation = Quaternion.LookRotation(dir); 	
		    tr.position += (dir * speed) * Time.deltaTime;
	    }
	
	    // 检查这个子弹碰撞到某个物体
        Collider[] hits = Physics.OverlapSphere(tr.position, radius, ~ignoreLayers.value);         // 返回球型半径之内（包括半径）的所有碰撞体,注意：一般来说，这个函数检查的是碰撞体的包围体，而不是碰撞体的实际边界。

	    bool  collided = false;
	    foreach(Collider c in hits) {		    
            // 不对触发器进行碰撞, Collider.isTrigger : U3D API, 碰撞器是一个触发器？
		    if (c.isTrigger)
			    continue;
		
		    Health targetHealth = c.GetComponent<Health> ();
		    if (targetHealth) {
			    // 应用毁坏
			    targetHealth.OnDamage (damageAmount, -tr.forward);
		    }
		    // 如果有的话，得到刚体
		    if (c.GetComponent<Rigidbody>()) {
			    // 对目标物体施加力
			    Vector3 force = tr.forward * forceAmount;
			    force.y = 0;
                c.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);    // ForceMode.Impulse : U3D API, 使用它的质量, 添加一个瞬间冲击力到刚体。
		    }
		    collided = true;
	    }
	    if (collided) {
		    Spawner.Destroy (gameObject);                                                                           // 销毁导弹
		    Spawner.Spawn (explosionPrefab, transform.position, transform.rotation);        // 创建爆炸
	    }
    }

}