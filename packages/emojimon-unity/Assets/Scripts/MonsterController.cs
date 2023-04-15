using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Serializable]
    public struct MonsterInfo
    {
        public int ID;
        public GameObject Model;
    }

    [SerializeField]
    List<MonsterInfo> Monsters;

    [SerializeField]
    PlayerController Player;

    [SerializeField]
    float animTime = 1;
    private float _elapsedTime;

    protected void Start()
    {
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        if (gs.HasEncounter)
        {
            _elapsedTime = 0;
            transform.position = Player.transform.position;
            int id = gs.MonsterType;
            foreach (var info in Monsters)
            {
                if (info.ID == id)
                {
                    info.Model.SetActive(true);
                }
                else
                {
                    info.Model.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var info in Monsters)
            {
                info.Model.SetActive(false);
            }
        }
    }

    protected void Update()
    {
        _elapsedTime += Time.deltaTime;
        var t = Mathf.Clamp(_elapsedTime / animTime, 0, 1);
        var s = Easing.easeOutElastic(t);

        transform.localScale = new Vector3(s, s, s);
    }
}
