using UnityEngine;
using System.Collections;

public class ExplosionTrail : MonoBehaviour {

private Vector3 _dir;

void OnEnable (){
	_dir = Random.onUnitSphere;
	_dir.y = 1.25f;
}

void Update (){
	transform.position += _dir * Time.deltaTime * 5.5f;
	
	_dir.y -= Time.deltaTime;
	
	if(_dir.y<0.0f && transform.position.y <= -1.0f) {
		this.enabled = false;	
	}
}


}