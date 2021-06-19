using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{
    private List<UnitManager> units = new List<UnitManager>();
    public int IDX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitManager um = collision.gameObject.GetComponent<UnitManager>();
        if (um != null) units.Add(um);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UnitManager um = collision.gameObject.GetComponent<UnitManager>();
        if (um != null) units.Remove(um);
    }

    private void LateUpdate()
    {
        int uCount = 0;
        int sCount = 0;

        foreach (UnitManager item in units)
        {
            if (item.squadInfluenceManager.style == SquadInfluenceManager.InfluenceStyle.SHARED) sCount++;
            else uCount++;
        }

        GameController.POINTSTATE pState = GameController.Instance.pointstates[IDX];

        if (uCount == 0 && sCount != 0)
        {
            GameController.Instance.pointstates[IDX] = GameController.POINTSTATE.SHARED;
        }
        else if (uCount != 0 && sCount == 0)
        {
            GameController.Instance.pointstates[IDX] = GameController.POINTSTATE.UNSHARED;
        }

        if (pState != GameController.Instance.pointstates[IDX])
        {
            IterationManager.Instance.currentMatchData.PointStatesVerlauf.Add(new IterationManager.PointStateStatus(GameController.Instance.pointstates));
        }

        GameController.Instance.UpdatePointStateText();
    }
}
