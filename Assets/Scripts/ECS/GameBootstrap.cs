using Leopotam.Ecs;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private BusinessConfig _businessConfig; // конфигурация бизнеса
    [SerializeField] private BusinessNamesConfig _namesConfig; // конфигурация названий бизнеса
    [SerializeField] private BusinessView _businessViewPrefab; // префаб представления бизнеса
    [SerializeField] private Transform _businessesContainer; // контейнер для бизнеса
    [SerializeField] private BalanceView _balanceView; // представление баланса

    private EcsWorld _world;
    private EcsSystems _systems;

    private void Start()
    {
        ValidateReferences();

        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems
            .Add(new BusinessInitSystem(_businessConfig, _namesConfig))
            .Add(new IncomeProgressSystem(_businessConfig))
            .Add(new LevelUpSystem(_businessConfig))
            .Add(new UpgradeSystem(_businessConfig))
            // .Add(new SaveSystem())
            // .Add(new LoadSystem())
            .Add(new UpdateViewSystem())
            .OneFrame<UpdateViewEvent>()
            .OneFrame<LevelUpRequest>()
            .OneFrame<UpgradeRequest>()
            .Inject(_businessConfig)
            .Inject(_namesConfig);

        _systems.Init();

        InitializeUI();
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
    }

    private void InitializeUI()
    {
        if (!_world.IsAlive()) return;

        // инициализация представления баланса
        var balanceFilter = _world.GetFilter(typeof(EcsFilter<Balance>));
        if (!balanceFilter.IsEmpty())
        {
            _balanceView.Initialize(_world, balanceFilter.GetEntity(0));
        }

        // инициализация представления бизнеса
        var businessFilter = _world.GetFilter(typeof(EcsFilter<Business>));
        foreach (var i in businessFilter)
        {
            var entity = businessFilter.GetEntity(i);
            var view = Instantiate(_businessViewPrefab, _businessesContainer);
            view.Initialize(_world, entity, _businessConfig, _namesConfig);
        }
    }

    private void Update()
    {
        _systems?.Run();
    }

    // private void OnApplicationQuit()
    // {
    //     _systems?.Get1<SaveSystem>()?.Save();
    // }

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