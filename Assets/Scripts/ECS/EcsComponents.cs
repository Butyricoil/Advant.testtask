public struct Business
{
    public int Id;
    public int Level;
    public bool Upgrade1Purchased;
    public bool Upgrade2Purchased;
}

public struct IncomeProgress
{
    public float Value;
    public float TimePassed;
}

public struct LevelUpRequest { public int BusinessId; }

public struct UpgradeRequest
{
    public int BusinessId;
    public int UpgradeId;
}

public struct Balance { public int Value; }

public readonly struct UpdateViewEvent {}

public readonly struct SaveEvent {}

public readonly struct DropSaveEvent {}

public readonly struct LoadEvent {}

public struct BusinessInitialized {}

public struct PauseEvent { }

public struct ContinueEvent { }

public struct LoadSceneEvent { public string SceneName; }

public struct ExitEvent { }