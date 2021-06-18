using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkToHighestInfluence", menuName = "Actions/WalkToHighestInfluence")]
public class WalkToHighestInfluence : Action
{
    public override void action(UnitManager agent)
    {
        Vector2 pos = agent.squadInfluenceManager.map.getWorldPositionFromCell(agent.squadInfluenceManager.map.highestInfluencePos.x, agent.squadInfluenceManager.map.highestInfluencePos.y);

        if (agent.aiPath != null)
            agent.aiPath.destination = pos;
    }
}
