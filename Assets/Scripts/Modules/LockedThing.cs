using UnityEngine;
using System.Collections;

public class LockedThing : MonoBehaviour {


// This component will forward a signal only if all the locks are unlocked

Lock[] locks;
SignalSender conditionalSignal;

void OnSignal (){
	bool  locked = false;
	foreach(Lock lockObj in locks) {
		if (lockObj.locked)
			locked = true;
	}
	
	if (locked == false)
		conditionalSignal.SendSignals (this);
}

}