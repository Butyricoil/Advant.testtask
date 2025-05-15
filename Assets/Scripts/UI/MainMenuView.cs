using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;

        private void Start()
        {
            ValidateReferences();
            InitializeButtons();
        }

        private void ValidateReferences()
        {
            if (_startButton == null)
                Debug.LogError("Кнопка старта не назначена!");
            if (_exitButton == null)
                Debug.LogError("Кнопка выхода не назначена!");
        }

        private void InitializeButtons()
        {
            _startButton.onClick.AddListener(OnStartClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnStartClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void OnExitClicked()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        private void OnDestroy()
        {
            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);
            if (_exitButton != null)
                _exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
}