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

    [SerializeField] List<MonsterInfo> Monsters;
    [SerializeField] PlayerController Player;
    protected void Start()
    {
        GameStateMediator.Instance.EventStateUpdated += OnStateUpdate;
    }

    private void OnStateUpdate(GameState gs)
    {
        if (gs.HasEncounter)
        {
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
}
