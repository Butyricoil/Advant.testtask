using Leopotam.Ecs;
using UnityEngine;

public class PauseSystem : IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter<PauseEvent> _pauseEvents;
    private EcsFilter<UnpauseEvent> _unpauseEvents;
    private EcsFilter<Paused> _pausedFilter;

    public void Run()
    {
        // Обработка события паузы
        if (!_pauseEvents.IsEmpty() && _pausedFilter.IsEmpty())
        {
            _world.NewEntity().Get<Paused>();
            Time.timeScale = 0f;
            foreach (var i in _pauseEvents)
                _pauseEvents.GetEntity(i).Destroy();
        }

        // Обработка события снятия паузы
        if (!_unpauseEvents.IsEmpty() && !_pausedFilter.IsEmpty())
        {
            foreach (var i in _pausedFilter)
                _pausedFilter.GetEntity(i).Destroy();
            Time.timeScale = 1f;
            foreach (var i in _unpauseEvents)
                _unpauseEvents.GetEntity(i).Destroy();
        }
    }
} 