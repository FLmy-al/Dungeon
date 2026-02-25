using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEndContinueButton : MonoBehaviour
{
    public void PutDown()
    {
        EventManager.Instance.TriggerEvent(EventTypes.ExitFightNode, new ExitFightNodeEvent());
        GetComponentInParent<GridController>().gameObject.SetActive(false);
    }
}
