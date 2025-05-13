using Leopotam.Ecs;
using UnityEngine;

public class IncomeProgressSystem : IEcsRunSystem
{
    private readonly BusinessConfig _config;

    private EcsWorld _world;
    private EcsFilter<Business, IncomeProgress> _filter;
    private EcsFilter<Balance> _balanceFilter;

    public IncomeProgressSystem(BusinessConfig config)
    {
        _config = config;
    }

    public void Run()
    {
        ref var balance = ref _balanceFilter.Get1(0);

        foreach (var i in _filter)
        {
            ref var business = ref _filter.Get1(i);
            ref var progress = ref _filter.Get2(i);

            if (business.Level == 0) continue;

            float incomeDelay = _config.IncomeDelays[business.Id];
            progress.TimePassed += Time.deltaTime;
            progress.Value = Mathf.Clamp01(progress.TimePassed / incomeDelay);

            if (progress.TimePassed >= incomeDelay)
            {
                int income = CalculateIncome(business, _config);
                balance.Value += income;
                progress.TimePassed = 0;
                progress.Value = 0;
            }
        }
    }

    private int CalculateIncome(Business business, BusinessConfig config)
    {
        float multiplier = 1f;
        if (business.Upgrade1Purchased) multiplier += config.Upgrade1Multipliers[business.Id];
        if (business.Upgrade2Purchased) multiplier += config.Upgrade2Multipliers[business.Id];

        return (int)(business.Level * config.BaseIncomes[business.Id] * multiplier);
    }
}