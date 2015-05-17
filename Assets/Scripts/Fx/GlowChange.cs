using UnityEngine;
using System.Collections;

public class GlowChange : MonoBehaviour {

    public int signalsNeeded = 1;

    void OnSignal (){
	    signalsNeeded--;
	    if (signalsNeeded == 0) {
		    GetComponent<Renderer>().material.SetColor ("_TintColor",new Color (0.29f, 0.64f, 0.15f, 0.5f));
		    enabled = false;
	    }
    }

}