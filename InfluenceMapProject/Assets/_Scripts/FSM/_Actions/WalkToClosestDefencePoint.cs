using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkToClosestDefencePoint", menuName = "Actions/WalkToClosestDefencePoint")]
public class WalkToClosestDefencePoint : Action
{
    private List<InfluencerManager.CapturePoint> capList = new List<InfluencerManager.CapturePoint>();

    public override void action(UnitManager agent)
    {
        int idx = -1;
        Vector2Int closestCell = Vector2Int.zero;

        capList.Clear();

        // Get CapPointss
        for (int i = 0; i < GameController.Instance.pointstates.Length; i++)
        {
            if ((agent.squadInfluenceManager.style == SquadInfluenceManager.InfluenceStyle.SHARED && GameController.Instance.pointstates[i] == GameController.POINTSTATE.SHARED) ||
                    (agent.squadInfluenceManager.style == SquadInfluenceManager.InfluenceStyle.UNSHARED && GameController.Instance.pointstates[i] == GameController.POINTSTATE.UNSHARED))
            {
                capList.Add(InfluencerManager.Instance.capturePoints[i]);
            }
        }

       
        for (int x = 0; x < agent.squadInfluenceManager.map._map.GetLength(0); x++)
        {
            for (int y = 0; y < agent.squadInfluenceManager.map._map.GetLength(1); y++)
            {
                for (int i = 0; i < capList.Count; i++)
                {
                    if (capList[i].StartPoint.x < x && capList[i].StartPoint.x + capList[i].Size.x > x && capList[i].StartPoint.y < y && capList[i].StartPoint.y + capList[i].Size.y > y)
                    {
                        if (closestCell == Vector2Int.zero)
                        {
                            idx = i;
                            closestCell = new Vector2Int(x, y);
                        }
                        else
                        {
                            if (Vector2.Distance(agent.gameObject.transform.position, agent.squadInfluenceManager.map.getWorldPositionFromCell(x, y)) <
                                Vector2.Distance(agent.gameObject.transform.position, agent.squadInfluenceManager.map.getWorldPositionFromCell(closestCell.x, closestCell.y)))
                            {
                                idx = i;
                                closestCell = new Vector2Int(x, y);
                            }
                        }
                    }
                }
            }
        }

        if (idx != -1)
        {
            Vector2Int pos = Vector2Int.zero;

            // Choose Pos
            for (int x = 0; x < agent.squadInfluenceManager.map._map.GetLength(0); x++)
            {
                for (int y = 0; y < agent.squadInfluenceManager.map._map.GetLength(1); y++)
                {
                    if (agent.squadInfluenceManager.map._map[x, y].y == 0)
                    {
                        if (capList[idx].StartPoint.x <
                        x && capList[idx].StartPoint.x +
                        capList[idx].Size.x >
                        x && capList[idx].StartPoint.y <
                        y && capList[idx].StartPoint.y +
                        capList[idx].Size.y > y)
                        {
                            if (pos == Vector2Int.zero)
                            {
                                pos = new Vector2Int(x, y);
                            }
                            else
                            {
                                if (agent.squadInfluenceManager.map._map[x, y].x < agent.squadInfluenceManager.map._map[pos.x, pos.y].x)
                                {
                                    pos = new Vector2Int(x, y);
                                }
                            }
                        }
                    }
                }
            }

            // Set Path Position
            agent.aiPath.destination = agent.squadInfluenceManager.map.getWorldPositionFromCell(pos.x, pos.y);
        }
    }

}
