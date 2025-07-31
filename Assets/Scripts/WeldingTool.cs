using UnityEngine;

public class WeldingTool : MonoBehaviour
{
    [Header("Welding Settings")]
    public GameObject weldBlobPrefab;
    public float weldRate = 15f;
    public float weldOffset = 0.02f;
    public LayerMask weldableLayer;
    public Transform raycastOrigin;

    [Header("Effects")]
    public AudioSource weldingAudioSource;
    public ParticleSystem weldingSparks;
    public ParticleSystem weldingFlame; // --- NEW --- Added reference for the flame effect

    // --- Private Fields ---
    private bool isWeldingActive = false; // True when trigger is held
    private bool isTouchingWeldable = false; // True when touching a weldable surface
    private float nextWeldTime = 0f;

    // Called by the XR Grab Interactable 'Activate' event
    public void StartWelding()
    {
        isWeldingActive = true;
    }

    // Called by the 'Deactivate' event
    public void StopWelding()
    {
        isWeldingActive = false;
    }

    // Update is called every frame
    private void Update()
    {
        // --- NEW: Control Flame ---
        // The flame is active whenever the trigger is held down, regardless of surface contact.
        if (isWeldingActive && !weldingFlame.isPlaying)
        {
            weldingFlame.Play();
        }
        else if (!isWeldingActive && weldingFlame.isPlaying)
        {
            weldingFlame.Stop();
        }

        // This variable determines if sparks and audio should play
        bool shouldBeActive = isWeldingActive && isTouchingWeldable;

        // Control Sparks
        if (shouldBeActive && !weldingSparks.isPlaying)
        {
            weldingSparks.Play();
        }
        else if (!shouldBeActive && weldingSparks.isPlaying)
        {
            weldingSparks.Stop();
        }

        // Control Audio
        if (shouldBeActive && !weldingAudioSource.isPlaying)
        {
            weldingAudioSource.Play();
        }
        else if (!shouldBeActive && weldingAudioSource.isPlaying)
        {
            weldingAudioSource.Stop();
        }
    }

    // Sets our flag when we touch a valid surface
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weldable"))
        {
            isTouchingWeldable = true;
        }
    }

    // Clears our flag when we stop touching
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weldable"))
        {
            isTouchingWeldable = false;
        }
    }

    // This logic for creating blobs remains the same
    private void OnTriggerStay(Collider other)
    {
        // The blob creation is conditional on the trigger being held and contact
        if (isWeldingActive && Time.time >= nextWeldTime && other.CompareTag("Weldable"))
        {
            nextWeldTime = Time.time + 1f / weldRate;

            if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, 1.0f, weldableLayer))
            {
                if (hit.collider == other)
                {
                    // Calculate the new position by pushing the blob slightly into the surface
                    Vector3 spawnPosition = hit.point - (hit.normal * weldOffset);

                    GameObject blob = Instantiate(weldBlobPrefab);
                    blob.transform.SetPositionAndRotation(spawnPosition, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
            }
        }
    }
}