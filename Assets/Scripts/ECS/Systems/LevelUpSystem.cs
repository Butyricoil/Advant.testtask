using Leopotam.Ecs;

public class LevelUpSystem : IEcsRunSystem
{
    private readonly BusinessConfig _config;

    private EcsWorld _world;
    private EcsFilter<LevelUpRequest> _requests;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balanceFilter;

    public LevelUpSystem(BusinessConfig config)
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
                    int cost = (business.Level + 1) * _config.BaseCosts[request.BusinessId];
                    if (balance.Value >= cost)
                    {
                        balance.Value -= cost;
                        business.Level++;
                        _world.NewEntity().Get<UpdateViewEvent>();
                    }
                    break;
                }
            }

            _requests.GetEntity(i).Destroy();
        }
    }
}