using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicatorScreenSpacedUIElement : ScreenSpacedUIElement
{
    [SerializeField, Tooltip("How long the Damage Indicator stays")]
    public float TimeToLive = 2.5f;

    [SerializeField, Tooltip("The amount to animate upwards")]
    public float MoveUpAmount = 10;

    /// <summary>
    ///  A reference to the text of this object
    /// </summary>
    private TMP_Text _text;

    new void Start()
    {
        base.Start();

        _text = GetComponent<TMP_Text>();

        transform.localScale = new Vector3(0.5f, 0.5f, 1);

        StartCoroutine(AnimateUp());
    }

    IEnumerator AnimateUp()
    {
        Vector3 initialTarget = TargetPoint;

        LeanTween.value(gameObject, 0, 1, 0.25f)
            .setEaseOutQuad()
            .setOnUpdate((float t) =>
            {
                _text.color = new Color(1, 1, 1, t);
                transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 1), Vector3.one, t);
            });

        yield return new WaitForSeconds(0.1f);

        LeanTween.value(gameObject, TargetPoint, TargetPoint + new Vector3(0, MoveUpAmount), TimeToLive)
            .setEaseOutCirc()
            .setOnUpdate((Vector3 target) =>
            {

                TargetPoint = target;
            });

        yield return new WaitForSeconds(TimeToLive);

        Vector3 currentValue = TargetPoint;

        LeanTween.value(gameObject, 0, 1, 0.2f)
            .setEaseOutQuad()
            .setOnUpdate((float t) => {
                _text.color = new Color(1, 1, 1, 1 - t);
                TargetPoint = Vector3.Lerp(currentValue, initialTarget, t / 3);
            });

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject, TimeToLive);
    }
}
