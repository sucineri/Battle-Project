using UnityEngine;
using System.Collections;

public class AnimationHelper
{
    public static IEnumerator MoveToPosition(Transform transformToMove, Vector3 destination, float duration, float completion = 1f)
    {
        var startPosition = transformToMove.position;
        var timeElapsed = 0f;
        
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            var progress = Mathf.Clamp(timeElapsed / duration, 0f, completion);
            transformToMove.position = Vector3.Lerp(startPosition, destination, progress);
            yield return 0;
        }
    }
}
