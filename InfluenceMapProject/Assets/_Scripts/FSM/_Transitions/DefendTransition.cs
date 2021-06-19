using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Transitions/DefendTransition", fileName = "DefendTransition")]
public class DefendTransition : Transition
{
    public override bool checkTransition(SquadInfluenceManager squad)
    {
        int unsharedCount = 0;
        int sharedCount = 0;

        bool haveOurTeamMorePoints = false;

        foreach (GameController.POINTSTATE pst in GameController.Instance.pointstates)
        {
            if (pst == GameController.POINTSTATE.SHARED) sharedCount++;
            if (pst == GameController.POINTSTATE.UNSHARED) unsharedCount++;
        }

        if (squad.style == SquadInfluenceManager.InfluenceStyle.SHARED)
        {
            if (sharedCount > unsharedCount) haveOurTeamMorePoints = true;
        }
        else if (squad.style == SquadInfluenceManager.InfluenceStyle.UNSHARED)
        {
            if (unsharedCount > sharedCount) haveOurTeamMorePoints = true;
        }

        return haveOurTeamMorePoints;
    }
}
