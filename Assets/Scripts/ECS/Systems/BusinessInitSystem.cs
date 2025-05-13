using Leopotam.Ecs;

public class BusinessInitSystem : IEcsInitSystem
{
    private readonly BusinessConfig _businessConfig;
    private readonly BusinessNamesConfig _namesConfig;

    private EcsWorld _world;

    public BusinessInitSystem(BusinessConfig businessConfig, BusinessNamesConfig namesConfig)
    {
        _businessConfig = businessConfig;
        _namesConfig = namesConfig;
    }

    public void Init()
    {
        // Initial balance (если не загрузилось из сохранения)
        if (_world.GetFilter(typeof(EcsFilter<Balance>)).IsEmpty())
        {
            var balanceEntity = _world.NewEntity();
            balanceEntity.Get<Balance>().Value = 150;
        }

        // Initialize businesses (если не загрузились из сохранения)
        var businessFilter = _world.GetFilter(typeof(EcsFilter<Business>));
        if (businessFilter.IsEmpty())
        {
            for (int i = 0; i < _businessConfig.IncomeDelays.Length; i++)
            {
                var businessEntity = _world.NewEntity();
                ref var business = ref businessEntity.Get<Business>();
                business.Id = i;
                business.Level = i == 0 ? 1 : 0;

                ref var progress = ref businessEntity.Get<IncomeProgress>();
                progress.Value = 0;
                progress.TimePassed = 0;
            }
        }
    }
}