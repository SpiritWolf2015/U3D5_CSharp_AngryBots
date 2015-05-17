using UnityEngine;
using System.Collections;

public class MuzzleFlashAnimate : MonoBehaviour {

    void Update (){
	    transform.localScale = Vector3.one * Random.Range(0.5f,1.5f);	    
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(0, 90.0f));
    }

}