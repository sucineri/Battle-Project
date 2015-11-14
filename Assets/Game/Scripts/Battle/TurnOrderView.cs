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

    public void ShowOrder(List<UnitControllerBase> orderedUnites)
    {
        // reuse cells
        for (int i = 0; i < cells.Count; ++i)
        {
            var cell = cells [i];
            UnitControllerBase unit = null;
            if (i < orderedUnites.Count)
            {
                unit = orderedUnites [i];
                cell.Setup(unit);
                cell.gameObject.SetActive(true);
            } else
            {
                cell.gameObject.SetActive(false);
            }
        }

        // create cells if we don't have enough
        if (orderedUnites.Count > cells.Count)
        {
            for (int i = cells.Count; i < orderedUnites.Count; ++i)
            {
                CreateCell(orderedUnites [i]);
            }
        }
    }

    private void CreateCell(UnitControllerBase unit)
    {
        var go = Instantiate(cellPrefab) as GameObject;
        var cell = go.GetComponent<TurnOrderCell>();
        cell.Setup(unit);
        cell.gameObject.SetActive(true);
        cell.transform.SetParent(this.layout.transform);
        cell.transform.localScale = Vector3.one;
        cells.Add(cell);
    }
}
