using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Slider))]
public class HpBar : MonoBehaviour {

    private Slider progressBar;

    private float animationDuration = 0.5f;

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

    public IEnumerator AnimateValueChange(float to)
    {
        var timeElapsed = 0f;
        var current = progressBar.value;
        while(timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Min(timeElapsed / animationDuration, 1f);
            progressBar.value = Mathf.Lerp(current, to, progress);
            yield return 0;
        }
        yield return 0;
    }
}
