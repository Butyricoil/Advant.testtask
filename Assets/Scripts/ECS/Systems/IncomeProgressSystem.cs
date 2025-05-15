using Leopotam.Ecs;
using UnityEngine;

public class IncomeProgressSystem : IEcsRunSystem
{
    private readonly BusinessConfig _config;

    private EcsWorld _world;
    private EcsFilter<Business, IncomeProgress> _filter;
    private EcsFilter<Balance> _balanceFilter;
    private EcsFilter<Paused> _pausedFilter;

    public IncomeProgressSystem(BusinessConfig config)
    {
        _config = config;
    }

    public void Run()
    {
        if (!_pausedFilter.IsEmpty())
            return;

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
                int income = BusinessUtils.CalculateIncome(business, _config);
                balance.Value += income;
                progress.TimePassed = 0;
                progress.Value = 0;
            }
        }
    }
}