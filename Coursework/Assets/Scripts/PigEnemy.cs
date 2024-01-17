using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigEnemy : MonoBehaviour
{
    [SerializeField]
    float chargeDistance = 1.0f;
    [SerializeField]
    float attackDistance = 1.0f;

    float nextChargeTime = 0;
    [SerializeField]
    float chargeCooldown = 20.0f;
    [SerializeField]
    float chargeTime = 2.0f;

    bool facingRight = true;
    bool charging = false;

    [SerializeField]
    LayerMask playerLayer;

    void Update()
    {
        // If can't charge || Charging
        if (nextChargeTime > Time.time || charging)
            return;

        // Get Closest Player
        Collider2D closestPlayer = FindClosest(chargeDistance);
        if (closestPlayer == null ) return;

        // Find Direction to closest
        Vector2 direction = closestPlayer.transform.position - transform.position;

        // Check the sign of the x component of the direction vector
        if (direction.x > 0)
        {
            // The closest player is to the right
            facingRight = true;
        }
        else if (direction.x < 0)
        {
            // The closest player is to the left
            facingRight = false;
        }
        // Charge
        StartCoroutine(Charge());

    }

    IEnumerator Charge()
    {
        charging = true;
        Vector2 force;
        // Face Direction && Set Direction Force
        if (facingRight)
        {
            Vector3 newRot = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newRot);  

            force = new Vector2(400.0f, 0.0f);
        }
        else
        {
            Vector3 newRot = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newRot);

            force = new Vector2(-400.0f, 0.0f);
        }

        // Set Animation
        GetComponent<Animator>().SetBool("Moving", true);


        bool complete = false;
        float finishedTime = Time.time + chargeTime;
        while (!complete)
        {
            if (Time.time > finishedTime) { complete = true; }
            GetComponent<Rigidbody2D>().AddForce(force);
            Collider2D closestPlayer = FindClosest(attackDistance);
            if (closestPlayer)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                Debug.Log("Attack");
                // TODO: Damage Player
                complete = true;
            }
            yield return new WaitForFixedUpdate();
        }

        AudioManager.instance.PlaySoundToAll("Hit");

        Debug.Log("Attack Finished");
        nextChargeTime = Time.time + chargeCooldown;
        charging = false;
        GetComponent<Animator>().SetBool("Moving", false);
    }

    Collider2D FindClosest(float dist)
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, dist, playerLayer);
        // If no players to aggro to, ignore
        if (players.Length == 0)
            return null;

        Collider2D closestPlayer = players[0];
        float closestDistance = Vector2.Distance(transform.position, closestPlayer.transform.position);

        for (int i = 1; i < players.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, players[i].transform.position);

            if (distance < closestDistance)
            {
                closestPlayer = players[i];
                closestDistance = distance;
            }
        }
        return closestPlayer;
    }
}
