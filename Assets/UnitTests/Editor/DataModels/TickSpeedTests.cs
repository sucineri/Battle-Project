using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BattleSaga.UnitTests
{
    [TestFixture]
    [Category("TickSpeedTests Tests")]
    internal class TickSpeedTests
    {
        [TestCase(0d, Result=28)]
        [TestCase(1d, Result=26)]
        [TestCase(4d, Result=20)]
        [TestCase(16d, Result=12)]
        [TestCase(28d, Result=9)]
        [TestCase(39d, Result=7)]
        [TestCase(62d, Result=5)]
        [TestCase(41d, Result=7)]
        [TestCase(89d, Result=5)]
        [TestCase(100d, Result=4)]
        [TestCase(180d, Result=3)]
        public int GetTickSpeed_PositionAndZeroAgility_ReturnsCorrectSpeed(double agility)
        {
            return TickSpeed.GetTickSpeed(agility);
        }

        [Test]
        public void GetTickSpeed_NegativeAgility_ReturnsZero()
        {
            var negativeAgility = -1d;

            var speed = TickSpeed.GetTickSpeed(negativeAgility);

            Assert.AreEqual(speed, 0);
        }
    }
}
