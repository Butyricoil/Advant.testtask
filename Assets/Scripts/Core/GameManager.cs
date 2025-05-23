using Leopotam.Ecs;
using UnityEngine;
using UI.PauseMenu;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Business Configuration")]
    [SerializeField] private BusinessConfig _businessConfig;
    [SerializeField] private BusinessNamesConfig _namesConfig;
    
    [Header("UI References")]
    [SerializeField] private BusinessView _businessViewPrefab;
    [SerializeField] private Transform _businessesContainer;
    [SerializeField] private BalanceView _balanceView;
    [SerializeField] private PauseMenuView _pauseMenuView;

    private EcsWorld _world;
    private EcsSystems _systems;

    private float _autosaveInterval = 120f;
    private float _autosaveTimer = 0f;

    private void Start()
    {
        ValidateReferences();

        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        InitializeSystems();
        _systems.Init();
        Debug.Log("Системы инициализированы");

        InitializeUI();
    }

    private void InitializeSystems()
    {
        _systems
            .Add(new BusinessInitSystem(_businessConfig, _namesConfig))
            .Add(new LoadSystem())
            .Add(new IncomeProgressSystem(_businessConfig))
            .Add(new LevelUpSystem(_businessConfig))
            .Add(new UpgradeSystem(_businessConfig))
            .Add(new UpdateViewSystem())
            .Add(new SaveSystem())
            .Add(new DropSaveSystem())
            .Add(new PauseSystem())
            .OneFrame<UpdateViewEvent>()
            .OneFrame<LevelUpRequest>()
            .OneFrame<UpgradeRequest>()
            .OneFrame<SaveEvent>()
            .OneFrame<DropSaveEvent>()
            .Inject(_businessConfig)
            .Inject(_namesConfig);
    }

    private void ValidateReferences()
    {
        if (_businessConfig == null)
            Debug.LogError("BusinessConfig reference is missing!");
        if (_namesConfig == null)
            Debug.LogError("BusinessNamesConfig reference is missing!");
        if (_businessViewPrefab == null)
            Debug.LogError("BusinessView prefab reference is missing!");
        if (_businessesContainer == null)
            Debug.LogError("Businesses container reference is missing!");
        if (_balanceView == null)
            Debug.LogError("BalanceView reference is missing!");
        if (_pauseMenuView == null)
            Debug.LogError("PauseMenuView reference is missing!");
    }

    private void InitializeUI()
    {
        if (!_world.IsAlive()) return;

        _pauseMenuView.Initialize(_world);
        InitializeBalanceView();
        InitializeBusinessViews();
    }

    private void InitializeBalanceView()
    {
        var balanceFilter = _world.GetFilter(typeof(EcsFilter<Balance>));
        if (!balanceFilter.IsEmpty())
        {
            _balanceView.Initialize(_world, balanceFilter.GetEntity(0));
            Debug.Log("Представление баланса инициализировано");
        }
        else
        {
            Debug.LogError("Не найдена сущность баланса!");
        }
    }

    private void InitializeBusinessViews()
    {
        var businessFilter = _world.GetFilter(typeof(EcsFilter<Business>));
        if (!businessFilter.IsEmpty())
        {
            foreach (var i in businessFilter)
            {
                var entity = businessFilter.GetEntity(i);
                var view = Instantiate(_businessViewPrefab, _businessesContainer);
                view.Initialize(_world, entity, _businessConfig, _namesConfig);
            }
            Debug.Log($"Инициализировано {businessFilter.GetEntitiesCount()} бизнесов");
        }
        else
        {
            Debug.LogError("Не найдены сущности бизнесов!");
        }
    }


    private void Update()
    {
        _systems?.Run();

        // Автосейв
        if (_world != null && _world.IsAlive())
        {
            _autosaveTimer += Time.deltaTime;
            if (_autosaveTimer >= _autosaveInterval)
            {
                _world.NewEntity().Get<SaveEvent>();
                _autosaveTimer = 0f;
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && _world != null && _world.IsAlive())
        {
            _world.NewEntity().Get<SaveEvent>();
            _systems?.Run();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && _world != null && _world.IsAlive())
        {
            _world.NewEntity().Get<SaveEvent>();
            _systems?.Run();
        }
    }

    private void OnDestroy()
    {
        if (_systems != null)
        {
            _systems.Destroy();
            _systems = null;
            _world.Destroy();
            _world = null;
        }
    }
}