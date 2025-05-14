using Leopotam.Ecs;
using System.IO;
using UnityEngine;

public class LoadSystem : IEcsInitSystem
{
    private EcsWorld _world;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balance;

    public void Init()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        if (!File.Exists(savePath))
        {
            Debug.Log("Файл сохранения не найден. Начинаем новую игру.");
            return;
        }

        string json = File.ReadAllText(savePath);
        var saveData = JsonUtility.FromJson<GameSaveData>(json);

        // загрузка баланса
        if (!_balance.IsEmpty())
        {
            ref var balance = ref _balance.Get1(0);
            balance.Value = saveData.Balance;
        }

        // загрузка бизнесов
        foreach (var businessData in saveData.Businesses)
        {
            foreach (var i in _businesses)
            {
                ref var business = ref _businesses.Get1(i);
                if (business.Id == businessData.Id)
                {
                    business.Level = businessData.Level;
                    business.Upgrade1Purchased = businessData.Upgrade1Purchased;
                    business.Upgrade2Purchased = businessData.Upgrade2Purchased;

                    // восстановление прогресса дохода
                    var entity = _businesses.GetEntity(i);
                    if (entity.Has<IncomeProgress>())
                    {
                        ref var progress = ref entity.Get<IncomeProgress>();
                        progress.Value = businessData.IncomeProgress;
                        progress.TimePassed = businessData.TimePassed;
                    }
                    break;
                }
            }
        }

        Debug.Log("Игра успешно загружена");
    }
}
