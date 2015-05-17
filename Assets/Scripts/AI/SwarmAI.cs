using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuzzerData {
    public Transform transform;
    public MovementMotor motor;
    public AudioSource audio;
    public float charged = 0;
    public bool didChargeEffect = false;
    public int sign;
    public Material chargeMaterial;
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public ParticleEmitter electricBall;
    public LineRenderer electricArc;
}

public class SwarmAI : MonoBehaviour {

	public MovementMotor[] buzzers;
	public float zapDist = 2.3f;
	public float slowDownDist = 1.0f;
	public float rechargeDist = 5.5f;
	public float chargeTime = 6.0f;
	public float visibleChargeFraction = 0.8f;
	public float nonAttackSpeedFactor = 0.2f;
	public float damageAmount = 5.0f;
	public float minTimeBetweenAttacks = 1.0f;
	public AudioClip zapSound;
	public AudioClip rechargeSound;
	
	// Private memeber data
	private List<BuzzerData> buzz;
	private Transform player;
	private bool  attacking = false;
	private int attackerIndex = 0;
	private float nextAttackTime = 0;
	
	void Awake (){
		player = GameObject.FindWithTag ("Player").transform;
		
		buzz = new List<BuzzerData> ();
		for (int i = 0; i<buzzers.Length; i++) {
			BuzzerData buzzer = new BuzzerData();
			buzzer.motor = buzzers[i];
			if (!buzzers[i])
				Debug.Log ("buzzer not found at "+i, transform);	
			buzzer.transform = buzzers[i].transform;
			buzzer.audio = buzzers[i].GetComponent<AudioSource>();
			buzzer.sign = i % 2 == 0 ? 1 : -1;
			buzzer.chargeMaterial = buzzer.transform.Find("buzzer_bot/electric_buzzer_plasma").GetComponent<Renderer>().material;
			buzzer.spawnPosition = buzzer.transform.position;
			buzzer.spawnRotation = buzzer.transform.rotation;
				buzzer.electricBall = buzzer.transform.GetComponentInChildren (typeof( ParticleEmitter)) as ParticleEmitter;
				buzzer.electricArc = buzzer.electricBall.GetComponent<LineRenderer> ();
			buzz.Add (buzzer);
		}
		buzz[attackerIndex].charged = 0.5f;
	}
	
	void OnTriggerEnter ( Collider other  ){
		if (other.transform == player) {
			attacking = true;
			for (int i = 0; i<buzz.Count; i++) {
				buzz[i].motor.enabled = true;
			}
		}
	}
	
	void OnTriggerExit ( Collider other  ){
		if (other.transform == player) {
			attacking = false;
		}
	}
	
	void Update (){
		for (int c = buzz.Count-1; c>=0; c--) {
			if (buzz[c].transform == null) {
				buzz.RemoveAt (c);
				if (buzz.Count > 0)
					attackerIndex = attackerIndex % buzz.Count;
			}
		}
		if (buzz.Count == 0)
			return;
		
		if (attacking)
			UpdateAttack ();
		else
			UpdateRetreat ();
	}
	
	void UpdateRetreat (){
		for (int i = 0; i<buzz.Count; i++) {
			if (buzz[i].motor.enabled) {
				Vector3 spawnDir = (buzz[i].spawnPosition - buzz[i].transform.position);
				if (spawnDir.sqrMagnitude > 1)
					spawnDir.Normalize ();
				buzz[i].motor.movementDirection = spawnDir * nonAttackSpeedFactor;
				buzz[i].motor.facingDirection = buzz[i].spawnRotation * Vector3.forward;
				
				if (spawnDir.sqrMagnitude < 0.01f) {
					buzz[i].transform.position = buzz[i].spawnPosition;
					buzz[i].transform.rotation = buzz[i].spawnRotation;
					buzz[i].motor.enabled = false;
					buzz[i].transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
					buzz[i].transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				}
			}
		}
	}
	
