using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField]
    List<DiceVariant> diceVariants;

    [SerializeField]
    float timeToRoll;
    float rollStartTime;

    [SerializeField]
    float rotationSpeed;

    RollableDice currentDice;

    int result;

    Vector3 rotateAround1;
    Vector3 rotateAround2;

    [SerializeField]
    Dice debugDice;
    [SerializeField]
    int debugResult;
    [SerializeField]
    bool rollDebug;

    [SerializeField]
    float liveTime;

    [SerializeField]
    AnimationCurve scaleCurveX, scaleCurveY, scaleCurveZ;

    float GetNormalizedLiveTime()
    {
        return (Time.unscaledTime - rollStartTime) / liveTime;
    }

    [Serializable]
    public struct DiceVariant
    {
        public RollableDice RollableDice;
        public Dice DiceType;
    }

    private void Start()
    {
        if(rollDebug)
        {
            Init(debugDice, debugResult);
        }
    }

    private void Update()
    {
        if(currentDice != null)
        {
            transform.localScale = new Vector3(scaleCurveX.Evaluate(GetNormalizedLiveTime()),
                scaleCurveY.Evaluate(GetNormalizedLiveTime()),
                scaleCurveZ.Evaluate(GetNormalizedLiveTime()));
            if(Time.unscaledTime - rollStartTime >= timeToRoll)
            {
                Mathf.Clamp(debugResult, 0, currentDice.values.Count - 1);
                currentDice.transform.rotation = currentDice.values[result - 1];
            }
            else
            {
                currentDice.transform.Rotate(rotateAround1, rotationSpeed * Time.deltaTime, Space.Self);
                currentDice.transform.Rotate(rotateAround2, rotationSpeed * 10 * Time.deltaTime, Space.Self);
            }
            if(GetNormalizedLiveTime() >= 1)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Init(Dice diceType, int value)
    {
        DiceVariant variant = diceVariants.First(d => d.DiceType == diceType);
        currentDice = Instantiate(variant.RollableDice.gameObject, transform).GetComponent<RollableDice>();
        currentDice.transform.localPosition = Vector3.zero;
        rotateAround1 = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized;
        rotateAround2 = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized;
        rollStartTime = Time.unscaledTime;
        result = Math.Clamp( value, 1, (int)diceType);
    }
}
