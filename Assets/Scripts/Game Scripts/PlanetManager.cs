using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { get; private set; }

    [Header("Planets")]
    public PlanetData[] planets;

    [Header("Runtime")]
    public Transform environmentParent; // where landscape prefabs will be instantiated
    public GameObject xrRig; // assign your XR Rig root (so we can reposition it)
    public AudioSource ambientAudioSource; // audio source used for ambient ambience

    [Header("Settings")]
    public float earthGravity = 9.81f;
    public float gravityLerpDuration = 0.5f; // smooth transition duration for Physics.gravity

    GameObject currentEnvironmentInstance;
    Coroutine gravityCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectPlanetByIndex(int index)
    {
        if (index < 0 || index >= planets.Length) return;
        SelectPlanet(planets[index]);
    }

    public void SelectPlanet(PlanetData planet)
    {
        if (planet == null) return;

        // Load environment prefab
        LoadEnvironment(planet);

        // Set gravity smoothly
        Vector3 targetGravity = Vector3.down * earthGravity * planet.gravityScale;
        if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
        gravityCoroutine = StartCoroutine(LerpSetPhysicsGravity(targetGravity, gravityLerpDuration));

        // Set skybox
        if (planet.skyboxMaterial != null) RenderSettings.skybox = planet.skyboxMaterial;

        // Play ambient audio
        if (ambientAudioSource != null)
        {
            ambientAudioSource.clip = planet.ambientAudio;
            if (planet.ambientAudio != null) ambientAudioSource.Play();
            else ambientAudioSource.Stop();
        }

        // Move XR Rig to spawn point (if provided in prefab)
        if (xrRig != null && currentEnvironmentInstance != null)
        {
            // if planet defines a playerSpawnPoint, use it
            if (planet.playerSpawnPoint != null)
            {
                xrRig.transform.position = planet.playerSpawnPoint.position;
                xrRig.transform.rotation = planet.playerSpawnPoint.rotation;
            }
            else
            {
                // fallback: try to find a child named "PlayerSpawn" in instantiated prefab
                Transform spawn = currentEnvironmentInstance.transform.Find("PlayerSpawn");
                if (spawn != null)
                {
                    xrRig.transform.position = spawn.position;
                    xrRig.transform.rotation = spawn.rotation;
                }
                // else keep player where they are (hub -> they should be teleported manually)
            }
        }
    }

    void LoadEnvironment(PlanetData planet)
    {
        // destroy previous instance
        if (currentEnvironmentInstance != null) Destroy(currentEnvironmentInstance);

        // instantiate new
        if (planet.landscapePrefab != null && environmentParent != null)
        {
            currentEnvironmentInstance = Instantiate(planet.landscapePrefab, environmentParent);
            currentEnvironmentInstance.name = planet.planetName + "_Environment";
        }
        else if (planet.landscapePrefab != null)
        {
            currentEnvironmentInstance = Instantiate(planet.landscapePrefab);
            currentEnvironmentInstance.name = planet.planetName + "_Environment";
        }
    }

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
