using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePush : MonoBehaviour
{
    // La force maximale de poussée à appliquer au joueur
    public float maxPushForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifie si la collision est avec un joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            // Calcule la vélocité de la sphère au moment de la collision
            Rigidbody sphereRigidbody = GetComponent<Rigidbody>();
            float sphereVelocity = sphereRigidbody.velocity.magnitude;

            // Calcule la force de poussée en fonction de la vélocité de la sphère
            float pushForce = sphereVelocity;

            // Calcule la direction dans laquelle pousser le joueur
            Vector3 pushDirection = collision.contacts[0].point - transform.position;
            pushDirection = pushDirection.normalized;

            // Applique la force au joueur
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}
