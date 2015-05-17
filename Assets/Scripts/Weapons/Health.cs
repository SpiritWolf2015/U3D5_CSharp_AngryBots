using UnityEngine;
using System.Collections;

//2013年7月27日18:23:29，郭志程

public class Health : MonoBehaviour {

    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float regenerateSpeed = 0.0f;
    public bool  invincible = false;
    public bool  dead = false;

    public GameObject damagePrefab;
    public Transform damageEffectTransform;
    public float damageEffectMultiplier = 1.0f;
    public bool  damageEffectCentered = true;

    public GameObject scorchMarkPrefab = null;
    private GameObject scorchMark = null;

    public SignalSender damageSignals = new SignalSender();          // SignalSender,位于Modules（模块，组件）文件夹下的脚本
    public SignalSender dieSignals = new SignalSender();

    private float lastDamageTime = 0;
    private ParticleEmitter damageEffect;
    private float damageEffectCenterYOffset;

    private float colliderRadiusHeuristic = 1.0f;

    void Awake (){
	    enabled = false;
	    if (damagePrefab) {
		    if (damageEffectTransform == null)
			    damageEffectTransform = transform;
            GameObject effect = Spawner.Spawn(damagePrefab, Vector3.zero, Quaternion.identity);    // Spawner ：产卵 ,位于Managers文件夹下的脚本
		    effect.transform.parent = damageEffectTransform;
		    effect.transform.localPosition = Vector3.zero;
		    damageEffect = effect.GetComponent<ParticleEmitter>();
		    Vector2 tempSize = new Vector2(GetComponent<Collider>().bounds.extents.x,GetComponent<Collider>().bounds.extents.z);
		    colliderRadiusHeuristic = tempSize.magnitude * 0.5f;
		    damageEffectCenterYOffset = GetComponent<Collider>().bounds.extents.y;

	    }
	    if (scorchMarkPrefab) {
		    scorchMark = GameObject.Instantiate(scorchMarkPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		    scorchMark.SetActive (false);
	    }
    }

    /// <summary>
    /// 被攻击,不是u3d api函数
    /// </summary>       
    public void OnDamage ( float amount ,   Vector3 fromDirection  ) {
	    // Take no damage if invincible, dead, or if the damage is zero
	    if(invincible)
		    return;
	    if (dead)
		    return;
	    if (amount <= 0)
		    return;

	    // Decrease health by damage and send damage signals

	    // @HACK: this hack will be removed for the final game
	    //  but makes playing and showing certain areas in the
	    //  game a lot easier
	    /*
	    #if !UNITY_IPHONE && !UNITY_ANDROID
	    if(gameObject.tag != "Player")
		    amount *= 10.0f;
	    #endif
	    */

	    health -= amount;
	    damageSignals.SendSignals (this);
	    lastDamageTime = Time.time;

	    // Enable so the Update function will be called
	    // if regeneration is enabled
	    if (regenerateSpeed > 0)
		    enabled = true;

	    // Show damage effect if there is one
	    if (damageEffect) {
		    damageEffect.transform.rotation = Quaternion.LookRotation (fromDirection, Vector3.up);
		    if(!damageEffectCentered) {
			    Vector3 dir = fromDirection;
			    dir.y = 0.0f;
			    damageEffect.transform.position = (transform.position + Vector3.up * damageEffectCenterYOffset) + colliderRadiusHeuristic * dir;
		    }
		    // @NOTE: due to popular demand (ethan, storm) we decided
		    // to make the amount damage independent ...
		    //FIXME_VAR_TYPE particleAmount= Random.Range (damageEffect.minEmission, damageEffect.maxEmission + 1);
		    //particleAmount = particleAmount * amount * damageEffectMultiplier;
		    damageEffect.Emit();// (particleAmount);
	    }

	    // Die if no health left
	    if (health <= 0) {
		    GameScore.RegisterDeath (gameObject);

		    health = 0;
		    dead = true;
		    dieSignals.SendSignals (this);
		    enabled = false;

		    // scorch marks
		    if (scorchMark) {
			    scorchMark.SetActive (true);
			    // @NOTE: maybe we can justify a raycast here so we can place the mark
			    // on slopes with proper normal alignments
			    // @TODO: spawn a yield return new Sub() to handle placement, as we can
			    // spread calculations over several frames => cheap in total
			    Vector3 scorchPosition = GetComponent<Collider>().ClosestPointOnBounds (transform.position - Vector3.up * 100);
			    scorchMark.transform.position = scorchPosition + Vector3.up * 0.1f;			
                scorchMark.transform.eulerAngles = new Vector3(scorchMark.transform.eulerAngles.x, Random.Range(0.0f, 90.0f), scorchMark.transform.eulerAngles.z);//y值随机0到90
		    }
	    }
    }

    void OnEnable (){
        StartCoroutine(Regenerate());
    }

    // 回血
    IEnumerator Regenerate() {
	    if (regenerateSpeed > 0.0f) {
		    while (enabled) {
			    if (Time.time > lastDamageTime + 3) {
				    health += regenerateSpeed;

				    yield return 0;

				    if (health >= maxHealth) {
					    health = maxHealth;
					    enabled = false;
				    }
			    }
			    yield return new WaitForSeconds (1.0f);
		    }
	    }
    }

}