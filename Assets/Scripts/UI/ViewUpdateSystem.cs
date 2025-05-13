using Leopotam.Ecs;

public class ViewUpdateSystem : IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter<UpdateViewEvent> _filter;

    public void Run()
    {
        foreach (var i in _filter)
        {
            _filter.GetEntity(i).Destroy();
        }
    }
}