using UnityEngine;
using System.Collections;

//2013年7月27日17:44:09，郭志程

public class SelfIlluminationBlink : MonoBehaviour {

    public float blink = 0.0f;

    void OnWillRenderObject (){
	    GetComponent<Renderer>().sharedMaterial.SetFloat ("_SelfIllumStrength", blink);	
    }

    public void Blink() {
	    blink = 1.0f - blink;
    }

}