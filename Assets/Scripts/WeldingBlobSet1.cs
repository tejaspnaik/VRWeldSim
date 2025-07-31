using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingBlobSet1 : MonoBehaviour
{
    public Mesh flatBlobMesh;
    public Material blobCooledMaterial, blobHotMaterial;

    [SerializeField] private float coolingDelay = 1.5f;
    [SerializeField] private float coolingFade = 1.5f;

    // --- NEW: Joint Creation Logic ---
    [Header("Joint Settings")]
    [Tooltip("How far the blob looks to find pieces to join.")]
    public float jointSearchRadius = 0.1f;
    private bool jointCheckHasBeenPerformed = false;
    // --- END NEW ---

    internal bool tiltForward = false;
    Color fadeToCool = new Color(0.5f, 0.5f, 0.3f, 1f);

    private void Start()
    {
        // --- NEW: Attempt to create a joint ONCE when the blob is first created ---
        if (!jointCheckHasBeenPerformed)
        {
            TryCreateJoint();
            jointCheckHasBeenPerformed = true; // Ensure this only runs once.
        }
        // --- END NEW ---

        // 1. Make the blob hot IMMEDIATELY.
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

    // --- NEW: This entire method creates the physical joint ---
    private void TryCreateJoint()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, jointSearchRadius);

        Rigidbody rb1 = null;
        Rigidbody rb2 = null;

        foreach (var col in nearbyColliders)
        {
            if (col.CompareTag("Weldable"))
            {
                Rigidbody currentRb = col.GetComponentInParent<Rigidbody>();
                if (currentRb == null) continue;

                if (rb1 == null)
                {
                    rb1 = currentRb;
                }
                else if (currentRb != rb1)
                {
                    rb2 = currentRb;
                    break;
                }
            }
        }

        if (rb1 != null && rb2 != null)
        {
            bool alreadyConnected = false;
            FixedJoint[] existingJoints = rb1.GetComponents<FixedJoint>();
            foreach (var joint in existingJoints)
            {
                if (joint.connectedBody == rb2)
                {
                    alreadyConnected = true;
                    break;
                }
            }

            if (!alreadyConnected)
            {
                Debug.Log($"Weld joint created between {rb1.name} and {rb2.name}");
                FixedJoint newJoint = rb1.gameObject.AddComponent<FixedJoint>();
                newJoint.connectedBody = rb2;
            }
        }
    }
    // --- END NEW ---

    internal void ShowGlow()
    {
        GetComponent<MeshRenderer>().material = blobHotMaterial;
        tiltForward = false;
        Start();
    }
}