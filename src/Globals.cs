﻿using System.Collections.Concurrent;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CombatModCollection
{
    public class TroopStat
    {
        public float Hitpoints;
    }

    public class MapEventStat
    {
        public ConcurrentDictionary<UniqueTroopDescriptor, TroopStat> TroopStats = new ConcurrentDictionary<UniqueTroopDescriptor, TroopStat>();
        public int StageRounds = 0;
    }

    public class GlobalStorage
    {
        public static ConcurrentDictionary<MBGUID, bool> IsDefenderRunAway = new ConcurrentDictionary<MBGUID, bool>();
        public static ConcurrentDictionary<MBGUID, MapEventStat> MapEventStats = new ConcurrentDictionary<MBGUID, MapEventStat>();
    }
}