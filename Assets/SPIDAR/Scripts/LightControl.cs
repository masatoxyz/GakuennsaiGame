//
// LightControl.cs
//
using UnityEngine;

public class LightControl : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        float intensity = Input.GetAxis("Intensity");

        if (intensity == 1)
        {
            Light l = gameObject.GetComponent<Light>();

            l.intensity += 0.01f;
        }

        if (intensity == -1)
        {
            Light l = gameObject.GetComponent<Light>();

            l.intensity -= 0.01f;
        }
    }

} // end of class LightControl.

// end of file.
