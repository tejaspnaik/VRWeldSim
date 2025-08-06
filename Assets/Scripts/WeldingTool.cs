using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;

public class WeldingTool : MonoBehaviour
{
    [Header("Welding Settings")]
    public GameObject weldBlobPrefab;
    public float weldRate = 15f;
    public float weldOffset = 0.02f;
    public LayerMask weldableLayer;
    public Transform raycastOrigin;

    [Header("Haptics")]
    [Range(0, 1)]
    public float hapticAmplitude = 0.7f;
    public float hapticDuration = 0.1f;

    [Header("Effects")]
    public AudioSource weldingAudioSource;
    public ParticleSystem weldingSparks;
    public ParticleSystem weldingFlame;

    // --- Private Fields ---
    private bool isWeldingActive = false;
    private bool isTouchingWeldable = false;
    private float nextWeldTime = 0f;
    private List<GameObject> currentWeldBlobs = new List<GameObject>();
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // When the "FinalizeWeld" button is pressed, call our new ClearWeldBlobs function
        inputActions.Player.FinalizeWeld.performed += ctx => ClearWeldBlobs();
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.FinalizeWeld.performed -= ctx => ClearWeldBlobs();
        inputActions.Player.Disable();
    }

    public void StartWelding()
    {
        isWeldingActive = true;
    }

    public void StopWelding()
    {
        isWeldingActive = false;
    }

    private void Update()
    {
        // Control Flame
        if (isWeldingActive && !weldingFlame.isPlaying)
        {
            weldingFlame.Play();
        }
        else if (!isWeldingActive && weldingFlame.isPlaying)
        {
            weldingFlame.Stop();
        }

        bool shouldBeActive = isWeldingActive && isTouchingWeldable;

        // Control Sparks & Audio
        if (shouldBeActive)
        {
            if (weldingSparks != null && !weldingSparks.isPlaying) weldingSparks.Play();
            if (weldingAudioSource != null && !weldingAudioSource.isPlaying) weldingAudioSource.Play();
        }
        else
        {
            if (weldingSparks != null && weldingSparks.isPlaying) weldingSparks.Stop();
            if (weldingAudioSource != null && weldingAudioSource.isPlaying) weldingAudioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weldable"))
        {
            isTouchingWeldable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weldable"))
        {
            isTouchingWeldable = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isWeldingActive && Time.time >= nextWeldTime && other.CompareTag("Weldable"))
        {
            nextWeldTime = Time.time + 1f / weldRate;

            if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, 1.0f, weldableLayer))
            {
                if (hit.collider == other)
                {
                    Vector3 spawnPosition = hit.point - (hit.normal * weldOffset);
                    GameObject blob = Instantiate(weldBlobPrefab);
                    blob.transform.SetPositionAndRotation(spawnPosition, Quaternion.FromToRotation(Vector3.up, hit.normal));

                    currentWeldBlobs.Add(blob);

                    if (GetComponent<XRGrabInteractable>().firstInteractorSelecting is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
                    {
                        controllerInteractor.SendHapticImpulse(hapticAmplitude, hapticDuration);
                    }
                }
            }
        }
    }

    // --- NEW, SIMPLIFIED RESET FUNCTION ---
    public void ClearWeldBlobs()
    {
        if (currentWeldBlobs.Count == 0)
        {
            Debug.Log("No weld blobs to clear.");
            return;
        }

        // Loop through all the tracked blobs and destroy them
        foreach (var blob in currentWeldBlobs)
        {
            Destroy(blob);
        }

        // Clear the list so we can start a new weld seam
        currentWeldBlobs.Clear();

        Debug.Log("Weld blobs cleared. Ready for a new seam.");
    }
}