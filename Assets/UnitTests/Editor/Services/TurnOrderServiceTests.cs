using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BattleSaga.UnitTests
{
    [TestFixture]
    [Category("TurnOrderServiceTests Tests")]
    internal class TurnOrderServiceTests
    {
        private List<BattleCharacter> _battleCharacters;

        [SetUp]
        public void Setup()
        {
            this._battleCharacters = this.CreateTestBattleCharacters();
            this._battleCharacters[0].ActionCooldown = 100;
            this._battleCharacters[1].ActionCooldown = 500;
            this._battleCharacters[2].ActionCooldown = 20;
            this._battleCharacters[3].ActionCooldown = 80;
        }

        [TestCase(100d, 3, Result=12)]
        [TestCase(100d, 4, Result=16)]
        [TestCase(170d, 3, Result=9)]
        [TestCase(15d, 5, Result=60)]
        public int ApplySkillCooldownToCharacter_CharacterCooldownUpdatedCorrectly(double agi, int rank)
        {
            var character = this.CreateCharacterWithAgi(agi);
            var battleCharacter = new BattleCharacter(character, Const.Team.Player);
            var skill = this.CreateSkillWithRank(rank);

            var service = new TurnOrderService();
            service.ApplySkillCooldownToCharacter(battleCharacter, skill);

            return battleCharacter.ActionCooldown;
        }

        [TestCase(100d, Result=12)]
        [TestCase(69d, Result=15)]
        [TestCase(170d, Result=9)]
        [TestCase(15d, Result=36)]
        public int ApplyDefaultCooldownToCharacter_CharacterCooldownUpdatedCorrectly(double agi)
        {
            var character = this.CreateCharacterWithAgi(agi);
            var battleCharacter = new BattleCharacter(character, Const.Team.Player);

            var service = new TurnOrderService();
            service.ApplyDefaultCooldownToCharacter(battleCharacter);

            return battleCharacter.ActionCooldown;
        }

        [Test]
        public void GetActionOrder_ReturnsListOrderedByCooldwon()
        {
            var service = new TurnOrderService();
            var orderedList = service.GetActionOrder(this._battleCharacters);

            Assert.AreEqual(orderedList[0].BattleCharacterId, 3);
            Assert.AreEqual(orderedList[1].BattleCharacterId, 4);
            Assert.AreEqual(orderedList[2].BattleCharacterId, 1);
            Assert.AreEqual(orderedList[3].BattleCharacterId, 2);
        }

        [Test]
        public void GetActionOrder_MinCooldownSubtractedForAllCharacters()
        {
            var service = new TurnOrderService();
            var orderedList = service.GetActionOrder(this._battleCharacters);

            Assert.AreEqual(orderedList[0].ActionCooldown, 0);
            Assert.AreEqual(orderedList[1].ActionCooldown, 60);
            Assert.AreEqual(orderedList[2].ActionCooldown, 80);
            Assert.AreEqual(orderedList[3].ActionCooldown, 480);
        }

        [Test]
        public void GetActionOrder_IgnoresDeadCharacter()
        {
            this._battleCharacters[0].CurrentHp = 0;

            var service = new TurnOrderService();
            var orderedList = service.GetActionOrder(this._battleCharacters);

            Assert.AreEqual(orderedList.Count, 3);
            Assert.AreEqual(orderedList[0].BattleCharacterId, 3);
            Assert.AreEqual(orderedList[1].BattleCharacterId, 4);
            Assert.AreEqual(orderedList[2].BattleCharacterId, 2);
        }

        private List<BattleCharacter> CreateTestBattleCharacters()
        {
            var list = new List<BattleCharacter>();

            var char1 = this.CreateCharacterWithAgi(100d);
            var char2 = this.CreateCharacterWithAgi(100d);
            var char3 = this.CreateCharacterWithAgi(29d);
            var char4 = this.CreateCharacterWithAgi(62d);

            var battleChar1 = new BattleCharacter(char1, Const.Team.Player);
            battleChar1.BattleCharacterId = 1;

            var battleChar2 = new BattleCharacter(char2, Const.Team.Player);
            battleChar2.BattleCharacterId = 2;

            var battleChar3 = new BattleCharacter(char3, Const.Team.Player);
            battleChar3.BattleCharacterId = 3;

            var battleChar4 = new BattleCharacter(char4, Const.Team.Player);
            battleChar4.BattleCharacterId = 4;

            list.Add(battleChar1);
            list.Add(battleChar2);
            list.Add(battleChar3);
            list.Add(battleChar4);

            return list;
        }

        private Character CreateCharacterWithAgi(double agi)
        {
            var stats = new BasicStats(100d, 100d, 100d, 100d, agi, 100d);
            var character = new Character();
            character.BasicStats = stats;
            return character;
        }

        private Skill CreateSkillWithRank(int rank)
        {
            var skill = new Skill();
            skill.Rank = rank;
            return skill;
        }
    }
}