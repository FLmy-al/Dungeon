using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance;

    public GridController onGrid;
    public Item onItem;
    public bool isOnDrag;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            isOnDrag = false;
        }

        onGrid = null;
        onItem = null;
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            onItem = collision.gameObject.GetComponent<Item>();
            if (!isOnDrag)
            {
                ItemDetailUI.Instance.canvasGroup.alpha = 1f;
                ItemDetailUI.Instance.ShowItemDetail(onItem);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            ItemDetailUI.Instance.canvasGroup.alpha = 0f;
            onItem = null;
        }
    }
}
