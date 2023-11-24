using System;
using UnityEngine;

public interface IInput
{
    bool PointerClick();
    Vector3Int GetPointerGridPositionPosition(bool showIndicator);
    bool Cancel();
    float IncreaseOrDecrease();
}