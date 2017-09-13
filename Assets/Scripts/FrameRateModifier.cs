using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateModifier : MonoBehaviour {

    void Update ( ) {
        System.Threading.Thread.Sleep ( 100 );
        Debug.Log ( "Framerate" + (1/Time.deltaTime) );
    }
}