	void UpdateAttack (){
		int count = buzz.Count;
		
		Vector3 attackerDir = player.position - buzz[attackerIndex].transform.position;
		attackerDir.y = 0;
		attackerDir.Normalize();
		// Rotate by 90 degrees the fast way
		Vector3 fleeDir = new Vector3(attackerDir.z, 0, -attackerDir.x);
		
		for (int i = 0; i<count; i++) {
			
			
			Vector3 playerDir = player.position - buzz[i].transform.position;
			float playerDist = playerDir.magnitude;
			playerDir /= playerDist;
			
			if (i == attackerIndex) {
				buzz[i].motor.facingDirection = playerDir;
				
				bool aimingCorrect= Vector3.Dot (buzz[i].transform.forward, playerDir) > 0.8f;
				
				if (!aimingCorrect || buzz[i].charged < 1 || Time.time < nextAttackTime) {
					if (playerDist < rechargeDist)
						buzz[i].motor.movementDirection = playerDir * -nonAttackSpeedFactor;
					else
						buzz[i].motor.movementDirection = Vector3.zero;
				}
				else {
					buzz[i].motor.movementDirection = playerDir;
					
					if (playerDist < zapDist + slowDownDist) {
						// Slow down when close;
						buzz[i].motor.movementDirection *= 0.01f;
						
						// Zap when within range
						if (playerDist < zapDist && aimingCorrect) {
							// Zap player here
							DoElectricArc (buzz[i]);
							
							// Apply damage
							Health targetHealth = player.GetComponent<Health> ();
							if (targetHealth) {
								targetHealth.OnDamage (damageAmount, -playerDir);
							}
							
							// Change active attacker
							buzz[i].charged = 0;
							attackerIndex = (attackerIndex + 1) % count;
							nextAttackTime = Time.time + minTimeBetweenAttacks * Random.Range (1.0f, 1.2f);
						}
					}
				}
			}
			else {
				Vector3 pos;
				float s = -Mathf.Sign(Vector3.Dot (fleeDir, playerDir));
				Vector3 posSide = player.position + Vector3.Project (-playerDir * playerDist, attackerDir) + fleeDir * s * rechargeDist;
				Vector3 posBehind = player.position + attackerDir * rechargeDist;
				float lerp = playerDist / rechargeDist;
				lerp = lerp * lerp;
				pos = Vector3.Lerp (posSide, posBehind, lerp * 0.6f);
				
				if (buzz[i].charged == 1)
					pos += Vector3.up * 2;
				
				buzz[i].motor.movementDirection = (pos - buzz[i].transform.position).normalized * nonAttackSpeedFactor;
				
				if ((i+1) % count == attackerIndex)
					buzz[i].motor.facingDirection = playerDir;
				else
					buzz[i].motor.facingDirection = buzz[i].motor.movementDirection;
			}
			
			// Recharge
			buzz[i].charged += Time.deltaTime / chargeTime;
			if (buzz[i].charged > 1)
				buzz[i].charged = 1;
				
			float visibleCharged = Mathf.InverseLerp (visibleChargeFraction, 1.0f, buzz[i].charged);
			buzz[i].electricBall.minSize = 0.30f * visibleCharged;
			buzz[i].electricBall.maxSize = 0.45f * visibleCharged;
			
			// Play rechage sound
			if (!buzz[i].didChargeEffect && visibleCharged > 0.5f) {
				buzz[i].audio.clip = rechargeSound;
				buzz[i].audio.Play ();
				buzz[i].didChargeEffect = true;
			}
			
			// Make charged buzzer glow
			buzz[i].chargeMaterial.mainTextureOffset = new Vector2 (0, (1-visibleCharged) * -1.9f);
			
			// Make charged buzzer vibrate
			buzz[i].motor.GetComponent<Rigidbody>().angularVelocity +=
				Random.onUnitSphere * 4 * visibleCharged;
		}
	}

    IEnumerator DoElectricArc(BuzzerData buzz)
    {
		// Play attack sound
		buzz.audio.clip = zapSound;
		buzz.audio.Play ();
		buzz.didChargeEffect = false;
		
		// Show electric arc
		buzz.electricArc.enabled = true;
		
		// Offset  electric arc texture while it's visible
		float stopTime = Time.time + 0.2f;
		while (Time.time < stopTime) {
			buzz.electricArc.SetPosition (0, buzz.electricArc.transform.position);
			buzz.electricArc.SetPosition (1, player.position);			
            buzz.electricArc.sharedMaterial.mainTextureOffset = new Vector2(Random.value, buzz.electricArc.sharedMaterial.mainTextureOffset.y);
			yield return 0;
		}
		
		// Hide electric arc
		buzz.electricArc.enabled = false;
	}
}
