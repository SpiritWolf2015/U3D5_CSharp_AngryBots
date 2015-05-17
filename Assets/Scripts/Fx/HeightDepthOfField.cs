using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (Camera))]
[AddComponentMenu("Image Effects/Height Depth of Field")]
public class HeightDepthOfField : MonoBehaviour {

    public enum DofQualitySetting {
	    OnlyBackground = 1,
	    BackgroundAndForeground = 2,
    }

    public enum DofResolution{
	    High = 2,
	    Medium = 3,
	    Low = 4,	
    }
	
    public DofResolution resolution  = DofResolution.High;
	
    public Transform objectFocus = null;
 
    public float maxBlurSpread = 1.55f;
    public float foregroundBlurExtrude = 1.055f;
    public float smoothness = 1.0f;
		
    private Shader dofBlurShader;
    private Material dofBlurMaterial = null;	

    private Shader dofShader;
    private Material dofMaterial = null;
   
    public bool  visualize = false;
   
    private float widthOverHeight = 1.25f;
    private float oneOverBaseSize = 1.0f / 512.0f;

    private float cameraNear = 0.5f;
    private float cameraFar = 50.0f;
    private float cameraFov = 60.0f;	
    private float cameraAspect = 1.333333f;

    void Start (){
	    FindShaders ();
	    CheckSupport ();
	    CreateMaterials ();	
    }

    void FindShaders (){	
	    if (!dofBlurShader)
		    dofBlurShader = Shader.Find("Hidden/BlurPassesForDOF");
	    if (!dofShader)
		    dofShader = Shader.Find("Hidden/HeightDepthOfField");	
    }

    void CreateMaterials (){		
	    if (!dofBlurMaterial)
		    dofBlurMaterial = PostEffects.CheckShaderAndCreateMaterial (dofBlurShader, dofBlurMaterial);
	    if (!dofMaterial)
		    dofMaterial = PostEffects.CheckShaderAndCreateMaterial (dofShader, dofMaterial);           
    }

    bool Supported (){
	     return (PostEffects.CheckSupport (true) && dofBlurShader.isSupported && dofShader.isSupported);
    }

    bool CheckSupport (){
	     if (!Supported ()) {
		    enabled = false;
		    return false;
	    }	
	    return true;
    }

    void OnDisable (){
	    if (dofBlurMaterial) {
		    DestroyImmediate (dofBlurMaterial);
		    dofBlurMaterial = null;	
	    }	
	    if (dofMaterial) {
		    DestroyImmediate (dofMaterial);
		    dofMaterial = null;	
	    }
    }

void OnRenderImage ( RenderTexture source ,   RenderTexture destination  ){	
#if UNITY_EDITOR
	FindShaders ();
	CheckSupport ();
	CreateMaterials ();	
#endif
	
	widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
	oneOverBaseSize = 1.0f / 512.0f;		
	
	cameraNear = GetComponent<Camera>().nearClipPlane;
	cameraFar = GetComponent<Camera>().farClipPlane;
	cameraFov = GetComponent<Camera>().fieldOfView;
	cameraAspect = GetComponent<Camera>().aspect;

	Matrix4x4 frustumCorners = Matrix4x4.identity;		
	Vector4 vec;
	Vector3 corner;
	float fovWHalf = cameraFov * 0.5f;
	Vector3 toRight = GetComponent<Camera>().transform.right * cameraNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * cameraAspect;
	Vector3 toTop = GetComponent<Camera>().transform.up * cameraNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);
	Vector3 topLeft = (GetComponent<Camera>().transform.forward * cameraNear - toRight + toTop);
	float cameraScaleFactor = topLeft.magnitude * cameraFar/cameraNear;	
		
	topLeft.Normalize();
	topLeft *= cameraScaleFactor;

	Vector3 topRight = (GetComponent<Camera>().transform.forward * cameraNear + toRight + toTop);
	topRight.Normalize();
	topRight *= cameraScaleFactor;
	
	Vector3 bottomRight = (GetComponent<Camera>().transform.forward * cameraNear + toRight - toTop);
	bottomRight.Normalize();
	bottomRight *= cameraScaleFactor;
	
	Vector3 bottomLeft = (GetComponent<Camera>().transform.forward * cameraNear - toRight - toTop);
	bottomLeft.Normalize();
	bottomLeft *= cameraScaleFactor;
			
	frustumCorners.SetRow (0, topLeft); 
	frustumCorners.SetRow (1, topRight);		
	frustumCorners.SetRow (2, bottomRight);
	frustumCorners.SetRow (3, bottomLeft);	
	
	dofMaterial.SetMatrix ("_FrustumCornersWS", frustumCorners);
	dofMaterial.SetVector ("_CameraWS", GetComponent<Camera>().transform.position);			
	
	Transform t;
	if (null !=objectFocus)
		t = GetComponent<Camera>().transform;
	else
		t = objectFocus.transform;
																	
	dofMaterial.SetVector ("_ObjectFocusParameter",new Vector4 (	
				t.position.y - 0.25f, t.localScale.y * 1.0f / smoothness, 1.0f, objectFocus ? objectFocus.GetComponent<Collider>().bounds.extents.y * 0.75f : 0.55f));
       		
	dofMaterial.SetFloat ("_ForegroundBlurExtrude", foregroundBlurExtrude);
	dofMaterial.SetVector ("_InvRenderTargetSize",new Vector4 (1.0f / (1.0f * source.width), 1.0f / (1.0f * source.height),0.0f,0.0f));
	
