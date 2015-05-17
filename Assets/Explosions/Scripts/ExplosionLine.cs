using UnityEngine;
using System.Collections;

public class ExplosionLine : MonoBehaviour {

public int frames = 2;
private int _frames = 0;
void OnEnable (){
	_frames = 0;
}
void Update (){
	_frames++;

	if (_frames>frames)
	{
		gameObject.SetActive (false);
	}
}


}