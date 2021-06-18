using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;


public class GameController : Singleton<GameController>
{
    public enum POINTSTATE
    {
        SHARED,
        UNSHARED,
        NONE
    }

    public Text SharedText;
    public Text UnsharedText;
    public Text PointStateText;

    public int SharedPoints { get; private set; }
    public int UnsharedPoints { get; private set; }

    [Header("Settings")]
    public float UpdateFrequency = 5f;
    public int PointsPerUpgrade = 1;
    public float RespawnTime = 10f;
    [Range(0, 20)] public float gameSpeed = 1;

    public GameObject[] AgentPrefabs;

    [Header("Points")]
    public POINTSTATE[] pointstates;
    public PointArea[] pointAreas;

    private bool gameIsOver;

    private void Awake()
    {
        SharedPoints = 0;
        UnsharedPoints = 0;

        SharedText.text = "Shared: " + SharedPoints.ToString();
        UnsharedText.text = "Unshared: " + UnsharedPoints.ToString();

        pointstates = new POINTSTATE[3] { POINTSTATE.NONE, POINTSTATE.NONE, POINTSTATE.NONE };

        StartCoroutine(PointUpdate());

        StartGame();
    }

    private void LateUpdate()
    {
        if (!gameIsOver) Time.timeScale = gameSpeed;
    }

    private void StartGame()
    {
        float tmpRespawnTimer = RespawnTime;
        RespawnTime = 0;

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Squad"))
        {
            SquadInfluenceManager squad = obj.GetComponent<SquadInfluenceManager>();

            if (squad.style == SquadInfluenceManager.InfluenceStyle.SHARED)
            {
                for (int i = 0; i < 5; i++)
                {
                    StartRespawn(squad);
                }
            } else
            {
                StartRespawn(squad);
            }
        }

        RespawnTime = tmpRespawnTimer;
    }

    public void StartRespawn(SquadInfluenceManager squad)
    {
        StartCoroutine(Respawn(squad));
    }

    private IEnumerator Respawn(SquadInfluenceManager squad)
    {
        yield return new WaitForSeconds(RespawnTime);

        // Get Spawn Position
        bool foundPos = false;

        while (!foundPos)
        {
            int x = UnityEngine.Random.Range(0, squad.map._map.GetLength(0));
            int y = UnityEngine.Random.Range(0, squad.map._map.GetLength(1));

            Vector2 mapCell = squad.map._map[x, y];

            if (mapCell.y == 0)
            {
                RaycastHit2D hit = Physics2D.BoxCast(squad.map.getWorldPositionFromCell(x, y), Vector2.one, 0, Vector2.zero);

                // Check if Respawn Position is in Capturepoint
                if (!InfuenceManager.Instance.CheckIfCellIsInCapturePoint(x, y) && hit.collider == null)
                {
                    Vector2 Pos = squad.map.getWorldPositionFromCell(x, y);

                    Instantiate(squad.style == SquadInfluenceManager.InfluenceStyle.SHARED ? AgentPrefabs[0] : AgentPrefabs[1], Pos, Quaternion.identity, squad.transform);
                    Debug.Log("Spawning " + (squad.style == SquadInfluenceManager.InfluenceStyle.SHARED ? AgentPrefabs[0].name : AgentPrefabs[1].name) + " on " + Pos.ToString() + " Position");
                    foundPos = true;
                }
            }
        }
    }

    private bool ControlPoints()
    {
        gameIsOver = false;

        // STOP GAME
        if (SharedPoints >= 100 || UnsharedPoints >= 100)
        {
            Time.timeScale = 0;
            gameIsOver = true;
        }

        // DRAW INTO JSON

        // RESTART SCENE

        return gameIsOver;
    }

    public IEnumerator PointUpdate()
    {
        yield return new WaitForSeconds(UpdateFrequency);

        foreach (POINTSTATE p in pointstates)
        {
            if (p == POINTSTATE.SHARED) SharedPoints += PointsPerUpgrade;
            else if (p == POINTSTATE.UNSHARED) UnsharedPoints += PointsPerUpgrade;
        }

        SharedText.text = "Shared: " + SharedPoints.ToString();
        UnsharedText.text = "Unshared: " + UnsharedPoints.ToString();

        if (!ControlPoints())
            StartCoroutine(PointUpdate());
    }

    public void UpdatePointStateText()
    {
        string t = "|";

        foreach (POINTSTATE state in pointstates)
        {
            switch (state)
            {
                case POINTSTATE.SHARED:
                    t += " S |";
                    break;
                case POINTSTATE.UNSHARED:
                    t += " U |";
                    break;
                case POINTSTATE.NONE:
                    t += " X |";
                    break;
            }
        }

        PointStateText.text = t;
    }
}
