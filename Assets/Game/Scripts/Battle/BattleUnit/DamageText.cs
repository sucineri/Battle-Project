using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class DamageText : MonoBehaviour
{

    private Text label;
    private float animationDuration = 0.5f;
    private float moveDistance = 3f;
    private Color negativeEffectColor = Color.red;
    private Color positiveEffectColor = Color.green;

    void Awake()
    {
        label = this.GetComponent<Text>();
        label.enabled = false;
    }

    public void ShowText(string text, bool negativeEffect, bool showCritical, float delay)
    {
        label.text = text;
        label.color = negativeEffect ? this.negativeEffectColor : this.positiveEffectColor;
      
        if (showCritical)
        {
            label.fontStyle = FontStyle.Bold;
            label.fontSize = label.fontSize + 4;
        }                     

        StartCoroutine(this.AnimateText(delay));
    }

    private IEnumerator AnimateText(float delay)
    {
        yield return new WaitForSeconds(delay);
        label.enabled = true;
        var startPosition = this.transform.position;
        var destination = startPosition + new Vector3(0f, moveDistance, 0f);

        yield return StartCoroutine(AnimationHelper.MoveToPosition(this.transform, destination, animationDuration));
        Destroy(this.gameObject);
    }
}
