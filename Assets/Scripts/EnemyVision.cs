using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    // public Transform head;
    public float detectRange = 15f;
    public float detectAngle = 80f;

    public GameObject player;
    public Text rangeText, hiddenText, angleText, detectedText;

    bool isInRange, isInAngle, isNotHidden;
    public bool PlayerVisible => isInRange && isInAngle && isNotHidden;
    public bool PlayerFullySafe => !isNotHidden && !isInAngle;

    void Update()
    {
        isInAngle = false;
        isInRange = false;
        isNotHidden = false;

        if (Vector3.Distance(transform.position, player.transform.position) < detectRange)
        {
            isInRange = true;
            rangeText.text = "In Range";
            rangeText.color = Color.red;
        }
        else
        {
            rangeText.text = "Not In Range";
            rangeText.color = Color.green;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            if (hit.transform == player.transform)
            {
                isNotHidden = true;
                hiddenText.text = "YOU'RE NOT HIDDEN!";
                hiddenText.color = Color.red;
                Debug.DrawRay(transform.position, directionToPlayer * hit.distance, Color.red);
            }
            else
            {
                isNotHidden = false;
                hiddenText.text = "YOU'RE HIDDEN!";
                hiddenText.color = Color.green;
                Debug.DrawRay(transform.position, directionToPlayer * hit.distance, Color.yellow);
            }
        }
        else
        {
            isNotHidden = false;
            hiddenText.text = "YOU'RE HIDDEN!";
            hiddenText.color = Color.green;
            Debug.DrawRay(transform.position, directionToPlayer * hit.distance, Color.yellow);
        }

        Vector3 side1 = player.transform.position - transform.position;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);

        if (angle < detectAngle && angle > -1 * detectAngle && isInRange)
        {
            isInAngle = true;
            angleText.text = "YOU'RE IN VISION ANGLE!";
            angleText.color = Color.red;
        }
        else
        {
            angleText.text = "You're not in vision angle :)";
            angleText.color = Color.green;
        }

        if (isInAngle && isInRange && isNotHidden)
        {
            detectedText.text = "Player Detected!";
            detectedText.color = Color.red;
        }
        else if (!isInAngle && !isNotHidden)
        {
            detectedText.text = "Player Undetected!";
            detectedText.color = Color.green;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.cyan;

        // Sisi kanan
        Vector3 rightBoundary = Quaternion.Euler(0, detectAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightBoundary * detectRange);

        // Sisi kiri
        Vector3 leftBoundary = Quaternion.Euler(0, -detectAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftBoundary * detectRange);
    }
}
