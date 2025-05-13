using System.Collections.Generic;
using Leopotam.Ecs;
using System.IO;
using UnityEngine;

public class SaveSystem : IEcsSystem
{
    private const string SaveFileName = "game_save.json";

    private EcsWorld _world;
    private EcsFilter<Business> _businessFilter;
    private EcsFilter<Balance> _balanceFilter;

    public void Save()
    {
        var saveData = new SaveData();

        // Save balance
        if (!_balanceFilter.IsEmpty())
        {
            saveData.Balance = _balanceFilter.Get1(0).Value;
        }

        // Save businesses
        foreach (var i in _businessFilter)
        {
            var business = _businessFilter.Get1(i);
            var progress = _businessFilter.GetEntity(i).Get<IncomeProgress>();

            saveData.Businesses.Add(new BusinessSaveData
            {
                Id = business.Id,
                Level = business.Level,
                Upgrade1Purchased = business.Upgrade1Purchased,
                Upgrade2Purchased = business.Upgrade2Purchased,
                IncomeProgress = progress.Value,
                IncomeTimePassed = progress.TimePassed
            });
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(GetSavePath(), json);
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }
}

[System.Serializable]
public class SaveData
{
    public int Balance;
    public List<BusinessSaveData> Businesses = new List<BusinessSaveData>();
}

[System.Serializable]
public class BusinessSaveData
{
    public int Id;
    public int Level;
    public bool Upgrade1Purchased;
    public bool Upgrade2Purchased;
    public float IncomeProgress;
    public float IncomeTimePassed;
}