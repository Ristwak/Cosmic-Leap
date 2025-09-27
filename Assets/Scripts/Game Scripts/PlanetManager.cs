using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // for scene management

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { get; private set; }

    [Header("Planets")]
    public PlanetData[] planets;

    [Header("Runtime")]
    public GameObject xrRig;  // XR Rig for teleportation and gravity setup
    public AudioSource ambientAudioSource; // AudioSource to play ambient sounds and planet descriptions

    [Header("Settings")]
    public float earthGravity = 9.81f;
    public float gravityLerpDuration = 0.5f; // smooth transition duration for Physics.gravity

    string currentSceneName;  // To track the current planet's scene
    Coroutine gravityCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Select planet by index (from the UI)
    public void SelectPlanetByIndex(int index)
    {
        if (index < 0 || index >= planets.Length) return;
        SelectPlanet(planets[index]);
    }

    // Select planet and load its scene
    public void SelectPlanet(PlanetData planet)
    {
        if (planet == null) return;

        // Load the appropriate scene for the selected planet
        LoadPlanetScene(planet);

        // Set gravity smoothly for the selected planet
        Vector3 targetGravity = Vector3.down * earthGravity * planet.gravityScale;
        if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
        gravityCoroutine = StartCoroutine(LerpSetPhysicsGravity(targetGravity, gravityLerpDuration));

        // Set skybox and ambient audio for the planet
        if (planet.skyboxMaterial != null) RenderSettings.skybox = planet.skyboxMaterial;
        if (ambientAudioSource != null)
        {
            ambientAudioSource.clip = planet.ambientAudio;
            if (planet.ambientAudio != null) 
            {
                ambientAudioSource.Play();  // Play the planet-specific audio when scene is loaded
            }
            else 
            {
                ambientAudioSource.Stop();
            }
        }

        // Move XR Rig to spawn point (if defined in the scene)
        if (xrRig != null && planet.playerSpawnPoint != null)
        {
            xrRig.transform.position = planet.playerSpawnPoint.position;
            xrRig.transform.rotation = planet.playerSpawnPoint.rotation;
        }

        // Optional: Show a UI or audio cue to indicate the planet was selected and loading
    }

    // Load planet scene asynchronously
    void LoadPlanetScene(PlanetData planet)
    {
        if (currentSceneName != planet.sceneName)
        {
            // Unload the current scene if one is loaded
            if (!string.IsNullOrEmpty(currentSceneName))
            {
                SceneManager.UnloadSceneAsync(currentSceneName);
            }

            // Load the selected planet's scene asynchronously
            SceneManager.LoadSceneAsync(planet.sceneName, LoadSceneMode.Additive);

            // Update current scene name to the newly loaded scene
            currentSceneName = planet.sceneName;
        }
    }

    // Smoothly interpolate gravity over time
    IEnumerator LerpSetPhysicsGravity(Vector3 target, float duration)
    {
        Vector3 start = Physics.gravity;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Physics.gravity = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        Physics.gravity = target;
    }

    // Quick helper to get planet by name (optional)
    public PlanetData GetPlanetByName(string name)
    {
        foreach (var p in planets)
            if (p.planetName == name) return p;
        return null;
    }
}
