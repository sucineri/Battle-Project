using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BattleSaga.UnitTests
{
    [TestFixture]
    [Category("BattleCharacter Tests")]
    internal class BattleCharacterTests
    {
        [TestCase(Const.Stats.MaxHp, Result = 100d)]
        [TestCase(Const.Stats.MaxMp, Result = 0d)]
        [TestCase(Const.Stats.FireResistance, Result = 1d)]
        [TestCase(Const.Stats.IceResistance, Result = 0d)]
        public double GetStat_ValidStatEnum_ReturnsCorrectStat(Const.Stats stat)
        {
            var maxHp = 100d;
            var fireResistance = 1d;
            var baseStat = new CharacterStats();
            baseStat.SetStat(Const.Stats.MaxHp, maxHp);
            baseStat.SetStat(Const.Stats.FireResistance, fireResistance);

            var character = new Character();
            character.BaseStats = baseStat;

            var battleCharacter = new BattleCharacter(character, Const.Team.Player);

            return battleCharacter.GetStat(stat);
        }

        [TestCase(Const.Affinities.Healing, Result = 2d)]
        [TestCase(Const.Affinities.Fire, Result = -1d)]
        [TestCase(Const.Affinities.Ice, Result = 0d)]
        [TestCase(Const.Affinities.Holy, Result = 0d)]
        public double GetAffinityResistance_ValidAffinityEnum_ReturnsResistanceStat(Const.Affinities affinity)
        {
            var healingResistance = 2d;
            var fireResistance = -1d;
            var baseStat = new CharacterStats();
            baseStat.SetStat(Const.Stats.HealingResistance, healingResistance);
            baseStat.SetStat(Const.Stats.FireResistance, fireResistance);

            var character = new Character();
            character.BaseStats = baseStat;

            var battleCharacter = new BattleCharacter(character, Const.Team.Player);

            return battleCharacter.GetAffinityResistance(affinity);
        }
    }
}