using Leopotam.Ecs;
using System.IO;
using UnityEngine;

public class LoadSystem : IEcsInitSystem
{
    private const string SaveFileName = "game_save.json";

    private EcsWorld _world;
    private BusinessConfig _businessConfig;

    public void Init()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        var saveData = JsonUtility.FromJson<SaveData>(json);

        // Load balance
        var balanceEntity = _world.NewEntity();
        balanceEntity.Get<Balance>().Value = saveData.Balance;

        // Load businesses
        foreach (var businessSave in saveData.Businesses)
        {
            var businessEntity = _world.NewEntity();
            ref var business = ref businessEntity.Get<Business>();

            business.Id = businessSave.Id;
            business.Level = businessSave.Level;
            business.Upgrade1Purchased = businessSave.Upgrade1Purchased;
            business.Upgrade2Purchased = businessSave.Upgrade2Purchased;

            ref var progress = ref businessEntity.Get<IncomeProgress>();
            progress.Value = businessSave.IncomeProgress;
            progress.TimePassed = businessSave.IncomeTimePassed;
        }
    }
}