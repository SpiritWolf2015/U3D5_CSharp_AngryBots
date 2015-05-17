using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {



string clip = "MyAnimation";

void OnSignal (){
	GetComponent<Animation>().Play(clip);
}
}