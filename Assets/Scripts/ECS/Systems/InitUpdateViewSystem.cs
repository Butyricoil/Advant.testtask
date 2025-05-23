using Leopotam.Ecs;

public class InitUpdateViewSystem : IEcsInitSystem
{
    private EcsWorld _world;

    public void Init()
    {
        _world.NewEntity().Get<UpdateViewComponent>();
    }
} 