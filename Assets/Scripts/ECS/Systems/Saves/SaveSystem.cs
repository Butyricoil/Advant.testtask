using Leopotam.Ecs;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter<SaveEvent> _saveEvents;
    private EcsFilter<Business> _businesses;
    private EcsFilter<Balance> _balance;
    private EcsFilter<Business, IncomeProgress> _progress;

    public void Run()
    {
        if (_saveEvents.IsEmpty()) return;

        var saveData = new GameSaveData
        {
            Balance = _balance.Get1(0).Value,
            Businesses = new List<BusinessSaveData>()
        };

        // сохранение данных бизнеса
        foreach (var i in _businesses)
        {
            ref var business = ref _businesses.Get1(i);
            var businessData = new BusinessSaveData
            {
                Id = business.Id,
                Level = business.Level,
                Upgrade1Purchased = business.Upgrade1Purchased,
                Upgrade2Purchased = business.Upgrade2Purchased
            };
            saveData.Businesses.Add(businessData);
        }

        // сохранение прогресса дохода
        foreach (var i in _progress)
        {
            ref var business = ref _progress.Get1(i);
            ref var progress = ref _progress.Get2(i);
            var businessId = business.Id;
            
            var businessData = saveData.Businesses.Find(b => b.Id == businessId);
            if (businessData != null)
            {
                businessData.IncomeProgress = progress.Value;
                businessData.TimePassed = progress.TimePassed;
            }
        }

        // сохранение в файл
        string json = JsonUtility.ToJson(saveData, true);
        string savePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        File.WriteAllText(savePath, json);
        Debug.Log($"Игра сохранена в: {savePath}");

        // очистка событий сохранения
        foreach (var i in _saveEvents)
        {
            _saveEvents.GetEntity(i).Destroy();
        }
    }
}

[System.Serializable]
public class GameSaveData
{
    public int Balance;
    public List<BusinessSaveData> Businesses;
}

[System.Serializable]
public class BusinessSaveData
{
    public int Id;
    public int Level;
    public bool Upgrade1Purchased;
    public bool Upgrade2Purchased;
    public float IncomeProgress;
    public float TimePassed;
}
