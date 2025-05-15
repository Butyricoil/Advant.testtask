using Leopotam.Ecs;
using System.IO;
using UnityEngine;

public class DropSaveSystem : IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter<DropSaveEvent> _dropSaveEvents;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balance;
    private EcsFilter<Business, IncomeProgress> _progress;

    public void Run()
    {
        if (_dropSaveEvents.IsEmpty()) return;

        // Удаляем файл сохранения
        string savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Сохранение успешно удалено");
        }

        // Сбрасываем все бизнесы
        foreach (var i in _businesses)
        {
            ref var business = ref _businesses.Get1(i);
            business.Level = 0;
            business.Upgrade1Purchased = false;
            business.Upgrade2Purchased = false;
        }

        // Сбрасываем баланс
        ref var balance = ref _balance.Get1(0);
        balance.Value = 12;

        // Сбрасываем прогресс дохода
        foreach (var i in _progress)
        {
            ref var progress = ref _progress.Get2(i);
            progress.Value = 0;
            progress.TimePassed = 0;
        }

        // Уничтожаем событие
        _dropSaveEvents.GetEntity(0).Destroy();
    }
}
