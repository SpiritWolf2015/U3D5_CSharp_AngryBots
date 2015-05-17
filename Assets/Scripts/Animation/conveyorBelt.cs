using UnityEngine;
using System.Collections;

public class conveyorBelt : MonoBehaviour {



float scrollSpeed = 0.1f;
Material mat;

void Start (){
	enabled = false;
}

void OnBecameVisible (){
	enabled = true;	
}

void OnBecameInvisible (){
	enabled = false;	
}

void Update (){
	float offset = (Time.time * scrollSpeed) % 1.0f;
	
	mat.SetTextureOffset ("_MainTex",new Vector2(0, -offset));
	mat.SetTextureOffset ("_BumpMap",new Vector2(0, -offset));
}

}