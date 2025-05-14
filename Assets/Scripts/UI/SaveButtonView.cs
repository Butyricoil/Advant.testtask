using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonView : MonoBehaviour
{
    [SerializeField] private Button _saveButton; // кнопка сохранения

    private EcsWorld _world;

    private void Start()
    {
        if (_saveButton == null)
        {
            Debug.LogError("Отсутствует ссылка на кнопку сохранения!");
            return;
        }

        _saveButton.onClick.AddListener(OnSaveClicked);
    }

    public void Initialize(EcsWorld world)
    {
        _world = world;
    }

    private void OnSaveClicked()
    {
        if (_world != null && _world.IsAlive())
        {
            _world.NewEntity().Get<SaveEvent>();
        }
    }

    private void OnDestroy()
    {
        if (_saveButton != null)
        {
            _saveButton.onClick.RemoveListener(OnSaveClicked);
        }
    }
} 