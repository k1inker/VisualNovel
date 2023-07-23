using Naninovel;
using UnityEngine;

public class BindingDTTNaninovel : MonoBehaviour
{
    [SerializeField] private MemoryGameManager memoryGameManager;
    private void OnEnable() => memoryGameManager.ResultGame += FinishGame;
    private void OnDisable() => memoryGameManager.ResultGame -= FinishGame;
    public void StartGame()
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();

        variableManager.TryGetVariableValue<int>("difficultGame", out var intValue);
        memoryGameManager.StartGame(intValue);
    }
    private void FinishGame(int result)
    {
        var variableManager = Engine.GetService<ICustomVariableManager>();
        variableManager.TrySetVariableValue("resultGame", result);
        GetComponent<PlayScript>().Play();
    }
}
