using Leopotam.Ecs;

public class UpgradeSystem : IEcsRunSystem
{
    private readonly BusinessConfig _config;

    private EcsWorld _world;
    private EcsFilter<UpgradeRequest> _requests;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balanceFilter;

    public UpgradeSystem(BusinessConfig config)
    {
        _config = config;
    }

    public void Run()
    {
        ref var balance = ref _balanceFilter.Get1(0);

        foreach (var i in _requests)
        {
            var request = _requests.Get1(i);

            foreach (var j in _businesses)
            {
                ref var business = ref _businesses.Get1(j);

                if (business.Id == request.BusinessId)
                {
                    int price = request.UpgradeId == 1
                        ? _config.Upgrade1Prices[request.BusinessId]
                        : _config.Upgrade2Prices[request.BusinessId];

                    if (balance.Value >= price && !(request.UpgradeId == 1 ? business.Upgrade1Purchased : business.Upgrade2Purchased))
                    {
                        balance.Value -= price;

                        if (request.UpgradeId == 1)
                            business.Upgrade1Purchased = true;
                        else
                            business.Upgrade2Purchased = true;

                        _world.NewEntity().Get<UpdateViewEvent>();
                    }
                    break;
                }
            }

            _requests.GetEntity(i).Destroy();
        }
    }
}