using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAtTarget", menuName = "Actions/ShootAtTarget")]
public class ShootAtTargetAction : Action
{
    public override void action(UnitManager agent)
    {
        if (agent._Target != null && agent.checkTimer(agent._AttackRate))
        {
            agent._Weapon.SpawnBullet(agent._Damage, agent);
        }
    }
}
