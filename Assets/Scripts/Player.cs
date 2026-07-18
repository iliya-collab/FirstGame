using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] Key keyOne = Key.W;
    [SerializeField] Key keyTwo = Key.S;
    [SerializeField] Vector3 moveDirection = Vector3.forward;
    [SerializeField] float speed = 10f;

    private Rigidbody _rb;
    private Keyboard _keyboard;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _keyboard = Keyboard.current;
    }

    private void FixedUpdate()
    {
        if (_keyboard == null)
        {
            Debug.Log("Keyboard unavailable!");
            return;
        }

        Vector3 movement = Vector3.zero;

        if (_keyboard[keyOne].isPressed)
            movement = moveDirection * speed * Time.fixedDeltaTime;
        else if (_keyboard[keyTwo].isPressed)
            movement = -moveDirection * speed * Time.fixedDeltaTime;

        if (movement != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + movement);
            Debug.Log($"Position: {_rb.position}");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (this.CompareTag("Player") && this.CompareTag("Finish"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}