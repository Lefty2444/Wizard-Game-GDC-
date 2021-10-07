using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    
    public float wandRotationSpeed = 15;

    private Transform wand;

    // Start is called before the first frame update
    void Awake()
    {
        wand = transform.GetChild(0);
    }



    // Update is called once per frame
    void Update()
    {
        MoveWand();
    }


    private void MoveWand()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(wand.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        wand.rotation = Quaternion.Slerp(wand.rotation, targetRotation, wandRotationSpeed * Time.deltaTime);
    }
}
