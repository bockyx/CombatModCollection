﻿using System.Collections.Concurrent;
using System.Dynamic;
using System.Net;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{

    public class MapEventState
    {
        private static readonly FieldInfo MapEvent__mapEventUpdateCount = typeof(MapEvent).GetField(
            "_mapEventUpdateCount", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        private static readonly ConcurrentDictionary<MBGUID, MapEventState> AllMapEventStates = new ConcurrentDictionary<MBGUID, MapEventState>();

        public static MapEventState GetMapEventState(MapEvent mapEvent)
        {
            if (!AllMapEventStates.TryGetValue(mapEvent.Id, out MapEventState mapEventState))
            {
                mapEventState = new MapEventState();
                AllMapEventStates[mapEvent.Id] = mapEventState;
                mapEventState.StageRounds = (int)MapEvent__mapEventUpdateCount.GetValue(mapEvent);
            }         
            return mapEventState;
        }

        public static void RemoveMapEventState(MapEvent mapEvent)
        {
            AllMapEventStates.TryRemove(mapEvent.Id, out _);
        }


        private ConcurrentDictionary<MBGUID, TroopState> TroopStates = new ConcurrentDictionary<MBGUID, TroopState>();
        public int BattleScale = 2;
        public int StageRounds = 0;
        public bool IsDefenderRunAway = false;


        private TroopState GetTroopState(CharacterObject troop)
        {
            if (!TroopStates.TryGetValue(troop.Id, out TroopState troopState))
            {
                TroopStates[troop.Id] = new TroopState(troop);
                return TroopStates[troop.Id];
            }
            else
            {
                return troopState;
            }
        }

        public bool ApplyDamageToTroop(AttackComposition attack, CharacterObject troop, out float damage)
        {
            TroopState troopState = GetTroopState(troop);
            bool isFinishingBlow = troopState.TakeHit(attack, out damage);
            if (isFinishingBlow)
            {
                TroopStates.TryRemove(troop.Id, out _);
            }
            return isFinishingBlow;
        }

        public AttackComposition GetAttackPoints(CharacterObject troop)
        {
            if (SubModule.Settings.Battle_SendAllTroops_DetailedCombatModel)
            {
                TroopState troopState = GetTroopState(troop);
                troopState.PrepareWeapon(this);
                return troopState.DoAttack();
            } else
            {
                return new AttackComposition
                {
                    Melee = troop.GetPower()
                };
            }           
        }

        public float GetTroopStrength(CharacterObject troop)
        {
            TroopState troopState = GetTroopState(troop);
            return troopState.Hitpoints / troop.MaxHitPoints() * troop.GetPower();
        }
    }
}