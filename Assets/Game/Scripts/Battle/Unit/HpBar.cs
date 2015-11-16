using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Slider))]
public class HpBar : MonoBehaviour {

	private Slider _progressBar;

    void Awake() 
    {
        this._progressBar = this.GetComponent<Slider>();
    }

	public void Init(float currentHpPercentage)
    {
        _progressBar.minValue = 0f;
        _progressBar.maxValue = 1f;
		_progressBar.value = currentHpPercentage;
    }

    public IEnumerator AnimateValueChange(float toPercentage, float duration)
    {
        var timeElapsed = 0f;
        var current = _progressBar.value;
        while(timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Min(timeElapsed / duration, 1f);
			_progressBar.value = Mathf.Lerp(current, toPercentage, progress);
            yield return 0;
        }
        yield return 0;
    }
}
