using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class InfuenceManager : Singleton<InfuenceManager>
{
    [Serializable]
    public struct CapturePoint
    {
        public Vector2Int StartPoint;
        public Vector2Int Size;
    }

    [Range(0, 1)] public float Momentum = 0.5f;
    [Range(0, 1)] public float Decay = 0.5f;
    [Range(0, 1)] public float FrequenzySec = 1;
    [Range(0, 1)] public float CellSize = 0.5f;

    [Header("Map Settings")]
    public Vector2Int mapSize;
    public Vector2 offset;
    public CapturePoint[] capturePoints;

    public List<Tilemap> tilemaps;

    [Header("Blurr")]
    [Tooltip("Needs to be a odd number")] public int BlurRadius = 3;

    [Header("Gizmosettings")]
    public bool showGizmos = true;
    [Range(0, 1)] public float Gizmoalpha = 0.5f;

    public List<Imap> imaps = new List<Imap>();
    private void Awake()
    {
        // imaps.Add(new Imap(CellSize, mapSize, offset));
        StartCoroutine(UpdateFrequency());
    }

    public bool CheckIfCellIsInCapturePoint(int x, int y)
    {
        bool isInPoint = false;

        foreach (CapturePoint p in capturePoints)
        {
            if (p.StartPoint.x < x && p.StartPoint.x + p.Size.x > x && p.StartPoint.y < y && p.StartPoint.y + p.Size.y > y) isInPoint = true;
        }

        return isInPoint;
    }


    public void MapFlooding()
    {
        if (imaps.Count == 0) return;

        foreach (Imap map in imaps)
        {
            // Set Enemy and Ally Inf
            if (map.allyPosition != null)
            {
                foreach (Transform t in map.allyPosition)
                {
                    if (t != null)
                        map.setWorldPositionToCell(t.position.x, t.position.y, 1f);
                }
            }

            if (map.enemyPosition != null)
            {
                foreach (Transform t in map.enemyPosition)
                {
                    if (t != null)
                        map.setWorldPositionToCell(t.position.x, t.position.y, -1f);
                }
            }

            float[] avgInfPoint = new float[3] { 0, 0, 0 };
            float[] avgInfPointCount = new float[3] { 0, 0, 0 };


            // Iterate through map
            for (int x = 0; x < map._map.GetLength(0); x++)
            {
                for (int y = 0; y < map._map.GetLength(1); y++)
                {
                    // Check if point is walkable
                    if (map._map[x, y].y == 0)
                    {
                        // Calculate Inf
                        float inf = 0;
                        float count = 0;

                        // mean blur
                        for (int kx = -((BlurRadius - 1) / 2); kx < ((BlurRadius - 1) / 2) + 1; kx++)
                        {
                            for (int ky = -((BlurRadius - 1) / 2); ky < ((BlurRadius - 1) / 2) + 1; ky++)
                            {
                                if (y + ky < 0 || y + ky > map._map.GetLength(1) - 1 || x + kx < 0 || x + kx > map._map.GetLength(0) - 1) continue;
                                else
                                {
                                    count++;
                                    inf += map._map[x + kx, y + ky].x;
                                }
                            }
                        }

                        // Set Inf
                        if (count != 0)
                            map._map[x, y].x = (inf / count) * Decay;


                        // Set extreme Influence Points
                        if (inf > map._map[map.highestInfluencePos.x, map.highestInfluencePos.y].x) map.highestInfluencePos = new Vector2Int(x, y);
                        if (inf < map._map[map.lowestInfluencePos.x, map.lowestInfluencePos.y].x) map.lowestInfluencePos = new Vector2Int(x, y);

                        // Set Average Point Influence
                        for (int i = 0; i < capturePoints.Length; i++)
                        {
                            if (x >= capturePoints[i].StartPoint.x && x <= capturePoints[i].StartPoint.x + capturePoints[i].Size.x && y >= capturePoints[i].StartPoint.y && y <= capturePoints[i].StartPoint.y + capturePoints[i].Size.y)
                            {
                                avgInfPoint[i] += map._map[x, y].x;
                                avgInfPointCount[i] += 1;
                            }
                        }
                    }
                    else
                    {
                        map._map[x, y].x = 0;
                    }
                }
            }

            for (int i = 0; i < capturePoints.Length; i++)
            {
                avgInfPoint[i] = avgInfPoint[i] / avgInfPointCount[i];
                // Debug.Log(avgInfPoint[i]);
            }

            map.pointAverageInfluence = avgInfPoint;
        }
    }

    public void BilinearInterpolation()
    {
        foreach (Imap map in imaps)
        {
            // Set Enemy and Ally Inf
            foreach (Transform t in map.allyPosition) map.setWorldPositionToCell(t.position.x, t.position.y, 1f);
            foreach (Transform t in map.enemyPosition) map.setWorldPositionToCell(t.position.x, t.position.y, -1f);

            int xRowCount = map._map.GetLength(0);
            int yRowCount = map._map.GetLength(1);

            // Iterate through map
            for (int x = 0; x < xRowCount; x++)
            {
                for (int y = 0; y < yRowCount; y++)
                {
                    // Check if point is walkable
                    if (map._map[x, y].y == 0)
                    {
                        float c00;
                        float c10;
                        float c01;
                        float c11;

                        if (x >= 0 && x <= xRowCount && y >= 0 && y <= yRowCount) c00 = map._map[x, y].x;
                        if (x + 1 >= 0 && x + 1 <= xRowCount && y >= 0 && y <= yRowCount) c10 = map._map[x + 1, y].x;
                        if (x >= 0 && x <= xRowCount && y + 1 >= 0 && y + 1 <= yRowCount) c01 = map._map[x, y + 1].x;
                        if (x + 1 >= 0 && x + 1 <= xRowCount && y + 1 >= 0 && y + 1 <= yRowCount) c11 = map._map[x + 1, y + 1].x;
                    }
                }
            }
        }
    }

    private IEnumerator UpdateFrequency()
    {
        yield return new WaitForSecondsRealtime(FrequenzySec);

        for (int i = 0; i < imaps.Count; i++)
        {
            MapFlooding();
        }

        StartCoroutine(UpdateFrequency());
    }

    public void GizmoVisualisation(Imap imap)
    {
        if (imap == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)mapSize * 0.5f + offset, new Vector3(mapSize.x, mapSize.y, 0)); ;

        for (int x = 0; x < imap._map.GetLength(0); x++)
        {
            for (int y = 0; y < imap._map.GetLength(1); y++)
            {
                if (imap._map[x, y].y == 0)
                {
                    if (Vector2.Distance(worldPosition, imap.getWorldPositionFromCell(x, y)) < CellSize / 2) Handles.Label(worldPosition + new Vector2(1, 1), imap._map[x, y].x.ToString());

                    if (imap._map[x, y].x <= 0) Gizmos.color = new Color(Mathf.Lerp(0, 100, imap._map[x, y].x * -1), 0, 0, Gizmoalpha);
                    else Gizmos.color = new Color(0, 0, Mathf.Lerp(0, 100, imap._map[x, y].x), Gizmoalpha);

                    Gizmos.DrawCube(imap.getWorldPositionFromCell(x, y), Vector3.one * CellSize);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube((Vector2)mapSize * 0.5f + offset, new Vector3(mapSize.x, mapSize.y, 0));

        if (imaps.Count != 0)
        {
            foreach (CapturePoint p in capturePoints)
            {
                Vector2 pos00 = imaps[0].getWorldPositionFromCell(p.StartPoint.x, p.StartPoint.y);
                Vector2 pos01 = imaps[0].getWorldPositionFromCell(p.StartPoint.x, p.StartPoint.y + p.Size.y);
                Vector2 pos10 = imaps[0].getWorldPositionFromCell(p.StartPoint.x + p.Size.x, p.StartPoint.y);
                Vector2 pos11 = imaps[0].getWorldPositionFromCell(p.StartPoint.x + p.Size.x, p.StartPoint.y + p.Size.y);

                Gizmos.DrawLine(pos00, pos01);
                Gizmos.DrawLine(pos00, pos10);
                Gizmos.DrawLine(pos11, pos10);
                Gizmos.DrawLine(pos11, pos01);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        if (imaps.Count == 0) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        foreach (Imap imap in imaps)
        {
            for (int x = 0; x < imap._map.GetLength(0); x++)
            {
                for (int y = 0; y < imap._map.GetLength(1); y++)
                {
                    if (imap._map[x, y].y == 0)
                    {
                        if (Vector2.Distance(worldPosition, imap.getWorldPositionFromCell(x, y)) < CellSize / 2) Handles.Label(worldPosition + new Vector2(1, 1), imap._map[x, y].x.ToString());

                        if (imap._map[x, y].x <= 0) Gizmos.color = new Color(Mathf.Lerp(0, 100, imap._map[x, y].x * -1), 0, 0);
                        else Gizmos.color = new Color(0, 0, Mathf.Lerp(0, 100, imap._map[x, y].x));

                        Gizmos.DrawCube(imap.getWorldPositionFromCell(x, y), Vector3.one * CellSize);
                    }
                }
            }
        }
    }
}

