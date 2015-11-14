using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class DamageText : MonoBehaviour
{

    private Text label;
    private float animationDuration = 0.5f;
    private float moveDistance = 3f;

    void Awake()
    {
        label = this.GetComponent<Text>();
    }

    public void ShowDamage(int damage)
    {
        label.text = damage.ToString();
        StartCoroutine(this.AnimateText());
    }

    private IEnumerator AnimateText()
    {
        var startPosition = this.transform.position;
        var destination = startPosition + new Vector3(0f, moveDistance, 0f);

        yield return StartCoroutine(AnimationService.MoveToPosition(this, this.transform, destination, animationDuration));
        Destroy(this.gameObject);
    }
}
