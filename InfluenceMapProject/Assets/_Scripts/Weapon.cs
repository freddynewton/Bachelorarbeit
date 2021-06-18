using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject _BulletPF;
    public Transform BulletSpawnPos;

    public void SpawnBullet(int damage, UnitManager manager)
    {
        GameObject go = Instantiate(_BulletPF.gameObject, BulletSpawnPos.position, Quaternion.identity, null);
        go.GetComponent<Bullet>().Shoot(damage, manager._Target.position, manager);
    }
}
