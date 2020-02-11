using UnityEngine;
 
public class RipplePostProcessor : MonoBehaviour
{
    public Material RippleMaterial;
 
    [Range(0,1)]
    public float Friction = .9f;
 
    private float Amount = 0f;
 
    void Update()
    { 
        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }

    public void RippleEffect (Vector3 position, float rippleAmount)
	{
		position = GameManager.mainCamera.WorldToScreenPoint(position);
		this.Amount = rippleAmount;
		this.RippleMaterial.SetFloat("_CenterX", position.x);
		this.RippleMaterial.SetFloat("_CenterY", position.y);
	}
 
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, this.RippleMaterial);
    }
}