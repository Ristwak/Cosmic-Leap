using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "CosmicLeap/PlanetData", order = 0)]
public class PlanetData : ScriptableObject
{
    [Header("Identity")]
    public string planetName = "New Planet";  // Name of the planet (used to match the scene name)
    [TextArea] public string shortDescription; // Optional description

    [Header("Physics")]
    [Tooltip("Gravity multiplier relative to Earth (1 = Earth, 0.16 = Moon)")]
    public float gravityScale = 1f; // Gravity scaling for the planet (relative to Earth's gravity)

    [Header("Environment")]
    public Material skyboxMaterial;  // Optional skybox for the planet
    public AudioClip ambientAudio;   // Optional ambient sound for the planet (planet description audio)

    [Header("Gameplay")]
    public Transform playerSpawnPoint; // Optional spawn point for the player (if provided in the scene)
    public Sprite uiIcon;             // Icon for the planet on the UI button
    public Color uiAccent = Color.white; // Accent color for the planet's UI button

    [Header("Scene Info")]
    public string sceneName;         // The name of the scene to load for the planet (e.g., "Earth", "Mars")
}
