using UnityEngine;
using System.Collections;

public class SlowBulletFireWithNoise : MonoBehaviour {

    public GameObject bulletPrefab;
    public float frequency = 2;
    public float coneAngle = 1.5f;
    public AudioClip fireSound;
    public bool firing = false;
    public float noisiness = 2.0f;

    private float nextFireNoise = 1.0f;
    private float lastFireTime = -1;

    void Update () {
	    if (firing) {
		    if (Time.time > nextFireNoise + lastFireTime + 1 / frequency) {
			    Fire ();
		    }
	    }
    }

    void Fire (){
	    // Spawn visual bullet
        Quaternion coneRandomRotation = Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0);
	    Spawner.Spawn (bulletPrefab, transform.position, transform.rotation * coneRandomRotation);
	
	    if (GetComponent<AudioSource>() && fireSound) {
		    GetComponent<AudioSource>().clip = fireSound;
		    GetComponent<AudioSource>().Play ();
	    }
	
	    lastFireTime = Time.time;
	    nextFireNoise = Random.value * noisiness;
    }

    void OnStartFire (){
	    firing = true;
    }

    void OnStopFire (){
	    firing = false;
    }

}