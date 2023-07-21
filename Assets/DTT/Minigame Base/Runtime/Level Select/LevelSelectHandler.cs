﻿using System;
using DTT.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DTT.MinigameBase.LevelSelect
{
    /// <summary>
    /// Can be inherited from to be able to add a Level Select flow to your minigame.
    /// Will automatically start up the Level Select UI and allow the user to use the level selection to start the game.
    /// </summary>
    /// <typeparam name="TConfig">The configuration type of your minigame.</typeparam>
    /// <typeparam name="TResult">The result type of your minigame.</typeparam>
    /// <typeparam name="TMinigame">The minigame manager class.</typeparam>
    public abstract class LevelSelectHandler<TConfig, TResult, TMinigame> : LevelSelectHandlerBase where TMinigame : Object, IMinigame<TConfig, TResult>
    {
        /// <summary>
        /// Stores all the level data.
        /// </summary>
        [SerializeField]
        [Tooltip("Stores all the level data.")]
        private LevelDatabase _levelDatabase;

        /// <summary>
        /// Whether to immediately return to the Level Select when the <see cref="IMinigame{TConfig,TResult}.Finish"/> is invoked.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether to immediately return to the Level Select when the IMinigame.Finish is invoked.")]
        private bool _returnToLevelSelectOnFinish;
        
        /// <summary>
        /// The UI was opened.
        /// </summary>
        public event Action LevelSelectOpened;
        
        /// <summary>
        /// The UI was closed.
        /// </summary>
        public event Action LevelSelectClosed;
        
        /// <summary>
        /// The current level the user is playing.
        /// </summary>
        public override int CurrentLevel => _currentLevel;
        
        /// <summary>
        /// The current level the user is playing.
        /// </summary>
        private int _currentLevel;
        
        /// <summary>
        /// Manages the level selection process.
        /// </summary>
        private LevelSelect _levelSelect;

        /// <summary>
        /// The minigame that is being played.
        /// </summary>
        private IMinigame<TConfig, TResult> _minigame;

        /// <summary>
        /// The canvas group of the level select root. Used for fading the UI in/out.
        /// </summary>
        private CanvasGroup _levelSelectCanvasGroup;

        /// <summary>
        /// Loads in the Level Selection additively, to enable Level Select operations.
        /// </summary>
        protected void Awake()
        {
            _minigame = FindObjectOfType<TMinigame>();

            AsyncOperation async = SceneManager.LoadSceneAsync(LevelSelect.SCENE_NAME, LoadSceneMode.Additive);
            
            if (async == null)
            {
                Debug.LogError("The scene 'Level Select' should be added to the Build Settings so it can be loaded in!");
                return;
            }

            async.completed += _ =>
            {
                _levelSelect = FindObjectOfType<LevelSelect>();
                _levelSelectCanvasGroup = _levelSelect.transform.root.gameObject.AddComponent<CanvasGroup>();
                _levelDatabase.Load();
                _levelSelect.Populate(_levelDatabase);
            };
        }

        /// <summary>
        /// Adds listeners.
        /// </summary>
        protected void OnEnable() => _minigame.Finish += OnMinigameFinished;

        /// <summary>
        /// Removes listeners.
        /// </summary>
        protected void OnDisable() => _minigame.Finish -= OnMinigameFinished;

        /// <summary>
        /// Called when a level has been selected with the number of the level being played.
        /// Starts the game at that level.
        /// </summary>
        /// <param name="levelData">The data of the level that was selected.</param>
        protected virtual void OnLevelSelected(int difficultSelected)
        {
            _minigame.StartGame(GetConfig(difficultSelected));
        }

        /// <summary>
        /// Called when the minigame has finished with the result of the user.
        /// </summary>
        /// <param name="result">The result of the user.</param>
        protected virtual void OnMinigameFinished(TResult result)
        {
            float score = CalculateScore(result);

        }
        
        /// <summary>
        /// Should retrieve the level config object for that specific level.
        /// </summary>
        /// <param name="levelNumber">The current level that a config is required for.</param>
        /// <returns>The level config that is required for this level.</returns>
        protected abstract TConfig GetConfig(int levelNumber);

        /// <summary>
        /// Should calculate a score between 0 and 1, of how the user performed based on the results.
        /// </summary>
        /// <param name="result">The result of how the user performed.</param>
        /// <returns>A score between 0 and 1.</returns>
        protected abstract float CalculateScore(TResult result);
    }
}