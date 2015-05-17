using UnityEngine;
using System.Collections;

public class FanRotate : MonoBehaviour {

    public Mesh thisMesh;
    public Vector2[] uvs;

    #if !UNITY_IPHONE || !UNITY_ANDROID

    void Start (){
        thisMesh = GetComponent<MeshFilter>().mesh;
        uvs = thisMesh.uv;
    }

    void Update (){
	    for (int i = 0; i < uvs.Length; i++) 
	    {
		    uvs[i].y = (uvs[i].y + 0.25f);
	    }
	
	    thisMesh.uv = uvs;
    }

    #endif

}