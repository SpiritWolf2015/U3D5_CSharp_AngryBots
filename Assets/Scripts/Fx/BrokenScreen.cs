using UnityEngine;
using System.Collections;

public class BrokenScreen : MonoBehaviour {

public static Material brokenMaterial = null;

    void Start () {
	    if (brokenMaterial == null)
		    brokenMaterial = new Material (GetComponent<Renderer>().sharedMaterial);
		
	    GetComponent<Renderer>().material = brokenMaterial;
    }

    void OnWillRenderObject () {	    
        brokenMaterial.mainTextureOffset = new Vector2(brokenMaterial.mainTextureOffset.x + Random.Range(1.0f, 2.0f), brokenMaterial.mainTextureOffset.y);
    }   

}