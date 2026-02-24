using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFightButton : MonoBehaviour
{
    public void PutDown()
    {
        EventManager.Instance.TriggerEvent(EventTypes.FightStartEvent, new FightStartEvent());
        gameObject.SetActive(false);
    }
}
