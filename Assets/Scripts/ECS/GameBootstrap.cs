using Leopotam.Ecs;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private BusinessConfig _businessConfig; // конфигурация бизнеса
    [SerializeField] private BusinessNamesConfig _namesConfig; // конфигурация названий бизнеса
    [SerializeField] private BusinessView _businessViewPrefab; // префаб представления бизнеса
    [SerializeField] private Transform _businessesContainer; // контейнер для бизнеса
    [SerializeField] private BalanceView _balanceView; // представление баланса
    [SerializeField] private SaveButtonView _saveButtonView; // представление кнопки сохранения

    private EcsWorld _world;
    private EcsSystems _systems;

    private void Start()
    {
        ValidateReferences();

        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        // Сначала инициализируем базовые сущности
        _systems
            .Add(new BusinessInitSystem(_businessConfig, _namesConfig))
            // Затем загружаем сохранение
            .Add(new LoadSystem())
            // Затем добавляем остальные системы
            .Add(new IncomeProgressSystem(_businessConfig))
            .Add(new LevelUpSystem(_businessConfig))
            .Add(new UpgradeSystem(_businessConfig))
            .Add(new UpdateViewSystem())
            .Add(new SaveSystem())
            .OneFrame<UpdateViewEvent>()
            .OneFrame<LevelUpRequest>()
            .OneFrame<UpgradeRequest>()
            .OneFrame<SaveEvent>()
            .Inject(_businessConfig)
            .Inject(_namesConfig);

        _systems.Init();
        Debug.Log("Системы инициализированы");

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
        if (_saveButtonView == null)
            Debug.LogError("SaveButtonView reference is missing!");
    }

    private void InitializeUI()
    {
        if (!_world.IsAlive()) return;

        // инициализация представления баланса
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

        // инициализация представления бизнеса
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

        // инициализация кнопки сохранения
        _saveButtonView.Initialize(_world);
        Debug.Log("Кнопка сохранения инициализирована");
    }

    private void Update()
    {
        _systems?.Run();
    }

    private void OnApplicationQuit()
    {
        if (_world != null && _world.IsAlive())
        {
            _world.NewEntity().Get<SaveEvent>();
            // даем системам шанс обработать событие сохранения
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