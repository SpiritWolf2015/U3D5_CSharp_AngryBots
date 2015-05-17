using UnityEngine;
using System.Collections;

 [System.Serializable]
public class ExplosionPart {
    public GameObject gameObject = null;
    public float delay = 0.0f;
    public bool hqOnly = false;
    public float yOffset = 0.0f;
}

/// <summary>
/// 特效，比如大机器被打爆后的爆炸，以及大机器打出的导弹的爆炸效果等
/// </summary>
 [System.Serializable]
public class EffectSequencer : MonoBehaviour {

    public ExplosionPart[] ambientEmitters;
    public ExplosionPart[] explosionEmitters;
    public ExplosionPart[] smokeEmitters;

    public ExplosionPart[] miscSpecialEffects;

    IEnumerator Start () {	
        //ExplosionPart go;
	    float maxTime = 0;
	
	    foreach(var go in ambientEmitters) {
		    StartCoroutine( InstantiateDelayed(go));
		    if (go.gameObject.GetComponent<ParticleEmitter>())
			    maxTime = Mathf.Max (maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
	    }
	    foreach(var go in explosionEmitters) {
            StartCoroutine(InstantiateDelayed(go));	
		    if (go.gameObject.GetComponent<ParticleEmitter>())
			    maxTime = Mathf.Max (maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
	    }
	    foreach(var go in smokeEmitters) {
            StartCoroutine(InstantiateDelayed(go));
		    if (go.gameObject.GetComponent<ParticleEmitter>())
			    maxTime = Mathf.Max (maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
	    }
	
	    if (GetComponent<AudioSource>() && GetComponent<AudioSource>().clip)
		    maxTime = Mathf.Max (maxTime, GetComponent<AudioSource>().clip.length);
	
	    yield return 0;
	
	    foreach(var go in miscSpecialEffects) {
            StartCoroutine(InstantiateDelayed(go));
		    if (go.gameObject.GetComponent<ParticleEmitter>())
			    maxTime = Mathf.Max (maxTime, go.delay + go.gameObject.GetComponent<ParticleEmitter>().maxEnergy);
	    }
	
	    Destroy (gameObject, maxTime + 0.5f);
    }

     // 延迟实例预制
    IEnumerator InstantiateDelayed ( ExplosionPart go  ){
	    if (go.hqOnly && QualityManager.quality < Quality.High)
		    yield return 0;
		
	    yield return new WaitForSeconds (go.delay);
	    Instantiate (go.gameObject, transform.position + Vector3.up * go.yOffset, transform.rotation);
    }

}