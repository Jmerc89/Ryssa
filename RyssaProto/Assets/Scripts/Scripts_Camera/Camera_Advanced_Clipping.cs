using UnityEngine;
using System.Collections.Generic;

public class AdvancedClipping : MonoBehaviour
{
    [Header("References")]
    public Transform target;                // The target the camera is following (e.g., the player)

    [Header("Transparency Settings")]
    [Range(0, 1)]
    public float transparentAlpha = 0.3f;   // Target alpha for obstructing objects
    public float fadeSpeed = 5f;            // Speed at which the fade occurs

    [Header("Tag Settings")]
    // Only objects with this tag will be processed for transparency.
    public string cullTag = "Cull";

    // Dictionary to keep track of renderers and their original material colors.
    // This stores the original color for each material so we know what to restore.
    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();

    void Update()
    {
        HandleObstructions();
    }

    private void HandleObstructions()
    {
        // Cast a ray from the camera to the target.
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance);

        // Use a hash set to track which renderers are currently obstructing.
        HashSet<Renderer> obstructingRenderers = new HashSet<Renderer>();

        // Process all hits along the ray.
        foreach (RaycastHit hit in hits)
        {
            // Only process objects with the specified cull tag.
            if (!hit.collider.CompareTag(cullTag))
                continue;

            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                obstructingRenderers.Add(rend);

                // If we haven't stored the original colors for this renderer, do so.
                if (!originalColors.ContainsKey(rend))
                {
                    Material[] mats = rend.materials;
                    Color[] origColors = new Color[mats.Length];
                    for (int i = 0; i < mats.Length; i++)
                    {
                        origColors[i] = mats[i].color;
                    }
                    originalColors[rend] = origColors;
                }

                // Gradually fade each material's alpha toward transparentAlpha.
                foreach (Material mat in rend.materials)
                {
                    Color c = mat.color;
                    c.a = Mathf.Lerp(c.a, transparentAlpha, Time.deltaTime * fadeSpeed);
                    mat.color = c;

                    // Ensure the material is set up for transparency.
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
            }
        }

        // Now restore any renderers that are no longer obstructing.
        List<Renderer> renderersToRestore = new List<Renderer>();
        foreach (Renderer rend in originalColors.Keys)
        {
            if (!obstructingRenderers.Contains(rend))
            {
                renderersToRestore.Add(rend);
            }
        }

        foreach (Renderer rend in renderersToRestore)
        {
            Color[] origColors = originalColors[rend];
            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Color c = mats[i].color;
                // Lerp back to the original alpha.
                c.a = Mathf.Lerp(c.a, origColors[i].a, Time.deltaTime * fadeSpeed);
                mats[i].color = c;

                // Optionally, if the material is nearly restored, reset its settings to opaque.
                if (Mathf.Abs(c.a - origColors[i].a) < 0.01f)
                {
                    mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mats[i].SetInt("_ZWrite", 1);
                    mats[i].DisableKeyword("_ALPHATEST_ON");
                    mats[i].DisableKeyword("_ALPHABLEND_ON");
                    mats[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mats[i].renderQueue = -1;
                }
            }
            // Once restored, remove it from our dictionary.
            if (Mathf.Abs(rend.materials[0].color.a - origColors[0].a) < 0.01f)
            {
                originalColors.Remove(rend);
            }
        }
    }
}
