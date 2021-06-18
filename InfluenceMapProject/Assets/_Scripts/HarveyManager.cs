using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarveyManager : MonoBehaviour
{
    public List<Action> actionList = new List<Action>();

    private UnitManager manager;

    private void Update()
    {
        if (actionList.Count != 0)
        {
            actionList[0].action(manager);
            actionList.Remove(actionList[0]);
        }

        if (actionList.Count == 0)
        {
            manager.squadInfluenceManager.callBackAction(manager);
        }
    }

    private void Awake()
    {
        manager = GetComponent<UnitManager>();
    }
}
