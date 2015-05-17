using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerOnPresence : MonoBehaviour {

    public SignalSender enterSignals;
    public SignalSender exitSignals;

    public List<GameObject> objects;

    void Awake (){
	    objects = new List<GameObject> ();
	    enabled = false;
    }

    void OnTriggerEnter ( Collider other  ){
	    if (other.isTrigger)
		    return;
	
	    bool  wasEmpty = (objects.Count == 0);
	
	    objects.Add (other.gameObject);
	
	    if (wasEmpty) {
		    enterSignals.SendSignals (this);
		    enabled = true;
	    }
    }

    void OnTriggerExit ( Collider other  ){
	    if (other.isTrigger)
		    return;
	
	    if (objects.Contains (other.gameObject))
		    objects.Remove (other.gameObject);
	
	    if (objects.Count == 0) {
		    exitSignals.SendSignals (this);
		    enabled = false;
	    }
    }

}