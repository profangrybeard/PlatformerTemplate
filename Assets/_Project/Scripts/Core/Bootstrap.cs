using UnityEngine;
using UnityEngine.SceneManagement;
using PrecisionPlatformer.Combat.Services;
using PrecisionPlatformer.Data.Combat;

namespace PrecisionPlatformer.Core
{
    /// <summary>
    /// Bootstrap script that initializes core services on game start.
    /// Attach this to a GameObject in the BootstrapScene.
    /// Students will add their own service registrations here as they build systems.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        [Header("Scene Management")]
        [Tooltip("Scene to load after initialization (typically DevPlayground)")]
        [SerializeField] private string sceneToLoad = "DevPlayground";

        [Header("Combat Configuration")]
        [Tooltip("Global combat settings (create via Assets > Create > Platformer > Config > Combat)")]
        [SerializeField] private CombatConfig combatConfig;

        // Combat service instance for cleanup
        private CombatService combatService;

        private void Awake()
        {
            Debug.Log("=== Bootstrap: Initializing Core Services ===");

            // Initialize core systems here
            InitializeServices();

            Debug.Log("=== Bootstrap: Core Services Initialized ===");
        }

        private void Start()
        {
            // Load the main playground scene
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log($"Bootstrap: Loading scene '{sceneToLoad}'");
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            }
        }

        private void OnDestroy()
        {
            // Clean up services and event subscriptions
            Debug.Log("=== Bootstrap: Cleaning up services ===");

            // Clean up combat service subscriptions
            combatService?.Cleanup();

            EventBus.ClearAll();
            ServiceLocator.ClearAll();
        }

        /// <summary>
        /// Initialize and register all game services.
        /// Students will add their service registrations here as they implement them.
        /// </summary>
        private void InitializeServices()
        {
            // Example service registration pattern (students will add their own):
            // var inputHandler = new InputHandler();
            // ServiceLocator.Register<InputHandler>(inputHandler);

            // var groundDetection = new GroundDetectionService();
            // ServiceLocator.Register<GroundDetectionService>(groundDetection);

            // var physicsService = new PhysicsService();
            // ServiceLocator.Register<PhysicsService>(physicsService);

            // Combat Service (Combat Track)
            // Uncomment below when starting the combat track
            // if (combatConfig != null)
            // {
            //     combatService = new CombatService(combatConfig);
            //     ServiceLocator.Register<CombatService>(combatService);
            //     Debug.Log("Bootstrap: CombatService registered");
            // }

            Debug.Log("Bootstrap: Service registration complete (no services registered yet - students will add these)");
        }

        /// <summary>
        /// Called when application quits to ensure clean shutdown.
        /// </summary>
        private void OnApplicationQuit()
        {
            EventBus.ClearAll();
            ServiceLocator.ClearAll();
        }
    }
}
