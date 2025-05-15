using Leopotam.Ecs;
            using UnityEngine;
            using UnityEngine.SceneManagement;

            public class PauseMenuSystem : IEcsRunSystem
            {
                private EcsWorld _world;
                private EcsFilter<PauseEvent> _pauseEvents;
                private EcsFilter<ContinueEvent> _continueEvents;
                private EcsFilter<ExitEvent> _exitEvents;
                private EcsFilter<LoadSceneEvent> _loadSceneEvents;

                public void Run()
                {
                    // Обработка паузы
                    foreach (var i in _pauseEvents)
                    {
                        Time.timeScale = 0f;
                        _pauseEvents.GetEntity(i).Destroy();
                    }

                    // Обработка продолжения
                    foreach (var i in _continueEvents)
                    {
                        Time.timeScale = 1f;
                        _continueEvents.GetEntity(i).Destroy();
                    }

                    // Обработка выхода
                    foreach (var i in _exitEvents)
                    {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                        _exitEvents.GetEntity(i).Destroy();
                    }

                    // Обработка загрузки сцены
                    foreach (var i in _loadSceneEvents)
                    {
                        var sceneName = _loadSceneEvents.Get1(i).SceneName;
                        Time.timeScale = 1f;
                        SceneManager.LoadScene(sceneName);
                        _loadSceneEvents.GetEntity(i).Destroy();
                    }
                }
            }