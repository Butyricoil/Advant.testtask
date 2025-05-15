using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.PauseMenu
{
    public class PauseMenuView : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _dropSaveButton;
        [SerializeField] private Button _exitButton;

        [Header("Menu")]
        [SerializeField] private GameObject _pauseMenu;


        private EcsWorld _world;

        public void Initialize(EcsWorld world)
        {
            _world = world;
        }

        private void Start()
        {
            ValidateReferences();
            InitializeButtons();
        }

        private void ValidateReferences()
        {
            if (_pauseButton == null)
                Debug.LogError("Кнопка паузы не назначена!");
            if (_continueButton == null)
                Debug.LogError("Кнопка продолжения не назначена!");
            if (_saveButton == null)
                Debug.LogError("Кнопка сохранения не назначена!");
            if (_dropSaveButton == null)
                Debug.LogError("Кнопка сброса сохранений не назначена!");
            if (_exitButton == null)
                Debug.LogError("Кнопка выхода не назначена!");
            if (_pauseMenu == null)
                Debug.LogError("Меню паузы не назначено!");
        }

        private void InitializeButtons()
        {
            _pauseButton.onClick.AddListener(OnPauseClicked);
            _continueButton.onClick.AddListener(OnContinueClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _dropSaveButton.onClick.AddListener(OnDropSaveClicked);
            _exitButton.onClick.AddListener(OnExitClicked);

            _pauseMenu.SetActive(false);
        }


        private void OnPauseClicked()
        {
            if (_world != null && _world.IsAlive())
            {
                _pauseMenu.SetActive(true);
                _pauseButton.interactable = false;
                _pauseButton.gameObject.SetActive(false);
                _world.NewEntity().Get<PauseEvent>();
            }
        }

        private void OnContinueClicked()
        {
            if (_world != null && _world.IsAlive())
            {
                _pauseMenu.SetActive(false);
                _pauseButton.interactable = true;
                _pauseButton.gameObject.SetActive(true);
                _world.NewEntity().Get<UnpauseEvent>();
            }
        }

        private void OnSaveClicked()
        {
            if (_world != null && _world.IsAlive())
            {
                _world.NewEntity().Get<SaveEvent>();
            }
        }

        private void OnDropSaveClicked()
        {
            if (_world != null && _world.IsAlive())
            {
                _world.NewEntity().Get<DropSaveEvent>();
            }
        }

        private void OnExitClicked()
        {
            if (_world != null && _world.IsAlive())
            {
                _world.NewEntity().Get<SaveEvent>();
            }
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void OnDestroy()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(OnPauseClicked);
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnContinueClicked);
            if (_saveButton != null)
                _saveButton.onClick.RemoveListener(OnSaveClicked);
            if (_dropSaveButton != null)
                _dropSaveButton.onClick.RemoveListener(OnDropSaveClicked);
            if (_exitButton != null)
                _exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
} 