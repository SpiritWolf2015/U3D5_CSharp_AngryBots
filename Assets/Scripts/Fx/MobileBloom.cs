using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Camera))]
[AddComponentMenu("Image Effects/Mobile Bloom")]
public class MobileBloom : MonoBehaviour {

    public float intensity = 0.5f;
    public Color colorMix = Color.white;
    public float colorMixBlend = 0.25f;
    public float agonyTint = 0.0f;

    private Shader bloomShader;
    private Material apply = null;
    private RenderTextureFormat rtFormat = RenderTextureFormat.Default;

    void Start () {
	    FindShaders ();
	    CheckSupport ();
	    CreateMaterials ();	
    }

    void FindShaders () {	
	    if (!bloomShader)
		    bloomShader = Shader.Find("Hidden/MobileBloom");
    }

    void CreateMaterials () {		
	    if (!apply) {
		    apply = new Material (bloomShader);	
		    apply.hideFlags = HideFlags.DontSave;
	    }           
    }

    void OnDamage () {
	    agonyTint = 1.0f;	
    }

    bool Supported () {
	     return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && bloomShader.isSupported);
    }

    bool CheckSupport () {
	     if (!Supported ()) {
		    enabled = false;
		    return false;
	    }	
	    rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
	    return true;
    }

    void OnDisable () {
	    if (apply) {
		    DestroyImmediate (apply);
		    apply = null;	
	    }
    }

    void OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){		
    #if UNITY_EDITOR
	    FindShaders ();
	    CheckSupport ();
	    CreateMaterials ();	
    #endif

	    agonyTint = Mathf.Clamp01 (agonyTint - Time.deltaTime * 2.75f);
		
	    RenderTexture tempRtLowA = RenderTexture.GetTemporary (source.width / 4, source.height / 4,0, rtFormat);
	    RenderTexture tempRtLowB = RenderTexture.GetTemporary (source.width / 4, source.height / 4, 0,rtFormat);
        
	    // prepare data
	
	    apply.SetColor ("_ColorMix", colorMix);
	    apply.SetVector ("_Parameter",new Vector4 (colorMixBlend * 0.25f,  0.0f, 0.0f, 1.0f - intensity - agonyTint));	
	
	    // downsample & blur
	
	    Graphics.Blit (source, tempRtLowA, apply, agonyTint < 0.5f ? 1 : 5);
	    Graphics.Blit (tempRtLowA, tempRtLowB, apply, 2);
	    Graphics.Blit (tempRtLowB, tempRtLowA, apply, 3);
	
	    // apply
	
	    apply.SetTexture ("_Bloom", tempRtLowA);
	    Graphics.Blit (source, destination, apply, QualityManager.quality > Quality.Medium ? 4 : 0);
	
	    RenderTexture.ReleaseTemporary (tempRtLowA);
	    RenderTexture.ReleaseTemporary (tempRtLowB);
    }

}