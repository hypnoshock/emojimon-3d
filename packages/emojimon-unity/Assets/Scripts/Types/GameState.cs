using System.Collections.Generic;

// JSON: {"playerPosition":{"x":9,"y":11},"canSpawn":false,"otherPlayers":[{"entity":140,"position":{"x":5,"y":5}}],"encounter":{"0":"2","actionCount":"2","monsters":["0xc8e249ee5364aa9d18cccb5cf6aaaeef1d9519312e37956146914625a2581951"]},"hasEncounter":true}
public class GameState
{
    [Newtonsoft.Json.JsonProperty(
        "playerPosition",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public Position PlayerPosition { get; set; }

    [Newtonsoft.Json.JsonProperty(
        "canSpawn",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public bool CanSpawn { get; set; }

    [Newtonsoft.Json.JsonProperty(
        "otherPlayers",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public List<OtherPlayer> OtherPlayers;

    [Newtonsoft.Json.JsonProperty(
        "encounter",
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public Encounter Encounter;

    [Newtonsoft.Json.JsonProperty(
        "hasEncounter",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public bool HasEncounter;

    [Newtonsoft.Json.JsonProperty(
        "map",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public Map Map;
}

// "map":{"width":20,"height":20,"terrainValues":[{"x":0,"y":0,"value":0,"type":null}]
public class Map
{
    [Newtonsoft.Json.JsonProperty(
        "width",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int Width;

    [Newtonsoft.Json.JsonProperty(
        "height",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int Height;

    [Newtonsoft.Json.JsonProperty(
        "terrainValues",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public List<TerrainValue> TerrainValues;
}

public class TerrainValue
{
    [Newtonsoft.Json.JsonProperty(
        "x",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int X;

    [Newtonsoft.Json.JsonProperty(
        "y",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int Y;
    public int Value;
}

public class Encounter
{
    [Newtonsoft.Json.JsonProperty(
        "monsters",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public List<string> Monsters;

    [Newtonsoft.Json.JsonProperty(
        "actionCount",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int ActionCount;
}

public struct Position
{
    [Newtonsoft.Json.JsonProperty(
        "x",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int X;

    [Newtonsoft.Json.JsonProperty(
        "y",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int Y;
}

public struct OtherPlayer
{
    [Newtonsoft.Json.JsonProperty(
        "entity",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public int Entity;

    [Newtonsoft.Json.JsonProperty(
        "position",
        Required = Newtonsoft.Json.Required.DisallowNull,
        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
    )]
    public Position Position;
}
