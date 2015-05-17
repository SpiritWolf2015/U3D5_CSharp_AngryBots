using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MaterialImpact {
    public PhysicMaterial physicMaterial;
    public AudioClip[] playerFootstepSounds;
    public AudioClip[] mechFootstepSounds;
    public AudioClip[] spiderFootstepSounds;
    public AudioClip[] bulletHitSounds;
}

public class MaterialImpactManager : MonoBehaviour {

	public MaterialImpact[] materials;

    private static Dictionary<PhysicMaterial, MaterialImpact> dict;
	private static MaterialImpact defaultMat;
	
	void Awake (){
		defaultMat = materials[0];
		
		dict = new Dictionary<PhysicMaterial, MaterialImpact> ();
		for (int i = 0; i < materials.Length; i++) {
			dict.Add (materials[i].physicMaterial, materials[i]);
		}
	}
	
	public static AudioClip GetPlayerFootstepSound ( PhysicMaterial mat  ){
		MaterialImpact imp = GetMaterialImpact (mat);
		return GetRandomSoundFromArray(imp.playerFootstepSounds);
	}
	
	public static AudioClip GetMechFootstepSound ( PhysicMaterial mat  ){
		MaterialImpact imp = GetMaterialImpact (mat);
		return GetRandomSoundFromArray(imp.mechFootstepSounds);
	}
	
	public static AudioClip GetSpiderFootstepSound ( PhysicMaterial mat  ){
		MaterialImpact imp = GetMaterialImpact (mat);
		return GetRandomSoundFromArray(imp.spiderFootstepSounds);
	}
	
	public static AudioClip GetBulletHitSound ( PhysicMaterial mat  ){
		MaterialImpact imp = GetMaterialImpact (mat);
		return GetRandomSoundFromArray(imp.bulletHitSounds);
	}
	
	public static MaterialImpact GetMaterialImpact ( PhysicMaterial mat  ){
		if (mat && dict.ContainsKey (mat))
			return dict[mat];
		return defaultMat;
	}
	
	public static AudioClip GetRandomSoundFromArray ( AudioClip[] audioClipArray  ){
		if (audioClipArray.Length > 0)
			return audioClipArray[Random.Range (0, audioClipArray.Length)];
		return null;
	}

}