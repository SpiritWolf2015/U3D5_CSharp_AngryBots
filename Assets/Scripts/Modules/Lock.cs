using UnityEngine;
using System.Collections;

public class Lock : MonoBehaviour {

    public bool locked = true;
    public SignalSender unlockedSignal;

    void OnSignal (){
	    if (locked) {
		    locked = false;
		    unlockedSignal.SendSignals (this);
	    }
    }

}