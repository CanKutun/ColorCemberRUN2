using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowAnimator : MonoBehaviour
{
    public void MoveTo(Vector2 targetPos, float duration = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateMove(targetPos, duration));
    }

    IEnumerator AnimateMove(Vector2 target, float time)
    {
        RectTransform rt = transform as RectTransform;
        Vector2 start = rt.anchoredPosition; 
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            rt.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rt.anchoredPosition = target;
    }
}
