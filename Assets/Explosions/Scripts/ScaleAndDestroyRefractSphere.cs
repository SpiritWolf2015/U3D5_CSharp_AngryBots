using UnityEngine;
using System.Collections;

public class ScaleAndDestroyRefractSphere : MonoBehaviour {



public float maxScale = 5.0f;
public float scaleSpeed = 2.0f;
public float lifetime = 2.0f;

void Start (){
	Destroy(gameObject, lifetime);	
}

void Update (){
	transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxScale, Time.deltaTime * scaleSpeed);
}
}