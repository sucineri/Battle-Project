using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class ResourceBar : MonoBehaviour
{

    [SerializeField] private float _hpBarAnimationDuration = 0.5f;
    [SerializeField] private Slider _progressBar;

    public void Init(float currentHpPercentage)
    {
        _progressBar.minValue = 0f;
        _progressBar.maxValue = 1f;
        _progressBar.value = currentHpPercentage;
    }

    public IEnumerator AnimateValueChange(float toPercentage)
    {
        var timeElapsed = 0f;
        var current = _progressBar.value;
        while (timeElapsed < this._hpBarAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Min(timeElapsed / this._hpBarAnimationDuration, 1f);
            _progressBar.value = Mathf.Lerp(current, toPercentage, progress);
            yield return 0;
        }
        yield return 0;
    }
}
