using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BattleEnmity  
{
    private List<EnmityTarget> _enmityList = new List<EnmityTarget>();

    public void InitEnmityList(List<BattleCharacter> enemies, int initEnmityLevel)
    {
        this._enmityList = new List<EnmityTarget>();
        foreach (var enemy in enemies)
        {
            this._enmityList.Add(new EnmityTarget(enemy, initEnmityLevel));
        }
        this.ArrangeList();
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
        this.ArrangeList();
    }

    public List<EnmityTarget> GetEnmityList()
    {
        return this._enmityList;
    }

    public void ArrangeList()
    {
        this._enmityList.Sort();
    }

    public class EnmityTarget : IComparable
    {
        public BattleCharacter Character { get; private set; }
        public int EnmityLevel { get; set; }

        public EnmityTarget(BattleCharacter character, int enmityLevel)
        {
            this.Character = character;
            this.EnmityLevel = enmityLevel;
        }

        #region IComparable implementation
        public int CompareTo(object obj)
        {
            var otherEnmity = obj as EnmityTarget;
            if (otherEnmity != null)
            {
                return otherEnmity.EnmityLevel.CompareTo(this.EnmityLevel);
            }
            return -1;
        }
        #endregion
    }
}
