using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "CosmicLeap/PlanetData", order = 0)]
public class PlanetData : ScriptableObject
{
    [Header("Identity")]
    public string planetName = "New Planet";
    [TextArea] public string shortDescription;

    [Header("Physics")]
    [Tooltip("Gravity multiplier relative to Earth (1 = Earth, 0.16 = Moon)")]
    public float gravityScale = 1f; // multiply with 9.81

    [Header("Environment")]
    public GameObject landscapePrefab;   // prefab containing environment (cliff, ground, scenery)
    public Material skyboxMaterial;      // optional skybox
    public AudioClip ambientAudio;       // planet ambient sound

    [Header("Gameplay")]
    public Transform playerSpawnPoint;   // optional: local transform inside landscapePrefab (can be null)
    public Sprite uiIcon;                // show on UI button
    public Color uiAccent = Color.white; // optional color for UI
}
