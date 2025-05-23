using Leopotam.Ecs;

public class UpdateViewSystem : IEcsRunSystem
{
    private EcsFilter<UpdateViewEvent> _updateViewFilter;
    private EcsFilter<UpdateViewComponent> _updateViewComponentFilter;

    public void Run()
    {
        if (_updateViewFilter.IsEmpty()) return;

        foreach (var i in _updateViewComponentFilter)
        {
            ref var updateViewComponent = ref _updateViewComponentFilter.Get1(i);
            updateViewComponent.UpdateAllViews();
        }

        foreach (var i in _updateViewFilter)
        {
            _updateViewFilter.GetEntity(i).Destroy();
        }
    }
} 