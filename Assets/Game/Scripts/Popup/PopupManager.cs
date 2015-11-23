using UnityEngine;
using System;
using System.Collections;

public class PopupManager  
{
	public static IEnumerator OkCancel(Action okAction, Action cancelAction)
	{
		var prefab = Resources.Load ("Popup/StandardPopup");
		var go = GameObject.Instantiate (prefab) as GameObject;
		var controller = go.GetComponent<PopupController> ();
		return controller.ShowCoroutine ("OK", "Cancel", okAction, cancelAction);
	}
}
