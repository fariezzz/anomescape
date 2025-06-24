using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CapsuleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Noise Manager")]
    public float noiseRadius = 15f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // agar tetap menempel tanah
        }

        // Input gerakan
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravitasi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    //public void MakeNoise()
    //{
    //    Collider[] enemies = Physics.OverlapSphere(transform.position, 30f, LayerMask.GetMask("Enemy"));
    //    foreach (Collider col in enemies)
    //    {
    //        var enemy = col.GetComponent<EnemyAI>();
    //        if (enemy != null)
    //        {
    //            enemy.ReportNoise(transform.position);
    //        }
    //    }
    //}
}
