using UnityEngine;
using System.Collections;

public class RugTeleporter : MonoBehaviour
{
    // Public variable to link the destination rug in the Inspector.
    public Transform destination;

    // A static (shared) variable to prevent instant re-teleporting.
    private static bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name + " with tag: " + other.tag);
        // Check if the object that entered is the player and if we are not already teleporting.
        if (other.CompareTag("Player") && !isTeleporting)
        {
            // Find the root of the player object (the XR Origin).
            Transform playerRoot = other.transform.root;

            // --- Optional: For Character Controllers ---
            // If your XR Origin has a CharacterController, you should disable it before teleporting.
            CharacterController controller = playerRoot.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
            // -----------------------------------------

            // Move the entire player rig to the destination's position.
            playerRoot.position = destination.position;
            // Optional: Match the destination's rotation as well.
            // playerRoot.rotation = destination.rotation; 

            // --- Re-enable the Character Controller ---
            if (controller != null)
            {
                controller.enabled = true;
            }
            // -----------------------------------------

            // Start a cooldown to prevent an infinite teleport loop.
            StartCoroutine(TeleportCooldown());
        }
    }

    private IEnumerator TeleportCooldown()
    {
        // Set the flag to true for all instances of this script.
        isTeleporting = true;
        // Wait for a short duration.
        yield return new WaitForSeconds(1.5f);
        // Reset the flag.
        isTeleporting = false;
    }
}