using UnityEngine;
using System.Collections;

public class WeldBlob : MonoBehaviour
{
    [Header("Settings")]
    public Material cooledMaterial;
    public float coolingDuration = 7f;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        // Get the MeshRenderer component when the blob is created
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // Immediately start the cooling down process
        StartCoroutine(CoolDownRoutine());
    }

    private IEnumerator CoolDownRoutine()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(coolingDuration);

        // After waiting, switch to the cooled material
        if (meshRenderer != null && cooledMaterial != null)
        {
            meshRenderer.material = cooledMaterial;
        }
    }
}