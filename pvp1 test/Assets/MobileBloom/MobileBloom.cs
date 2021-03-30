using UnityEngine;
using UnityEngine.UI;

public class MobileBloom : MonoBehaviour{
    [Range(0, 5)]
    public float BloomAmount = 1f;
    [Range(0, 5)]
    public float BlurAmount = 2f;
	[Range(0, 1)]
	public float FadeAmount = 0.2f;
    static readonly int scrWidth=Screen.width/4;
	static readonly int scrHeight=Screen.height/4;
    static readonly int blAmountString = Shader.PropertyToID("_BloomAmount");
    static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
	static readonly int fadeAmountString = Shader.PropertyToID("_FadeAmount");
    static readonly int bloomTexString = Shader.PropertyToID("_BloomTex");
    public Material material=null;

	void  OnRenderImage (RenderTexture source ,   RenderTexture destination){
        material.SetFloat(blurAmountString, BlurAmount/2.0f);
		material.SetFloat(fadeAmountString, FadeAmount);
        RenderTexture buffer = RenderTexture.GetTemporary(scrWidth, scrHeight, 0,source.format);
        Graphics.Blit(source, buffer, material,0);

		RenderTexture temp = RenderTexture.GetTemporary(scrWidth/2, scrHeight/2, 0, source.format);
		Graphics.Blit(buffer, temp, material, 1);
		RenderTexture.ReleaseTemporary(buffer);

		RenderTexture temp2 = RenderTexture.GetTemporary(scrWidth, scrHeight, 0, source.format);
		Graphics.Blit(temp, temp2, material, 1);
		RenderTexture.ReleaseTemporary(temp);

        material.SetFloat(blAmountString, BloomAmount);
        material.SetTexture(bloomTexString, temp2);
        Graphics.Blit(source, destination, material,2);
		RenderTexture.ReleaseTemporary(temp2);
    }

    public void Blor(Slider a)
    {
        BlurAmount = a.value;
    }
    public void Blum(Slider a)
    {
        BloomAmount = a.value;
    }
}
