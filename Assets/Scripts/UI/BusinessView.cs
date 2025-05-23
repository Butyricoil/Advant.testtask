using Leopotam.Ecs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class BusinessView : MonoBehaviour, IDisposable
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _nameBusinessText;
    [SerializeField] private Image _progressBusinessImage;
    [SerializeField] private TMP_Text _levelBusinessText;
    [SerializeField] private TMP_Text _incomeBusinessText;
    [SerializeField] private Button _levelUpBusinessButton;
    [SerializeField] private TMP_Text _levelUpBusinessPriceText;
    [SerializeField] private Button _upgradeBusiness1Button;
    [SerializeField] private TMP_Text _upgradeBusiness1Text;
    [SerializeField] private Button _upgradeBusiness2Button;
    [SerializeField] private TMP_Text _upgradeBusiness2Text;

    private EcsWorld _world;
    private EcsEntity _entity;
    private BusinessConfig _config;
    private BusinessNamesConfig _namesConfig;
    private UnityAction _upgrade1Action;
    private UnityAction _upgrade2Action;
    private EcsFilter<UpdateViewComponent> _updateViewFilter;

    public void Initialize(EcsWorld world, EcsEntity entity, BusinessConfig config, BusinessNamesConfig namesConfig)
    {
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
        
        _upgrade1Action = () => OnUpgradeClicked(1);
        _upgrade2Action = () => OnUpgradeClicked(2);
        
        _upgradeBusiness1Button.onClick.AddListener(_upgrade1Action);
        _upgradeBusiness2Button.onClick.AddListener(_upgrade2Action);

        // Регистрируем view в системе обновлния
        _updateViewFilter = _world.GetFilter<UpdateViewComponent>();
        if (_updateViewFilter.GetEntitiesCount() > 0)
        {
            ref var updateViewComponent = ref _updateViewFilter.Get1(0);
            updateViewComponent.RegisterBusinessView(this);
        }
    }

    private void Update()
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return;

        if (_entity.Has<IncomeProgress>())
        {
            float progress = _entity.Get<IncomeProgress>().Value;
            _progressBusinessImage.fillAmount = progress;
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
        if (business.Upgrade1Purchased)
        {
            _upgradeBusiness1Text.text = "Purchased";
            _upgradeBusiness1Button.interactable = false;
        }

        if (business.Upgrade2Purchased)
        {
            _upgradeBusiness2Text.text = "Purchased";
            _upgradeBusiness2Button.interactable = false;
        }
    }

    private void OnLevelUpClicked()
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return;

        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<LevelUpRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
    }

    private void OnUpgradeClicked(int upgradeId)
    {
        if (!_world.IsAlive() || !_entity.IsAlive()) return;

        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<UpgradeRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
        request.UpgradeId = upgradeId;
    }

    public void Dispose()
    {
        if (_levelUpBusinessButton != null)
            _levelUpBusinessButton.onClick.RemoveListener(OnLevelUpClicked);
        if (_upgradeBusiness1Button != null)
            _upgradeBusiness1Button.onClick.RemoveListener(_upgrade1Action);
        if (_upgradeBusiness2Button != null)
            _upgradeBusiness2Button.onClick.RemoveListener(_upgrade2Action);

        // Отписываемся от системы обновления
        if (_world != null && _world.IsAlive() && _updateViewFilter != null)
        {
            if (_updateViewFilter.GetEntitiesCount() > 0)
            {
                ref var updateViewComponent = ref _updateViewFilter.Get1(0);
                updateViewComponent.UnregisterBusinessView(this);
            }
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }
}