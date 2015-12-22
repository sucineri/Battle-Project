using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderView : MonoBehaviour
{

    [SerializeField]
    private HorizontalLayoutGroup layout;
    [SerializeField]
    private GameObject cellPrefab;

    private List<TurnOrderCell> cells = new List<TurnOrderCell>();

    public void UpdateView(List<BattleCharacter> actionList)
    {
        // reuse cells
        for (int i = 0; i < cells.Count; ++i)
        {
            var cell = cells[i];
            BattleCharacter character = null;
            if (i < actionList.Count)
            {
                character = actionList[i];
                cell.Setup(character);
                cell.gameObject.SetActive(true);
            }
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        // create cells if we don't have enough
        if (actionList.Count > cells.Count)
        {
            for (int i = cells.Count; i < actionList.Count; ++i)
            {
                this.CreateCell(actionList[i]);
            }
        }
    }

    private void CreateCell(BattleCharacter character)
    {
        var go = Instantiate(cellPrefab) as GameObject;
        var cell = go.GetComponent<TurnOrderCell>();
        cell.Setup(character);
        cell.gameObject.SetActive(true);
        cell.transform.SetParent(this.layout.transform);
        cell.transform.localScale = Vector3.one;
        cells.Add(cell);
    }
}
