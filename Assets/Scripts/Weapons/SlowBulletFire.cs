using UnityEngine;
using System.Collections;

public class SlowBulletFire : MonoBehaviour {

    public GameObject bulletPrefab;
    public float frequency = 2;                             // 频率
    public float coneAngle = 1.5f;
    public AudioClip fireSound;
    public bool firing = false;

    private float lastFireTime = -1;

    void Start() {
        lastFireTime = -1;
    }

    void Update () {
	    if (firing) {
		    if (Time.time > lastFireTime + 1 / frequency) {
			    Fire ();
		    }
	    }
    }

    void Fire (){
	    // 创建缓存子弹
        Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0);
	    Spawner.Spawn (bulletPrefab, transform.position, transform.rotation * coneRandomRotation);
	
	    if (GetComponent<AudioSource>() && fireSound) {
		    GetComponent<AudioSource>().clip = fireSound;
		    GetComponent<AudioSource>().Play ();
	    }
	
	    lastFireTime = Time.time;
    }

    void OnStartFire () {
	    firing = true;
    }

    void OnStopFire () {
	    firing = false;
    }

}