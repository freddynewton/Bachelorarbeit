using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="State", menuName ="FSM/State")]
public class State : ScriptableObject
{
    public Action[] Actions;
    public Transition[] Transitions;
}
