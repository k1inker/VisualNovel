using Naninovel;
using UnityEngine;

public class EnableQuestLocation : MonoBehaviour
{
    [SerializeField] private GameObject[] questSign;

    private ICustomVariableManager _variableManager;
    private void Start()
    {
        _variableManager = Engine.GetService<ICustomVariableManager>();
    }
    public void EnableQuestSign()
    {

        string selectLocation = _variableManager.GetVariableValue("locationQuest");
        if ("warehouse" == selectLocation)
        {
            questSign[0].SetActive(true);
        }
        else if ("town" == selectLocation)
        {
            questSign[1].SetActive(true);
        }
        else if ("club" == selectLocation)
        {
            questSign[2].SetActive(true);
        }
    }
    public void DisableQuestSign()
    {
        foreach(var sign in questSign)
        {
            sign.SetActive(false);
        }
    }
}
