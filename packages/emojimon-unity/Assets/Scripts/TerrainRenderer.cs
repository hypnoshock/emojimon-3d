using UnityEngine;
using System.Collections.Generic;

class TerrainObject
{
    public TerrainValue TerrainValue;
    public GameObject Model;

    public Renderer Rend;

    public TerrainObject(TerrainValue terrainValue, GameObject model)
    {
        TerrainValue = terrainValue;
        Model = model;
        Rend = model.GetComponentInChildren<Renderer>();
    }
}

public class TerrainRenderer : MonoBehaviour
{
    public GameObject RockPrefab;
    public GameObject TreePrefab;

    private float fromAlpha = 0;
    private float targetAlpha = 0;
    private float currrentAlpha = 0;
    public float lerpTimeSecs = 1f;
    public float elapsedTime = 0;

    private Dictionary<Position, TerrainObject> _terrainObjs;

    protected void Start()
    {
        _terrainObjs = new Dictionary<Position, TerrainObject>();

        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    protected void Update()
    {
        elapsedTime += Time.deltaTime;
        var t = elapsedTime / lerpTimeSecs;

        currrentAlpha = Mathf.Lerp(fromAlpha, targetAlpha, t);

        SetAlpha(currrentAlpha);
    }

    private void OnStateUpdate(GameState gs)
    {
        UpdateTiles(gs.Map);

        if (gs.HasEncounter && targetAlpha != 0)
        {
            targetAlpha = 0;
            fromAlpha = 1;
            elapsedTime = 0;
        }
        else if (targetAlpha != 1)
        {
            targetAlpha = 1;
            fromAlpha = 0;
            elapsedTime = 0;
        }
    }

    private void UpdateTiles(Map map)
    {
        foreach (var tv in map.TerrainValues)
        {
            Position pos = new Position() { X = tv.X, Y = tv.Y };
            if (_terrainObjs.ContainsKey(pos))
            {
                // If the terrain object has changed then delete the existing obj and instantiate a new one
                var terrainObj = _terrainObjs[pos];
                if (terrainObj.TerrainValue.Value != tv.Value)
                {
                    GameObject.Destroy(terrainObj.Model);
                    _terrainObjs.Remove(pos);
                }
                else
                {
                    // No need to update
                    continue;
                }
            }

            var prefab = GetTerrainPrefab(tv.Value);
            if (prefab == null)
                continue;

            var model = Instantiate(prefab, this.transform, false);
            model.transform.SetLocalPositionAndRotation(
                new Vector3(tv.X, 0, -tv.Y),
                Quaternion.identity
            );

            var to = new TerrainObject(tv, model);
            _terrainObjs[pos] = to;
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (var kvp in _terrainObjs)
        {
            Color color = kvp.Value.Rend.material.color;
            color.a = alpha;
            kvp.Value.Rend.material.color = color;
        }
    }

    private GameObject GetTerrainPrefab(int val)
    {
        switch (val)
        {
            case 1:
                return TreePrefab;
            case 2:
                return RockPrefab;
        }

        return null;
    }
}
