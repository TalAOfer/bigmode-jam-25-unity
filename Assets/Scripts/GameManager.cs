using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core References")]
    public Player player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mainMenu;
    public GalaxyManager galaxyManager;
    public AudioController audioController;
    public ScreenFlash screenFlash;
    public BackgroundManager backgroundManager;
    public PlanetDirectionIndicatorManager planetDirectionIndicatorManager;

    [Header("Game Settings")]
    [SerializeField] private float worldBoundary = 250f;

    private bool isGamePaused;
    private bool isInitialized;

    private void Awake()
    {
        SetupSingleton();
        InitializeGameState();
    }

    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameState()
    {
        galaxyManager.Initialize();
        InitializePlayer();
        backgroundManager.GoToPlanetSkybox();
        isInitialized = true;
        StartCoroutine(PlayMusic());
    }

    public IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(1f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Music/Music");
    }

    private void InitializePlayer()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in GameManager!");
            return;
        }

        player.planetIdx = 0;
        player.GetClosestPlanet(galaxyManager.planets);
    }

    private void Update()
    {
        if (!isInitialized) return;
 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (!isGamePaused)
        {
            UpdateGameState();
        }
    }

    private void UpdateGameState()
    {
        UpdatePlayerState();

        Vector2 prev = player.currentPlanet.transform.position;

        galaxyManager.UpdatePlanets(Time.deltaTime);

        Vector2 curr = player.currentPlanet.transform.position;

        player.transform.position += (Vector3)(curr - prev);

    }

    private void UpdatePlayerState()
    {
        if (player == null) return;

        player.GetClosestPlanet(galaxyManager.planets);
        WrapPlayerPosition();
        player.UpdatePlayer();
    }

    private void WrapPlayerPosition()
    {
        Vector2 newPosition = (Vector2)player.transform.position;
        bool positionChanged = false;

        if (player.transform.position.x < -worldBoundary)
        {
            newPosition.x = worldBoundary - 3f;
            positionChanged = true;
        }
        else if (player.transform.position.x > worldBoundary)
        {
            newPosition.x = -worldBoundary + 3f;
            positionChanged = true;
        }

        if (player.transform.position.y < -worldBoundary)
        {
            newPosition.y = worldBoundary - 3f;
            positionChanged = true;
        }
        else if (player.transform.position.y > worldBoundary)
        {
            newPosition.y = -worldBoundary + 3f;
            positionChanged = true;
        }

        if (positionChanged)
        {
            player.transform.position = newPosition;
        }
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        mainMenu.SetActive(isGamePaused);
        Time.timeScale = isGamePaused ? 0 : 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(worldBoundary * 2, worldBoundary * 2, 0);
        Gizmos.DrawWireCube(center, size);
    }
}