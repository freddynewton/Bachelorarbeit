using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transitions/AttackTransition", fileName = "AttackTransition")]
public class AttackTransition : Transition
{
    public override bool checkTransition(SquadInfluenceManager squad)
    {
        int unsharedCount = 0;
        int sharedCount = 0;

        bool haveOtherTeamMorePoints = false;

        foreach (GameController.POINTSTATE pst in GameController.Instance.pointstates)
        {
            if (pst == GameController.POINTSTATE.SHARED) sharedCount++;
            if (pst == GameController.POINTSTATE.UNSHARED) unsharedCount++;
        }

        if (squad.style == SquadInfluenceManager.InfluenceStyle.SHARED)
        {
            if (sharedCount < unsharedCount) haveOtherTeamMorePoints = true;
        }
        else if (squad.style == SquadInfluenceManager.InfluenceStyle.UNSHARED)
        {
            if (unsharedCount < sharedCount) haveOtherTeamMorePoints = true;
        }

        return haveOtherTeamMorePoints;
    }
}
