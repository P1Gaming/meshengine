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
    public Vector3Int GetClosestHitPoint(float maxDistance)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int hits = Physics.RaycastNonAlloc(ray, hitResults, 30);
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

            return Vector3Int.CeilToInt(hitResults[hitindexClosestToPlayer].point);
        }
        return new Vector3Int(int.MaxValue, int.MaxValue,int.MaxValue);

        /*else
        {
            hitPlane.transform.position = transform.position + transform.forward*10;
            transform.rotation = Quaternion.Euler(transform.forward);
            hitPlane.SetActive(true);
        }*/
    }
}


