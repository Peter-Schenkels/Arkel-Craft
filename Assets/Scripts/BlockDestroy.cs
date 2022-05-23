using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class BlockDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        bool leftIsDown = false;
        bool rightIsDown = false;
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 direction = new Vector3(0, 0, 0);
        void Start()
        {

        }


        void Update()
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 6;
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Input.GetMouseButtonDown(0) && !leftIsDown)
            {
                leftIsDown = true;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f, layerMask))
                {
                    Vector3 blockPosition;
                    if (hit.normal.z != -1 && hit.normal.x != -1 && hit.normal.y != -1)
                    {
                        blockPosition = (hit.point - hit.normal);
                    }
                    else
                    {
                        blockPosition = hit.point;
                    }

                    hit.collider.GetComponent<WorldGeneration>().DestroyBlock(blockPosition);

                }

            }
            else if (Input.GetMouseButtonDown(1) && !rightIsDown)
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5f, layerMask))
                {
                    Vector3 blockPosition;
                    Debug.Log(hit.normal);
                    if (hit.normal.z != 1 && hit.normal.x != 1 && hit.normal.y != 1)
                    {
                        blockPosition = (hit.point + hit.normal);
                    }
                    else
                    {
                        blockPosition = hit.point;
                    }
                    hit.collider.GetComponent<WorldGeneration>().AddBlock(blockPosition);
                }
            }
            else
            {
                leftIsDown = false;
                rightIsDown = false;
            }

        }
    }
}
