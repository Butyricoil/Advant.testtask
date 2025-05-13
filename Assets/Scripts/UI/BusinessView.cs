using Leopotam.Ecs;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BusinessView : MonoBehaviour
{
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

    public void Initialize(EcsWorld world, EcsEntity entity, BusinessConfig config, BusinessNamesConfig namesConfig)
    {
        _world = world;
        _entity = entity;
        _config = config;
        _namesConfig = namesConfig;

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

    private void UpdateView()
    {
        if (!_entity.IsAlive()) return;

        ref var business = ref _entity.Get<Business>();

        _levelBusinessText.text = $"LVL {business.Level}";
        _incomeBusinessText.text = $"Доход: {CalculateIncome(business)}";

        int levelUpPrice = (business.Level + 1) * _config.BaseCosts[business.Id];
        _levelUpBusinessPriceText.text = $"LVL UP\n${levelUpPrice}";

        if (business.Upgrade1Purchased) // проверяем, куплено ли первое улучшение
        {
            _upgradeBusiness1Text.text = "Куплено";
            _upgradeBusiness1Button.interactable = false;
        }

        if (business.Upgrade2Purchased) // проверяем, куплено ли второе улучшение
        {
            _upgradeBusiness2Text.text = "Куплено";
            _upgradeBusiness2Button.interactable = false;
        }
    }

    private int CalculateIncome(Business business) // расчет дохода
    {
        float multiplier = 1f;
        if (business.Upgrade1Purchased) multiplier += _config.Upgrade1Multipliers[business.Id];
        if (business.Upgrade2Purchased) multiplier += _config.Upgrade2Multipliers[business.Id];

        return (int)(business.Level * _config.BaseIncomes[business.Id] * multiplier);
    }

    private void OnLevelUpClicked() // кнопка покупки Lеvеlup
    {
        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<LevelUpRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
    }

    private void OnUpgradeClicked(int upgradeId) // кнопка покупки улучшения
    {
        var requestEntity = _world.NewEntity();
        ref var request = ref requestEntity.Get<UpgradeRequest>();
        request.BusinessId = _entity.Get<Business>().Id;
        request.UpgradeId = upgradeId;
    }
}