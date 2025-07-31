using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class WeldingBlobSet : MonoBehaviour

{

    public Mesh flatBlobMesh;

    public Material blobCooledMaterial, blobHotMaterial;



    [SerializeField] private float coolingDelay = 1.5f;

    [SerializeField] private float coolingFade = 1.5f;



    internal bool tiltForward = false;



    Color fadeToCool = new Color(0.5f, 0.5f, 0.3f, 1f);



    // Start is called before the first frame update

    private void Start()
    {
        // 1. Make the blob hot IMMEDIATELY.
        // We create a new instance of the material so we can fade it without affecting other blobs.
        Material materialToFade = Instantiate(blobHotMaterial);
        GetComponent<MeshRenderer>().material = materialToFade;

        // 2. Now, create a delay BEFORE the cooling fade begins.
        LeanTween.value(gameObject, 0, 1, coolingDelay).setOnComplete(() =>
        {
            // 3. After the delay, start the fade animation over the 'coolingFade' duration.
            LeanTween.value(gameObject, 0, 1, coolingFade).setOnUpdate((float val) => {

                // This lerps the color and emission over time.
                materialToFade.color = Color.Lerp(materialToFade.color, fadeToCool, val);
                materialToFade.SetColor("_EmissionColor", new Color(1 - val, 1 - val, 0, 1 - val));

            }).setOnComplete(() =>
            {
                // 4. Once the fade is finished, switch to the final cooled material.
                GetComponent<MeshRenderer>().material = blobCooledMaterial;
                GetComponent<MeshFilter>().mesh = flatBlobMesh;

                // Clean up the material instance we created.
                Destroy(materialToFade);
            });
        });
    }





    internal void ShowGlow()

    {

        GetComponent<MeshRenderer>().material = blobHotMaterial;

        tiltForward = false;

        Start();



    }



}