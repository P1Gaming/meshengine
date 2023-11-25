using UnityEngine;

public class WorldPositionSelection : MonoBehaviour
{
    [SerializeField] private GameObject hitPlane;
    RaycastHit[] hitResults = new RaycastHit[50];
    private float maxDistance = 30;
    [SerializeField] private Transform indicator;
    
    
    public Vector3Int GetClosestHitPoint()
    {hitPlane.transform.localPosition = Vector3.forward * maxDistance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int hits = Physics.RaycastNonAlloc(ray, hitResults, maxDistance+10);
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

            Vector3Int pos = Vector3Int.RoundToInt(hitResults[hitindexClosestToPlayer].point);
            indicator.position = pos;
            return pos;
        }
        return new Vector3Int(int.MaxValue, int.MaxValue,int.MaxValue);
    }

    public void ChangeDistance(float value)
    {
        maxDistance += value;
        maxDistance = Mathf.Clamp(maxDistance, 10, 50);
    }
}


