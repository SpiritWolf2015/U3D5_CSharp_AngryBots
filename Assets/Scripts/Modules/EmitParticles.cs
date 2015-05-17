using UnityEngine;
using System.Collections;

public class EmitParticles : MonoBehaviour {


void OnSignal (){
	GetComponent<ParticleEmitter>().emit = true;
}
}