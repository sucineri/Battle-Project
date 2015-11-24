using UnityEngine;
using System;
using System.Collections;

public class PopupManager  
{
	public static void OkCancel(Action okAction, Action cancelAction)
	{
		var prefab = Resources.Load ("Popups/StandardPopup");
		var go = GameObject.Instantiate (prefab) as GameObject;
		go.SetActive (true);
		var controller = go.GetComponent<PopupController> ();
		controller.Show ("OK", "Cancel", okAction, cancelAction);
	}
}
