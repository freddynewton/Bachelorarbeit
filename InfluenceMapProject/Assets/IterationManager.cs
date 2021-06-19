using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IterationManager : MonoBehaviour
{
    public static IterationManager Instance { get; private set; }

    [Header("Settings")]
    public string FileName = "InfluenceMapSheet";    
    public int IterationAmount = 1000;
    [HideInInspector] public int currentIteration = 0;

    public MatchData currentMatchData;

    private JsonParser jsonParser;

    [System.Serializable]
    public class MatchData
    {
        // Iteration Number
        public int id;

        // Time in Sec
        public float matchDuration;

        /// <summary>
        /// 0 = null
        /// 1 = Shared
        /// 2 = Unshared
        /// </summary>
        public int winner;

        /// <summary>
        /// Counts Where and How often Agents from the own team or from the enemies died
        /// </summary>
        public MatchDeath SharedMatchDeath;
        public MatchDeath UnsharedMatchDeath;

        /// <summary>
        /// Track When and How Long some points are hold
        /// </summary>
        public List<PointStateStatus> PointStatesVerlauf;

        /// <summary>
        /// Map
        /// </summary>
        //public Imap map;

        public List<PointGradient> PointVerlauf;

        /// <summary>
        /// Generator
        /// </summary>
        public MatchData()
        {
            Instance.currentIteration += 1;

            id = Instance.currentIteration;
            matchDuration = 0;
            winner = 0;
            SharedMatchDeath = new MatchDeath();
            UnsharedMatchDeath = new MatchDeath();
            PointStatesVerlauf = new List<PointStateStatus>();
            PointVerlauf = new List<PointGradient>();
            //map = new Imap(InfuenceManager.Instance.CellSize, InfuenceManager.Instance.mapSize, InfuenceManager.Instance.offset);
        }
    }

    [System.Serializable]
    public class PointGradient
    {
        public int SharedPoints;
        public int UnsharedPoints;
        public float MatchDuration;

        public PointGradient(int sharedPoints, int unsharedPoints)
        {
            SharedPoints = sharedPoints;
            UnsharedPoints = unsharedPoints;
            MatchDuration = GameController.Instance.MatchDuration;
        }
    }

    [System.Serializable]
    public class PointStateStatus
    {
        public GameController.POINTSTATE[] PointStates;
        public float MatchDuration;

        public PointStateStatus(GameController.POINTSTATE[] points)
        {
            PointStates = points;
            MatchDuration = GameController.Instance.MatchDuration;
        }
    }

    [System.Serializable]
    public class MatchDeath
    {
        public List<Vector2> DeathPositions;
        public List<Vector2> KillPositions;

        public MatchDeath()
        {
            DeathPositions = new List<Vector2>();
            KillPositions = new List<Vector2>();
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        jsonParser = new JsonParser(name);
        currentMatchData = new MatchData();
        jsonParser.MatchDatas.Add(currentMatchData);
    }

    public void RestartMatch()
    {
        currentMatchData.matchDuration = GameController.Instance.MatchDuration;

        if (GameController.Instance.SharedPoints >= 100) currentMatchData.winner = 1;
        else if (GameController.Instance.UnsharedPoints >= 100) currentMatchData.winner = 2;
        else currentMatchData.winner = 0;

        if (currentIteration <= IterationAmount)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            jsonParser.InitFile();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL("http://google.com");
#else
         Application.Quit();
#endif
        }

        currentMatchData = new MatchData();
        jsonParser.MatchDatas.Add(currentMatchData);
        Time.timeScale = GameController.Instance.gameSpeed;
    }
}
