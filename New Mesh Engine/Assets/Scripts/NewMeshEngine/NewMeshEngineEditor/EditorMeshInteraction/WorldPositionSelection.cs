using System;
using System.Linq;
using UnityEngine;

public class WorldPositionSelection : MonoBehaviour
{
    [SerializeField] private GameObject hitPlane;
    RaycastHit[] hitResults = new RaycastHit[50];
    private void Awake()
    {
        //hitPlane.SetActive(false);
    }
    public Vector3 GetClosestHitPoint(float maxDistance)
    {hitPlane.transform.localPosition = Vector3.forward * maxDistance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int hits = Physics.RaycastNonAlloc(ray, hitResults, 100);
        if (hits > 0)
        {
            int hitindexClosestToPlayer = 0;
            float closetDistance = float.MaxValue;
            for (int i = 0; i < hits; i++)
            {
                if (hitResults[i].distance < closetDistance)
                {
                    closetDistance = hitResults[i].distance;
                    hitindexClosestToPlayer = i;
                }
            }
            return hitResults[hitindexClosestToPlayer].point;
        }
        return new Vector3(int.MaxValue, int.MaxValue,int.MaxValue);

        /*else
        {
            hitPlane.transform.position = transform.position + transform.forward*10;
            transform.rotation = Quaternion.Euler(transform.forward);
            hitPlane.SetActive(true);
        }*/
    }
}


