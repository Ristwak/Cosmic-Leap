using UnityEngine;

public class XRGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravityScale = 1f;  // Gravity scale relative to Earth (1 = Earth, 0.38 = Mars, etc.)
    public float gravityStrength = 9.81f;  // Earth's gravity strength (m/s²)
    public float fallSpeed = 0.0f;   // The fall speed based on gravity

    [Header("Gravity Application Settings")]
    public bool applyGravity = true;   // Whether to apply gravity to the XR Rig or not
    public Vector3 gravityDirection = Vector3.down;  // Direction of gravity (usually down)

    private Rigidbody rb;  // Rigidbody of the XR Rig (if using physics)
    private bool isFalling = false;  // Is the player currently falling?
    private bool hasLanded = false;  // Check if the player has already landed

    private void Awake()
    {
        // Check for Rigidbody, if none found we will apply gravity manually
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found! Gravity will be manually applied.");
        }
    }

    private void Update()
    {
        if (applyGravity && !hasLanded)
        {
            ApplyGravity();
        }
    }

    private void ApplyGravity()
    {
        if (rb != null)
        {
            // If a Rigidbody exists, apply gravity via physics
            rb.AddForce(gravityDirection * gravityStrength * gravityScale, ForceMode.Acceleration);
        }
        else
        {
            // If no Rigidbody, apply gravity manually
            fallSpeed += gravityStrength * gravityScale * Time.deltaTime;  // Calculate fall speed based on gravity
            transform.position += gravityDirection * fallSpeed * Time.deltaTime;  // Move the player down
        }

        // Check if the player has landed (on the ground or a platform)
        if (transform.position.y <= 0 && !hasLanded)
        {
            hasLanded = true;
            OnImpact();  // Trigger impact when landing
        }
    }

    // Method to start the falling process (for when the player jumps or falls)
    public void StartFalling()
    {
        isFalling = true;
        hasLanded = false;
        fallSpeed = 0;  // Reset fall speed
    }

    // Method to stop the falling process (if you want to disable gravity temporarily)
    public void StopFalling()
    {
        isFalling = false;
        hasLanded = true;
        fallSpeed = 0;
    }

    // Call this function when the player lands (to handle impact)
    private void OnImpact()
    {
        // You can add haptic feedback, sound, camera shake, or other effects here
        Debug.Log("Player has landed! Apply impact effects like haptics or visual feedback.");

        // Example of simple impact simulation:
        // - Haptic feedback (for VR controllers)
        // - Camera shake (to simulate the landing impact)
        // - Sound effects (if needed)
        ApplyImpactEffects();
    }

    // Method to apply haptic feedback and/or other visual effects on landing
    private void ApplyImpactEffects()
    {
        // Example: Apply haptic feedback on both controllers
        // Assuming you're using Unity's XR Toolkit:
        // XRController controller = ...;  // Reference to your XR controller
        // controller.SendHapticImpulse(0.5f, 0.1f);  // Apply haptic feedback for 0.5 intensity and 0.1 duration

        // Example: You could add a camera shake effect here if you have a script for it
        // Camera.main.GetComponent<CameraShake>().Shake(impactStrength);  // Custom camera shake logic
    }

    // Method to set the gravity scale dynamically (called by PlanetManager)
    public void SetGravityScale(float scale)
    {
        gravityScale = scale;
    }
}
