using UnityEngine;
using System;
using Timer = DTT.MinigameBase.Timer.Timer;
using DTT.MinigameMemory;
using Naninovel;

/// <summary>
/// Class that functions as the minigame manager.
/// </summary>
public class MemoryGameManager : MonoBehaviour
{ 
    /// <summary>
    /// Is called when the game has started.
    /// </summary>
    public event Action Started;

    /// <summary>
    /// Is called when the game has finished.
    /// </summary>
    public event Action<MemoryGameResults> Finish;

    public event Action<int> ResultGame;
    /// <summary>
    /// Time that has passed in the game.
    /// </summary>
    public TimeSpan Time => _timer.TimePassed;

    /// <summary>
    /// The <see cref="Board"/> used for the minigame.
    /// </summary>
    [SerializeField]
    private Board _board;

    /// <summary>
    /// Game timer.
    /// </summary>
    [SerializeField]
    private Timer _timer;

    /// <summary>
    /// The GameSettings.
    /// </summary>
    [SerializeField] private MemoryGameSettings[] _settings = new MemoryGameSettings[2];

    /// <summary>
    /// The amount a player has tried to match two cards during the game.
    /// </summary>
    private int _amountOfTurns = 0;

    private int _result = 0;
    /// <summary>
    /// Starts the game with the given settings.
    /// </summary>
    /// <param name="settings">The settings used for this play session.</param>
    public void StartGame(int difficulty)
    {
        _amountOfTurns = 0;
        _timer.Begin();

        _board.SetupGame(_settings[difficulty]);
        Started?.Invoke();
    }
    /// <summary>
    /// Finishes the current game.
    /// </summary>
    public void ForceFinish()
    {
        _timer.Stop();
        Finish?.Invoke(new MemoryGameResults(_timer.TimePassed, _amountOfTurns));
        ResultGame?.Invoke(_result);
    }
    /// <summary>
    /// Adds a <see cref="Timer"/> to the gameobject if there was not timer assigned.
    /// </summary>
    private void Awake() => _timer = (_timer == null) ? this.gameObject.AddComponent<Timer>() : _timer;

    /// <summary>
    /// Subscribe to board events.
    /// </summary>
    private void OnEnable()
    {
        _board.CardsTurned += IncreaseTurnAmount;
        _board.AllCardsMatched += ForceFinish;
        Finish += StoreResults;
    }

    /// <summary>
    /// Unsubscribe from board events.
    /// </summary>
    private void OnDisable()
    {
        _board.CardsTurned -= IncreaseTurnAmount;
        _board.AllCardsMatched -= ForceFinish;
    }

    /// <summary>
    /// Increases the amount of turns taken by one.
    /// </summary>
    private void IncreaseTurnAmount() => _amountOfTurns++;
    private void StoreResults(MemoryGameResults results)
    {
        if (results.amountOfTurns == 0)
        {
            _result = 0;
        }
        _result = (int)Mathf.InverseLerp(3, 0, results.timeTaken.Seconds / 20);
    }
}
