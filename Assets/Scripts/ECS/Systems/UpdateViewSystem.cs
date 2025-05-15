using System;
using Leopotam.Ecs;
using Object = UnityEngine.Object;

public class UpdateViewSystem : IEcsRunSystem
{
    private EcsFilter<UpdateViewEvent> _updateEvents;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balance;

    [Obsolete("Obsolete")]
    public void Run()
    {
        if (_updateEvents.IsEmpty()) return;

        foreach (var i in _updateEvents)
        {
            _updateEvents.GetEntity(i).Destroy();
        }

        var businessViews = Object.FindObjectsOfType<BusinessView>();
        foreach (var view in businessViews)
        {
            view.UpdateView();
        }

        var balanceViews = Object.FindObjectsOfType<BalanceView>();
        foreach (var view in balanceViews)
        {
            view.UpdateView();
        }
    }
}