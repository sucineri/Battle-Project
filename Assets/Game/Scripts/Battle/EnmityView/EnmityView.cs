using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnmityView : MonoBehaviour 
{

    [SerializeField]
    private Transform _layout;
    [SerializeField]
    private GameObject _cellPrefab;

    private List<EnmityDisplayCell> _cells = new List<EnmityDisplayCell>();

    public void UpdateView(BattleCharacter battleCharacter)
    {
        if (battleCharacter == null)
        {
            return;
        }

        var list = battleCharacter.Enmity.GetEnmityList();

        // reuse cells
        for (int i = 0; i < _cells.Count; ++i)
        {
            var cell = _cells[i];
            BattleEnmity.EnmityTarget characterEnmity = null;
            if (i < list.Count)
            {
                characterEnmity = list[i];
                cell.Setup(characterEnmity);
                cell.gameObject.SetActive(true);
            }
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        // create cells if we don't have enough
        if (list.Count > _cells.Count)
        {
            for (int i = _cells.Count; i < list.Count; ++i)
            {
                this.CreateCell(list[i]);
            }
        }
    }

    private void CreateCell(BattleEnmity.EnmityTarget characterEnmity)
    {
        var go = Instantiate(_cellPrefab) as GameObject;
        var cell = go.GetComponent<EnmityDisplayCell>();
        cell.Setup(characterEnmity);
        cell.gameObject.SetActive(true);
        cell.transform.SetParent(this._layout);
        cell.transform.localScale = Vector3.one;
        _cells.Add(cell);
    }
}
