using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Slider))]
public class HpBar : MonoBehaviour {

    private Slider progressBar;

    void Awake() 
    {
        this.progressBar = this.GetComponent<Slider>();
    }

    public void Init(float min, float max, float current)
    {
        progressBar.minValue = min;
        progressBar.maxValue = max;
        progressBar.value = current;
    }

    public IEnumerator AnimateValueChange(float to, float duration)
    {
        var timeElapsed = 0f;
        var current = progressBar.value;
        while(timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Min(timeElapsed / duration, 1f);
            progressBar.value = Mathf.Lerp(current, to, progress);
            yield return 0;
        }
        yield return 0;
    }
}
