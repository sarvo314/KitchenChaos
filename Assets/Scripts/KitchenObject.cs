using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] KitchenObjectSO kitchenObjectSO;

    private ClearCounter clearCounter;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetClearCounter(ClearCounter clearCounter)
    {
        //as classes are reference type we can do this
        ClearCounter previousClearCounter = this.clearCounter;
        if (previousClearCounter != null)
        {
            previousClearCounter.ClearKitchenObject();
        }
        //set this objects counter as the new counter that was passed through parameter
        this.clearCounter = clearCounter;

        if (clearCounter.HasKitchenObject())
        {
            Debug.LogError("Counter already has an object");
        }
        //set the kitchen object of the new counter to the current kitchen object
        clearCounter.SetKitchenObject(this);
        transform.parent = clearCounter.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public ClearCounter GetClearCounter()
    {
        return clearCounter;
    }

}
