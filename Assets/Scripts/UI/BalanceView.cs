using Leopotam.Ecs;
using TMPro;
using UnityEngine;

public class BalanceView : MonoBehaviour
{
    [SerializeField] private TMP_Text _balanceText;

    private EcsWorld _world;
    private EcsEntity _balanceEntity;

    private void ValidateReferences()
    {
        if (_balanceText == null)
            Debug.LogError("Balance text reference is missing!");
    }

    public void Initialize(EcsWorld world, EcsEntity balanceEntity)
    {
        ValidateReferences();

        if (world == null || !balanceEntity.IsAlive())
        {
            Debug.LogError("Failed to initialize BalanceView: Invalid references");
            return;
        }

        _world = world;
        _balanceEntity = balanceEntity;
        UpdateView();
    }

    private void Update()
    {
        if (_world == null || !_world.IsAlive() || !_balanceEntity.IsAlive()) return;

        UpdateView();
    }

    public void UpdateView()
    {
        if (!_balanceEntity.IsAlive() || !_balanceText) return;

        if (_balanceEntity.Has<Balance>())
        {
            _balanceText.text = $"Balance: ${_balanceEntity.Get<Balance>().Value}";
        }
    }
}