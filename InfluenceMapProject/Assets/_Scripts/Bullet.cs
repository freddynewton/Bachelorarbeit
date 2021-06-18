using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private UnitManager _shooter;
    public float _speed = 500f;

    [HideInInspector] public int _Damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(int damage, Vector2 target, UnitManager shooter)
    {
        _Damage = damage;
        _shooter = shooter;

        rb.AddForce((target - (Vector2)transform.position).normalized * _speed * Time.deltaTime, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitManager uManager = collision.gameObject.GetComponent<UnitManager>();

        if (uManager != null)
        {
            if (uManager.squadInfluenceManager.style != _shooter.squadInfluenceManager.style)
            {
                uManager.getDamage(_Damage);
                Destroy(gameObject);
            }
        } else
        {
            if (!collision.CompareTag("Bullet")) Destroy(gameObject);
        }
    }
}
