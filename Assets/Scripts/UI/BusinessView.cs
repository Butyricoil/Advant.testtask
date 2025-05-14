using Leopotam.Ecs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BusinessView : MonoBehaviour, IDisposable
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _nameBusinessText;    // название бизнеса
    [SerializeField] private Image _progressBusinessImage;  // прогресс бар
    [SerializeField] private TMP_Text _levelBusinessText; // уровень бизнеса
    [SerializeField] private TMP_Text _incomeBusinessText;  // доход бизнеса
    [SerializeField] private Button _levelUpBusinessButton; // кнопка повышения уровня
    [SerializeField] private TMP_Text _levelUpBusinessPriceText;   // цена повышения уровня
    [SerializeField] private Button _upgradeBusiness1Button; // кнопка покупки первого улучшения
    [SerializeField] private TMP_Text _upgradeBusiness1Text; // текст первого улучшения
    [SerializeField] private Button _upgradeBusiness2Button; // кнопка покупки второго улучшения
    [SerializeField] private TMP_Text _upgradeBusiness2Text; // текст второго улучшения

    private EcsWorld _world; // ссылка на мир ECS
    private EcsEntity _entity; // ссылка на сущность бизнеса
    private BusinessConfig _config; // ссылка на конфигурацию бизнеса
    private BusinessNamesConfig _namesConfig; // ссылка на конфигурацию названий бизнеса

    private void ValidateReferences() // проверка объектов со сцены
    {
        if (_nameBusinessText == null) Debug.LogError("Name text reference is missing!");
        if (_progressBusinessImage == null) Debug.LogError("Progress image reference is missing!");
        if (_levelBusinessText == null) Debug.LogError("Level text reference is missing!");
        if (_incomeBusinessText == null) Debug.LogError("Income text reference is missing!");
        if (_levelUpBusinessButton == null) Debug.LogError("Level up button reference is missing!");
        if (_levelUpBusinessPriceText == null) Debug.LogError("Level up price text reference is missing!");
        if (_upgradeBusiness1Button == null) Debug.LogError("Upgrade 1 button reference is missing!");
        if (_upgradeBusiness1Text == null) Debug.LogError("Upgrade 1 text reference is missing!");
        if (_upgradeBusiness2Button == null) Debug.LogError("Upgrade 2 button reference is missing!");
        if (_upgradeBusiness2Text == null) Debug.LogError("Upgrade 2 text reference is missing!");
    }

    public void Initialize(EcsWorld world, EcsEntity entity, BusinessConfig config, BusinessNamesConfig namesConfig)
    {
        ValidateReferences();

        _world = world;
        _entity = entity;
        _config = config;
        _namesConfig = namesConfig;

        if (_world == null || !_entity.IsAlive() || _config == null || _namesConfig == null)
        {
            Debug.LogError("Failed to initialize BusinessView: Invalid references");
            return;
        }

        ref var business = ref _entity.Get<Business>();

        _nameBusinessText.text = _namesConfig.BusinessNames[business.Id];
        _upgradeBusiness1Text.text = $"{_namesConfig.Upgrade1Names[business.Id]}\n(${_config.Upgrade1Prices[business.Id]})";
        _upgradeBusiness2Text.text = $"{_namesConfig.Upgrade2Names[business.Id]}\n(${_config.Upgrade2Prices[business.Id]})";

        _levelUpBusinessButton.onClick.AddListener(OnLevelUpClicked);
        _upgradeBusiness1Button.onClick.AddListener(() => OnUpgradeClicked(1));
        _upgradeBusiness2Button.onClick.AddListener(() => OnUpgradeClicked(2));

        UpdateView();
    }

    private void Update()
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return; // проверяем, что мир и сущность живы

        if (_entity.Has<IncomeProgress>())
        {
            float progress = _entity.Get<IncomeProgress>().Value;
            _progressBusinessImage.fillAmount = progress; // Обновляем fillAmount в зависимости от прогресса
        }
    }

    public void UpdateView()
    {
        if (!_entity.IsAlive()) return;

        ref var business = ref _entity.Get<Business>();

        _levelBusinessText.text = $"LVL {business.Level}";
        _incomeBusinessText.text = $"Income: ${BusinessUtils.CalculateIncome(business, _config)}";

        int levelUpPrice = (business.Level + 1) * _config.BaseCosts[business.Id];
        _levelUpBusinessPriceText.text = $"LVL UP\n${levelUpPrice}";

        UpdateUpgradeButtons(business);
    }

    private void UpdateUpgradeButtons(Business business)
    {
        if (business.Upgrade1Purchased) // проверяем, куплено ли первое улучшение
        {
            _upgradeBusiness1Text.text = "Purchased";
            _upgradeBusiness1Button.interactable = false;
        }

        if (business.Upgrade2Purchased) // проверяем, куплено ли второе улучшение
        {
            _upgradeBusiness2Text.text = "Purchased";
            _upgradeBusiness2Button.interactable = false;
        }
    }

    private void OnLevelUpClicked() // кнопка покупки Lеvеlup
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return;

        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<LevelUpRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
    }

    private void OnUpgradeClicked(int upgradeId) // кнопка покупки улучшения
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return;

        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<UpgradeRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
        request.UpgradeId = upgradeId;
    }

    public void Dispose() // отписка от кнопки
    {
        if (_levelUpBusinessButton != null)
            _levelUpBusinessButton.onClick.RemoveListener(OnLevelUpClicked);
        if (_upgradeBusiness1Button != null)
            _upgradeBusiness1Button.onClick.RemoveListener(() => OnUpgradeClicked(1));
        if (_upgradeBusiness2Button != null)
            _upgradeBusiness2Button.onClick.RemoveListener(() => OnUpgradeClicked(2));
    }

    private void OnDestroy()
    {
        Dispose();
    }
}