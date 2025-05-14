using Leopotam.Ecs;
using System.IO;
using UnityEngine;

public class BusinessInitSystem : IEcsInitSystem
{
    private readonly BusinessConfig _businessConfig;
    private readonly BusinessNamesConfig _namesConfig;

    private EcsWorld _world;
    private EcsFilter<BusinessInitialized> _initFilter;

    public BusinessInitSystem(BusinessConfig businessConfig, BusinessNamesConfig namesConfig)
    {
        _businessConfig = businessConfig;
        _namesConfig = namesConfig;
    }

    public void Init()
    {
        // проверяем, не инициализированы ли уже бизнесы
        if (!_initFilter.IsEmpty())
        {
            return;
        }

        // инициализация баланса (если не загрузился из сохранения)
        if (_world.GetFilter(typeof(EcsFilter<Balance>)).IsEmpty())
        {
            var balanceEntity = _world.NewEntity();
            balanceEntity.Get<Balance>().Value = 150;
        }

        // инициализация бизнеса (если не загрузились из сохранения)
        var businessFilter = _world.GetFilter(typeof(EcsFilter<Business>));
        if (businessFilter.IsEmpty())
        {
            for (int i = 0; i < _businessConfig.BaseCosts.Length; i++)
            {
                var entity = _world.NewEntity();
                ref var business = ref entity.Get<Business>();
                business.Id = i;
                business.Level = 0;
                business.Upgrade1Purchased = false;
                business.Upgrade2Purchased = false;

                entity.Get<IncomeProgress>();
            }
        }

        // отмечаем, что инициализация завершена
        _world.NewEntity().Get<BusinessInitialized>();
    }
}