using UnityEngine;
using System.Collections;

public class TriggerOnTag : MonoBehaviour {



public string triggerTag = "Player";
public SignalSender enterSignals;
public SignalSender exitSignals;

void OnTriggerEnter ( Collider other  ){
	if (other.isTrigger)
		return;
	
	if (other.gameObject.tag == triggerTag || triggerTag == "") {
		enterSignals.SendSignals (this);
	}
}

void OnTriggerExit ( Collider other  ){
	if (other.isTrigger)
		return;
	
	if (other.gameObject.tag == triggerTag || triggerTag == "") {
		exitSignals.SendSignals (this);
	}
}

}