using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] TMP_Text debugText;
    ChunkCoord playerChunk;
    Vector3Int playerVoxel;

    World world;
    float timer;
    float frameRate;

    private void OnEnable()
    {
        GameEvents.UpdateAction += OnUpdate;
        GameEvents.UpdatePlayerCoordsInWorldEvent += UpdatePlayerPosition;
    }

    private void OnDisable()
    {
        GameEvents.UpdateAction -= OnUpdate;
        GameEvents.UpdatePlayerCoordsInWorldEvent -= UpdatePlayerPosition;
    }

    void Start()
    {
        InputManager.openDebugScreenAction += ToggleScreen;
        gameObject.SetActive(false);
        timer = 0;
        frameRate = 0;
    }

    private void OnDestroy()
    {
        InputManager.openDebugScreenAction -= ToggleScreen;
    }

    // Update is called once per frame
    void OnUpdate(float dT, bool gamePaused)
    {
        debugText.text = "Survival Craft Game";
        debugText.text += "\n";
        debugText.text += "=====================";
        debugText.text += "\n\n";
        debugText.text += frameRate + " FPS";
        debugText.text += "\n";
        debugText.text += "Current Chunk: " + playerChunk.x + "," + playerChunk.z;
        debugText.text += "\n";
        debugText.text += "Current Voxel: " + playerVoxel.x + "," + playerVoxel.y + "," + playerVoxel.z;

        if (timer >= 1)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
            timer += dT;
    }

    void ToggleScreen()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    void UpdatePlayerPosition(ChunkCoord chunkCoord, Vector3Int voxel)
    {
        playerChunk = chunkCoord;
        playerVoxel = voxel;
    }
}
