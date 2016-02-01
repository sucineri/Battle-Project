using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BattleEnmity  
{
    private List<CharacterEnmity> _enmityList = new List<CharacterEnmity>();

    public void InitEnmityList(List<BattleCharacter> enemies, int initEnmityLevel)
    {
        this._enmityList = new List<CharacterEnmity>();
        foreach (var enemy in enemies)
        {
            this._enmityList.Add(new CharacterEnmity(enemy, initEnmityLevel));
        }
    }

    public void ChangeEnmityLevel(BattleCharacter character, int enmityDelta)
    {
        foreach (var e in this._enmityList)
        {
            if (e.Character == character)
            {
                e.EnmityLevel += enmityDelta;
                break;
            }
        }
    }

    public void ArrangeList()
    {
        this._enmityList.Sort();
    }

    internal class CharacterEnmity : IComparable
    {
        public BattleCharacter Character { get; private set; }
        public int EnmityLevel { get; set; }

        public CharacterEnmity(BattleCharacter character, int enmityLevel)
        {
            this.Character = character;
            this.EnmityLevel = enmityLevel;
        }

        #region IComparable implementation
        public int CompareTo(object obj)
        {
            var otherEnmity = obj as CharacterEnmity;
            if (otherEnmity != null)
            {
                return otherEnmity.EnmityLevel.CompareTo(this.EnmityLevel);
            }
            return -1;
        }
        #endregion
    }
}
