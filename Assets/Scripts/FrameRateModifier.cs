using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateModifier : MonoBehaviour {
    public int frameRate = 50000;
    void Update ( ) {
        System.Threading.Thread.Sleep ( 1000 / frameRate );
        Debug.Log ( "Framerate" + ( 1 / Time.deltaTime ) );
    }
}
