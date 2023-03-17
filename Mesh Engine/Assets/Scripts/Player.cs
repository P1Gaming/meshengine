using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform highlightBlock;
    [SerializeField] Transform highlightBlockPlacement;

    [SerializeField] Camera playerCamera;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] float maxBlockHitDistance;
    [SerializeField] float maxPlaceBlockDistance;

    [SerializeField] float blockSkinWidth; // how much the block detect ray should go beyond the hit point to find the actual block

    World playerWorld;

    private void OnEnable()
    {
;        GameEvents.UpdateAction += OnUpdate;
    }

    private void OnDisable()
    {
        GameEvents.UpdateAction -= OnUpdate;
    }

    public void InitPlayer(World world)
    {
        playerWorld = world;
    }

    private void OnUpdate(float dTime, bool paused)
    {
        GetBlockUnderCrosshair();
    }

    void GetBlockUnderCrosshair()
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
        float rayLength = 500f;

        // actual Ray
        Ray ray = Camera.main.ViewportPointToRay(rayOrigin);

        // debug Ray
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength, groundLayer))
        {
            Vector3Int placeVoxelCoord = playerWorld.GetVoxelCoordFromWorldPosition(hit.point - ray.direction * blockSkinWidth);
            Vector3Int hitVoxelCoord = playerWorld.GetVoxelCoordFromWorldPosition(hit.point + ray.direction * blockSkinWidth);
            BlockType block = playerWorld.blockTypes[playerWorld.GetVoxelFromWorld(hitVoxelCoord)];

            if (Vector3.Distance(hitVoxelCoord, transform.position) < maxBlockHitDistance)
            {
                highlightBlock.gameObject.SetActive(true);
                highlightBlock.position = hitVoxelCoord;
                highlightBlock.rotation = Quaternion.identity;
            }
            else
                highlightBlock.gameObject.SetActive(false);

            if (Vector3.Distance(placeVoxelCoord, transform.position) < maxPlaceBlockDistance)
            {
                highlightBlockPlacement.gameObject.SetActive(true);
                highlightBlockPlacement.position = placeVoxelCoord;
                highlightBlockPlacement.rotation = Quaternion.identity;
            }
            else
                highlightBlockPlacement.gameObject.SetActive(false);
        }
    }
}
