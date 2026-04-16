using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    PlayerController controller;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    // Estos métodos los llama automáticamente el componente PlayerInput
    // siempre que se llamen igual que la acción precedidos de "On"

    void OnMove(InputValue value)
    {
        controller.SetMoveInput(value.Get<Vector2>().x);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
            controller.Jump();
    }

    void OnDash(InputValue value)
    {
        if (value.isPressed)
            controller.Dash();
    }

    void OnMeleeAttack(InputValue value)
    {
        if (value.isPressed)
            GetComponent<PlayerCombat>().MeleeAttack();
    }

    void OnRangeAttack(InputValue value)
    {
        if (value.isPressed)
            GetComponent<PlayerCombat>().RangeAttack();
    }
}