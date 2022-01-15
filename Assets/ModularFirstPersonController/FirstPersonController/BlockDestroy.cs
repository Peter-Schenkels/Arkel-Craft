using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    bool isDown = false;
    BlockCollider component;
    void Start()
    {

    }


    void Update()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward));
        if (Input.GetMouseButtonDown(0) && !isDown)
        {
            isDown = true;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f, layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent<BlockCollider>(out component))
                {
                    Debug.Log(transform.position);
                    Debug.Log(hit.point);

                    component.gameObject.transform.GetComponent<WorldGeneration>().DestroyBlock(hit.point);

                    component.DestroyBlock();
                    Debug.Log("=============");
                }
            }
        }
        else
        {
            isDown = false;
        }


    }
}
