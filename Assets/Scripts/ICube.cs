using UnityEngine;
using UnityEngine.InputSystem;

public class ICube : MonoBehaviour
{
    [SerializeField] protected Key keyOne = Key.W;
    [SerializeField] protected Key keyTwo = Key.S;
    [SerializeField] protected Vector3 moveDirection = Vector3.forward;
    [SerializeField] protected float speed = 10f;

    protected Rigidbody _rb;
    protected Keyboard _keyboard;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _keyboard = Keyboard.current;
    }

    protected virtual void FixedUpdate()
    {
        if (_keyboard == null)
        {
            Debug.Log("Keyboard unavailable!");
            return;
        }

        Vector3 movement = GetMovement();

        if (movement != Vector3.zero)
        {
            ApplyMovement(movement);
            Debug.Log($"Position: {_rb.position}, Velocity: {_rb.linearVelocity}");
        }
    }

    protected virtual Vector3 GetMovement()
    {
        Vector3 movement = Vector3.zero;

        if (keyOne != Key.None && _keyboard[keyOne].isPressed)
            movement = moveDirection * speed;
        else if (keyTwo != Key.None && _keyboard[keyTwo].isPressed)
            movement = -moveDirection * speed;

        return movement;
    }

    protected virtual void ApplyMovement(Vector3 movement)
    {
        Vector3 force = (movement - _rb.linearVelocity) * 10f;
        _rb.AddForce(force, ForceMode.Acceleration);
    }

    protected virtual void OnTriggerEnter(Collider other) { }
    protected virtual void OnTriggerExit(Collider other) { }
}