using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition : ScriptableObject
{
    public State toState;

    public abstract bool checkTransition(SquadInfluenceManager squad);
}
