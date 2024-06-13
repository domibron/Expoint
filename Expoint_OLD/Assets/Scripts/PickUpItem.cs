using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour, IFKeyInput
{
    [SerializeField] InventoryController inventoryController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void IFKeyInput.F_Key_Pressed() // ! redo efficiantly
    {
        RaycastHit hit;

        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f);

        if (hit.transform.GetComponent<ObjectItemData>() != null)
        {
            ObjectItemData objData = hit.transform.GetComponent<ObjectItemData>();

            if (inventoryController.PickUpItemObject(objData))
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }
}
