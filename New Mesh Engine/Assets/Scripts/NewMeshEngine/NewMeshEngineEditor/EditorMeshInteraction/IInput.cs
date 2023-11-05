using System;
using UnityEngine;

public interface IInput
{
    bool PointerClick();
    Vector3 GetPointerPosition();
    bool Cancel();
    float IncreaseOrDecrease();
}