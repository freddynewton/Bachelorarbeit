using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadInfluenceManager : MonoBehaviour
{
    public enum InfluenceStyle
    {
        SHARED,
        UNSHARED
    }

    public enum SquadTeam
    {
        TEAM1,
        TEAM2
    }

    public InfluenceStyle style;
    //public SquadTeam team;
    public Imap map;
    public LayerMask ignoreTargetLayerMask;

    [Header("State Library")]
    public State currentState;
    public State[] states;

    [Header("Unit Lists")]
    public List<Transform> allied = new List<Transform>();
    public List<Transform> enemies = new List<Transform>();

    [Header("Debug Settings")]
    public bool showDebugRays = false;
    [HideInInspector] public List<UnitManager> squadUnits = new List<UnitManager>();

    private void Update()
    {
        CheckTransitions();
    }

    private void CheckTransitions()
    {
        if (currentState != null)
        {
            foreach (Transition t in currentState.Transitions)
            {
                if (t.checkTransition(this))
                {
                    currentState = t.toState;

                    foreach (UnitManager u in gameObject.GetComponentsInChildren<UnitManager>())
                    {
                        u.harveyManager.actionList.Clear();
                        callBackAction(u);
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        ScanForUnit();
    }

    public void callBackAction(UnitManager callingAgent)
    {
        if (currentState.Actions.Length > 0 && Time.timeScale != 0)
        {
            foreach (Action act in currentState.Actions)
            {
                callingAgent.harveyManager.actionList.Add(act);
            }
        }
    }

    private void ScanForUnit()
    {
        // Clear Lists
        allied.Clear();
        enemies.Clear();

        // Add Own Units to List
        foreach (Transform t in transform) allied.Add(t);
        GameObject[] squads = GameObject.FindGameObjectsWithTag("Squad");

        // Iterate through all Squads
        foreach (GameObject squad in squads)
        {
            SquadInfluenceManager sim = squad.GetComponent<SquadInfluenceManager>();

            // Check if it is not us
            if (sim != this)
            {
                // Check if it is our Team and UNSHARED to get allied
                if (sim.style == InfluenceStyle.UNSHARED)
                {
                    foreach (Transform t in transform)
                    {
                        foreach (Transform _t in sim.gameObject.transform)
                        {
                            if (!allied.Contains(_t))
                            {
                                // Raycast Line of Sight
                                RaycastHit2D[] hits = new RaycastHit2D[2];
                                Physics2D.RaycastNonAlloc(t.position, _t.position - t.position, hits);

                                if (hits[1].collider.gameObject != t.gameObject && hits[1].collider.gameObject == _t.gameObject)
                                {
                                    if (showDebugRays) Debug.DrawRay(t.position, _t.position - t.position, Color.green);
                                    allied.Add(_t);
                                }
                                else if (showDebugRays)
                                    Debug.DrawRay(t.position, _t.position - t.position, Color.red);
                            }
                        }
                    }
                }

                // Check for Enemies
                if (sim.style != this.style)
                {
                    foreach (Transform t in transform)
                    {
                        foreach (Transform _t in sim.gameObject.transform)
                        {
                            if (!enemies.Contains(_t))
                            {
                                // Raycast Line of Sight
                                RaycastHit2D[] hits = new RaycastHit2D[2];
                                Physics2D.RaycastNonAlloc(t.position, _t.position - t.position, hits);

                                try
                                {
                                    if (hits[1].collider.gameObject != t.gameObject && hits[1].collider.gameObject == _t.gameObject)
                                    {
                                        if (showDebugRays) Debug.DrawRay(t.position, _t.position - t.position, Color.green);
                                        enemies.Add(_t);
                                    }
                                    else if (showDebugRays)
                                        Debug.DrawRay(t.position, _t.position - t.position, Color.red);
                                }
                                catch (System.Exception)
                                {

                                }
                            }
                        }
                    }
                }

                // Check for Target
                CheckForTarget(sim);
            }
        }

        map.allyPosition = allied;
        map.enemyPosition = enemies;
    }

    private void CheckForTarget(SquadInfluenceManager sim)
    {
        if (sim.style != this.style)
        {
            foreach (Transform t in transform)
            {
                List<Transform> enemyTransforms = new List<Transform>();

                foreach (Transform _t in sim.gameObject.transform)
                {
                    // Raycast Line of Sight
                    // Create Raycast
                    RaycastHit2D hit = Physics2D.Raycast(t.position, _t.position - t.position, 1000f, ignoreTargetLayerMask);

                    if (hit.collider != null && hit.collider.gameObject.layer == sim.gameObject.transform.GetChild(0).gameObject.layer)
                    {
                        enemyTransforms.Add(_t);
                    }

                    /*
                    RaycastHit2D[] hits = new RaycastHit2D[2];
                    Physics2D.RaycastNonAlloc(t.position, _t.position - t.position, hits);

                    if (hits[1].collider.gameObject != t.gameObject && hits[1].collider.gameObject == _t.gameObject)
                    {
                        enemyTransforms.Add(_t);
                    }
                    */
                }

                if (enemyTransforms.Count > 0)
                {
                    Transform tEnemy = t.GetComponent<UnitManager>()._Target;

                    foreach (Transform _t in enemyTransforms)
                    {
                        if (tEnemy != null)
                        {
                            if (Vector2.Distance(t.position, _t.position) < Vector2.Distance(t.position, tEnemy.position))
                                tEnemy = _t;
                        }
                        else tEnemy = _t;
                    }

                    t.GetComponent<UnitManager>()._Target = tEnemy;
                }
                else
                    t.GetComponent<UnitManager>()._Target = null;
            }
        }
    }

    private void Awake()
    {
        map = new Imap(InfuenceManager.Instance.CellSize, InfuenceManager.Instance.mapSize, InfuenceManager.Instance.offset);
        InfuenceManager.Instance.imaps.Add(map);

        squadUnits = GetComponentsInChildren<UnitManager>().ToList();
    }

    private void OnDrawGizmosSelected()
    {
        if (map == null) return;

        InfuenceManager.Instance.GizmoVisualisation(map);
    }
}
