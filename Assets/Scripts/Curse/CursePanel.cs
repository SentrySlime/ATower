using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CursePanel : MonoBehaviour
{
    public TextMeshProUGUI conditionDescription;
    public TextMeshProUGUI conditionCount;
    public TextMeshProUGUI downSideDescription;

    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeBlessing());
    }

    IEnumerator FadeBlessing()
    {
        yield return new WaitForSeconds(2.5f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 1 * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
