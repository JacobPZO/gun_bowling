using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public float threshold;
    public PlayerController Player;
    private bool knockedOver;

    public bool upright()
    {
        return transform.up.y > threshold;/*say 0.6 ?*/
    }

    // Start is called before the first frame update
    void Start()
    {
        knockedOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!knockedOver && !upright())
        {
            ++Player.score;
            knockedOver=true;
        }
    }
}
