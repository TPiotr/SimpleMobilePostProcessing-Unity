using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessor : MonoBehaviour
{
    public static class ShaderUniforms {
        public static readonly int VignettePower = Shader.PropertyToID("_VignettePower");
        public static readonly int VignetteCenter = Shader.PropertyToID("_VignetteCenter");

		public static readonly int BlurRadius = Shader.PropertyToID("_Radius");
		public static readonly int BlurDir = Shader.PropertyToID("_Dir");
    }

	private List<Material> Materials = new List<Material>();
    
	public bool VignetteEnabled = true;
	public float VignettePower = 0.5f;
    public Vector2 VignetteCenter = new Vector2(0.0f, 0.0f);

	public Shader VignetteShader;	
	private Material CurVignetteMaterial;
	private Material VignetteMaterial {
		get {
			if(CurVignetteMaterial == null) {
				CurVignetteMaterial = new Material(VignetteShader);
				CurVignetteMaterial.hideFlags = HideFlags.HideAndDontSave;	
				Materials.Add(CurVignetteMaterial);
			}
			return CurVignetteMaterial;
		}
	}

	[Space]
	public bool BlurEnabled = false;
	[Range(0, 10)] 
	public float BlurRadius = 2f;
	[Range(1, 10)] 
	public float BlurTextureResolutionDivider = 1;

	public Shader BlurShader;
	private Material CurBlurMaterial;
	private Material BlurMaterial {
		get {
			if(CurBlurMaterial == null) {
				CurBlurMaterial = new Material(BlurShader);
				CurBlurMaterial.hideFlags = HideFlags.HideAndDontSave;	
				Materials.Add(CurBlurMaterial);
			}
			return CurBlurMaterial;
		}
	}

	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture) {
		RenderTexture currentTarget = sourceTexture;
		
		//Vignette
		if(VignetteEnabled) {
			RenderTexture vignetteTarget = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
			ApplyVignette(sourceTexture, vignetteTarget);
			ReleaseTemporary(currentTarget, sourceTexture);
			currentTarget = vignetteTarget;
		}

		//blur whole screen
		if(BlurEnabled && BlurRadius > 0) {
			//clamp blur parameters
			BlurTextureResolutionDivider = Mathf.Max(1f, BlurTextureResolutionDivider);
			BlurRadius = Mathf.Max(0f, BlurRadius);

			//H blur
			RenderTexture hBlurTarget = RenderTexture.GetTemporary((int) (currentTarget.width / BlurTextureResolutionDivider), 
																(int) (currentTarget.height / BlurTextureResolutionDivider));
			
			RenderTexture vBlurTarget = RenderTexture.GetTemporary((int) (currentTarget.width / BlurTextureResolutionDivider), 
																(int) (currentTarget.height / BlurTextureResolutionDivider));
			
			ApplyBlur(currentTarget, vBlurTarget, hBlurTarget, sourceTexture);
			ReleaseTemporary(currentTarget, sourceTexture); //release render target from previous post process effect!
			currentTarget = hBlurTarget;
		}

		//copy last render target to screen buffer
		Graphics.Blit(currentTarget, destTexture);
		ReleaseTemporary(currentTarget, sourceTexture);
	}
	
	private void ReleaseTemporary(RenderTexture texture, RenderTexture sourceTexture) {
		if(sourceTexture != texture) {
			RenderTexture.ReleaseTemporary(texture);
		}
	}

	private void ApplyVignette(RenderTexture from, RenderTexture to) {
		if(VignetteShader != null) {
			VignetteMaterial.SetFloat(ShaderUniforms.VignettePower, VignettePower);
            VignetteMaterial.SetVector(ShaderUniforms.VignetteCenter, VignetteCenter);
			Graphics.Blit(from, to, VignetteMaterial);
		} else {
			Graphics.Blit(from, to);	
			Debug.LogError("Vignette shader missing");
		}
	}

	private void ApplyBlur(RenderTexture currentTarget, RenderTexture vBlurTarget, RenderTexture hBlurTarget, RenderTexture sourceTexture) {
		if(BlurShader != null) {
			//first blur screen image verticaly
			BlurMaterial.SetVector(ShaderUniforms.BlurDir, new Vector2(0f, 1f));
			BlurMaterial.SetFloat(ShaderUniforms.BlurRadius, BlurRadius);
			Graphics.Blit(currentTarget, vBlurTarget, BlurMaterial);

			//then blur it horizontaly
			BlurMaterial.SetVector(ShaderUniforms.BlurDir, new Vector2(1f, 0f));
			Graphics.Blit(vBlurTarget, hBlurTarget, BlurMaterial);
			ReleaseTemporary(vBlurTarget, sourceTexture);
		} else {
			Graphics.Blit(currentTarget, hBlurTarget);
			Debug.LogError("Blur shader missing");	
		}
	}
	void OnDisable() {
		foreach(Material material in Materials) {
			if(material)
				DestroyImmediate(material);
		}
		Materials.Clear();
	}
}
