using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool gamePaused;

    private void Start()
    {
        gamePaused = false;
    }
    void Update()
    {
        GameEvents.UpdateAction?.Invoke(Time.deltaTime, gamePaused);
    }
}
