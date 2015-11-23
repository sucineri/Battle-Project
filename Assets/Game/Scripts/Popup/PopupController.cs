using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PopupController : MonoBehaviour {

	[SerializeField] private Text _label1;
	[SerializeField] private Text _label2;

	private Action _action1;
	private Action _action2;
	private bool _finish = false;

	public IEnumerator ShowCoroutine(String option1, String option2, Action action1, Action action2)
	{
		this._label1.text = option1;
		this._label2.text = option2;

		this._action1 = action1;
		this._action2 = action2;

		while (!_finish) {
			yield return 0;
		}
		Destroy (this.gameObject);
	}

	public void OnButton1 ()
	{
		if (this._action1 != null) {
			this._action1 ();
		}
		this._finish = true;
	}

	public void OnButton2 ()
	{
		if (this._action2 != null) {
			this._action2 ();
		}
		this._finish = true;
	}
}
