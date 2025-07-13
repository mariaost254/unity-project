using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PanoramaDisplay : MonoBehaviour
{
    private Renderer sphereRenderer;

    void Awake()
    {
        sphereRenderer = GetComponent<Renderer>();
    }

    public void SetPanorama(Texture2D tex)
    {
        if (sphereRenderer != null && tex != null)
        {
            sphereRenderer.material.mainTexture = tex;
        }
    }
}
