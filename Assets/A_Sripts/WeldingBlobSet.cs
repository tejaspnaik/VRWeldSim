using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingBlobSet : MonoBehaviour
{
    public Mesh originalBlobMesh;
    public Mesh flatBlobMesh;
    public Material blobCooledMaterial, blobHotMaterial;

    // --- VARIABLE ADDED BACK ---
    // This variable is needed by the WeldingHandle.cs script.
    // Making it 'public' is the standard way to allow access from other scripts.
    public bool tiltForward = false;
    // -------------------------

    [SerializeField] private float coolingDelay = 1.5f;
    [SerializeField] private float coolingFade = 1.5f;

    [SerializeField] private Color cooledWeldColor = new Color(0.5f, 0.5f, 0.3f, 1f);

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        if (originalBlobMesh == null)
        {
            originalBlobMesh = meshFilter.mesh;
        }
    }

    private void Start()
    {
        BeginCoolingSequence();
    }

    public void ShowGlow()
    {
        BeginCoolingSequence();
    }

    private void BeginCoolingSequence()
    {
        LeanTween.cancel(gameObject);

        // --- RESET LOGIC ADDED BACK ---
        // Reset the flag to its default state to match original behavior.
        tiltForward = false;
        // ------------------------------

        meshFilter.mesh = originalBlobMesh;

        Material materialToFade = Instantiate(blobHotMaterial);
        meshRenderer.material = materialToFade;

        Color startColor = materialToFade.color;
        Color startEmission = materialToFade.GetColor("_EmissionColor");

        LeanTween.value(gameObject, 0, 1, coolingDelay).setOnComplete(() =>
        {
            LeanTween.value(gameObject, 0, 1, coolingFade).setOnUpdate((float val) =>
            {
                materialToFade.color = Color.Lerp(startColor, cooledWeldColor, val);
                materialToFade.SetColor("_EmissionColor", Color.Lerp(startEmission, Color.black, val));

            }).setOnComplete(() =>
            {
                meshRenderer.material = blobCooledMaterial;
                meshFilter.mesh = flatBlobMesh;
                Destroy(materialToFade);
            });
        });
    }

    private void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }
}