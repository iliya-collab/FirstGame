using UnityEngine;

public class Cube : ICube
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("Cube") && (other.CompareTag("Cube") || other.CompareTag("Player")))
        {
            foreach (ButtonActivator button in FindObjectsOfType<ButtonActivator>())
                button.canPush = false;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (this.CompareTag("Cube") && (other.CompareTag("Cube") || other.CompareTag("Player")))
        {
            foreach (ButtonActivator button in FindObjectsOfType<ButtonActivator>())
                button.canPush = true;
        }
    }
}