using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TurnOrderData = BattleTurnOrderModel.TurnOrderData;

public class TurnOrderView : MonoBehaviour
{

    [SerializeField]
    private HorizontalLayoutGroup layout;
    [SerializeField]
    private GameObject cellPrefab;

    private List<TurnOrderCell> cells = new List<TurnOrderCell>();

    public void UpdateView(List<TurnOrderData> turnOrder)
    {
        // reuse cells
        for (int i = 0; i < cells.Count; ++i)
        {
            var cell = cells[i];
            BattleCharacter character = null;
            if (i < turnOrder.Count)
            {
                character = turnOrder[i].character;
                cell.Setup(character);
                cell.gameObject.SetActive(true);
            }
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        // create cells if we don't have enough
        if (turnOrder.Count > cells.Count)
        {
            for (int i = cells.Count; i < turnOrder.Count; ++i)
            {
                this.CreateCell(turnOrder[i].character);
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