	int divider = 1;
	if (resolution == DofResolution.Medium)
		divider = 2;
	else if (resolution >= DofResolution.Medium)
		divider = 3;
	
	RenderTexture hrTex = RenderTexture.GetTemporary (source.width, source.height, 0); 
	RenderTexture mediumTexture = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);    
	RenderTexture mediumTexture2 = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);    
	RenderTexture lowTexture = RenderTexture.GetTemporary (source.width / (divider * 2), source.height / (divider * 2), 0);     
	
	source.filterMode = FilterMode.Bilinear;
	hrTex.filterMode = FilterMode.Bilinear;   
	lowTexture.filterMode = FilterMode.Bilinear;     
	mediumTexture.filterMode = FilterMode.Bilinear;
	mediumTexture2.filterMode = FilterMode.Bilinear;
	
    // background (coc -> alpha channel)
   	CustomGraphicsBlit (null, source, dofMaterial, 3);		
   		
   	// better downsample (should actually be weighted for higher quality)
   	mediumTexture2.DiscardContents();
   	Graphics.Blit (source, mediumTexture2, dofMaterial, 6);	
			
	Blur (mediumTexture2, mediumTexture, 1, 0, maxBlurSpread * 0.75f);			
	Blur (mediumTexture, lowTexture, 2, 0, maxBlurSpread);			
    	      		
	// some final calculations can be performed in low resolution 		
	dofBlurMaterial.SetTexture ("_TapLow", lowTexture);
	dofBlurMaterial.SetTexture ("_TapMedium", mediumTexture);							
	Graphics.Blit (null, mediumTexture2, dofBlurMaterial, 2);
	
	dofMaterial.SetTexture ("_TapLowBackground", mediumTexture2); 
	dofMaterial.SetTexture ("_TapMedium", mediumTexture); // only needed for debugging		
							
	// apply background defocus
	hrTex.DiscardContents();
	Graphics.Blit (source, hrTex, dofMaterial, visualize ? 2 : 0); 
	
	// foreground handling
	CustomGraphicsBlit (hrTex, source, dofMaterial, 5); 
	
	// better downsample and blur (shouldn't be weighted)
	Graphics.Blit (source, mediumTexture2, dofMaterial, 6);					
	Blur (mediumTexture2, mediumTexture, 1, 1, maxBlurSpread * 0.75f);	
	Blur (mediumTexture, lowTexture, 2, 1, maxBlurSpread);	
	
	// some final calculations can be performed in low resolution		
	dofBlurMaterial.SetTexture ("_TapLow", lowTexture);
	dofBlurMaterial.SetTexture ("_TapMedium", mediumTexture);							
	Graphics.Blit (null, mediumTexture2, dofBlurMaterial, 2);	
	
	if (destination != null)
	    destination.DiscardContents ();
	    
	dofMaterial.SetTexture ("_TapLowForeground", mediumTexture2); 
	dofMaterial.SetTexture ("_TapMedium", mediumTexture); // only needed for debugging	       
	Graphics.Blit (source, destination, dofMaterial,1);	
	
	RenderTexture.ReleaseTemporary (hrTex);
	RenderTexture.ReleaseTemporary (mediumTexture);
	RenderTexture.ReleaseTemporary (mediumTexture2);
	RenderTexture.ReleaseTemporary (lowTexture);
}	

// flat blur

void Blur ( RenderTexture from ,   RenderTexture to ,   int iterations ,   int blurPass ,   float spread  ){
	RenderTexture tmp = RenderTexture.GetTemporary (to.width, to.height, 0);
	
	if (iterations < 2) {
		dofBlurMaterial.SetVector ("offsets",new Vector4 (0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
		tmp.DiscardContents ();
		Graphics.Blit (from, tmp, dofBlurMaterial, blurPass);
	
		dofBlurMaterial.SetVector ("offsets",new Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0f, 0.0f, 0.0f));		
		to.DiscardContents ();
		Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 	
	} 
	else {	
		dofBlurMaterial.SetVector ("offsets",new Vector4 (0.0f, spread * oneOverBaseSize, 0.0f, 0.0f));
		tmp.DiscardContents ();
		Graphics.Blit (from, tmp, dofBlurMaterial, blurPass);
		
		dofBlurMaterial.SetVector ("offsets",new Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0f, 0.0f, 0.0f));		
		to.DiscardContents ();
		Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 
	
		dofBlurMaterial.SetVector ("offsets",new Vector4 (spread / widthOverHeight * oneOverBaseSize,  spread * oneOverBaseSize, 0.0f, 0.0f));		
		tmp.DiscardContents ();
		Graphics.Blit (to, tmp, dofBlurMaterial, blurPass);	
	
		dofBlurMaterial.SetVector ("offsets",new Vector4 (spread / widthOverHeight * oneOverBaseSize,  -spread * oneOverBaseSize, 0.0f, 0.0f));		
		to.DiscardContents ();
		Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	
	}
	
	RenderTexture.ReleaseTemporary (tmp);
}

// used for noise

void CustomGraphicsBlit ( RenderTexture source ,   RenderTexture dest ,   Material fxMaterial ,   int passNr  ){
	RenderTexture.active = dest;
	       
	fxMaterial.SetTexture ("_MainTex", source);	        
        	        
	GL.PushMatrix ();
	GL.LoadOrtho ();	
    	
	fxMaterial.SetPass (passNr);	
	
    GL.Begin (GL.QUADS);
						
	GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
	GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL
	
	GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
	GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR
	
	GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
	GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR
	
	GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
	GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL
	
	GL.End ();
    GL.PopMatrix ();
}	

}