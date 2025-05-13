using Leopotam.Ecs;
using TMPro;
using UnityEngine;

public class BalanceView : MonoBehaviour
{
    [SerializeField] private TMP_Text _balanceText;

    private EcsWorld _world;
    private EcsEntity _balanceEntity;

    public void Initialize(EcsWorld world, EcsEntity balanceEntity)
    {
        _world = world;
        _balanceEntity = balanceEntity;
        UpdateView();
    }

    private void Update()
    {
        if (!_world.IsAlive() || !_balanceEntity.IsAlive()) return;

        UpdateView();
    }

    private void UpdateView()
    {
        if (_balanceEntity.Has<Balance>())
        {
            _balanceText.text = $"Баланс: ${_balanceEntity.Get<Balance>().Value}";
        }
    }
}