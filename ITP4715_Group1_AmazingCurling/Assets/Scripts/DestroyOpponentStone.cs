using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOpponentStone : MonoBehaviour
{
    private int chance = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (chance == 1)
        {
            if (gameObject.tag == "RedClone")
            {
                if (collision.gameObject.tag == "YellowClone")
                {
                    Destroy(collision.gameObject);
                    chance = 0;
                }
            }
            else if (gameObject.tag == "YellowClone")
            {
                if (collision.gameObject.tag == "RedClone")
                {
                    Destroy(collision.gameObject);
                    chance = 0;
                }
            }
        }
    }
}
