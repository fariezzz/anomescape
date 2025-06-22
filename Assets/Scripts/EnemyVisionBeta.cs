using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisionBeta : MonoBehaviour
{
    public float detectRange = 8;
    public float detectAngle = 30;
    bool isInAngle, isInRange, isNotHidden;
    public GameObject player;
    public Text rangeText, hiddenText, angleText, detectedText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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

        RaycastHit hit;
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, Mathf.Infinity))
        {
            if (hit.transform == player.transform)
            {
                isNotHidden = true;
                hiddenText.text = "YOU'RE NOT HIDDEN!";
                hiddenText.color = Color.red;
            }
            else
            {
                isNotHidden = false;
                hiddenText.text = "YOU'RE HIDDEN!";
                hiddenText.color = Color.green;
            }
        }
        else
        {
            isNotHidden = false;
            hiddenText.text = "YOU'RE HIDDEN!";
            hiddenText.color = Color.green;
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
        else
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
