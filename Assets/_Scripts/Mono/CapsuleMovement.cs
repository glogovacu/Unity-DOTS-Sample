using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    public string targetLayerName = "TargetLayer";
    public float movementSpeed = 5f;

    private GameObject target;
    private List<GameObject> _targets;

    private void Start()
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            targetLayerName = "Player2";
            _targets = CapsuleSpawner.Instance.Player2s;
        }
        else
        {
            targetLayerName = "Player1";
            _targets = CapsuleSpawner.Instance.Player1s;
        }
        target = _targets[Random.Range(0, _targets.Count)];


    }

    private void Update()
    {
        if (!CapsuleSpawner.Instance.IsPlayerMovementEnabled) {
            return;
        }
        if (target == null)
        {
            target = FindClosestTarget();
            if (target == null)
            {
                // No targets left, so we stop moving
                return;
            }
        }

        // Move towards the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            Destroy(target);
            _targets.Remove(target);
            target = null;
        }
    }


    private GameObject FindClosestTarget()
    {
        List<GameObject> targets = _targets;
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target;
            }
        }

        return closestTarget;
    }
}
