using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmissionChanger : MonoBehaviour
{
    public Slider SeasonIntensitySlider;
    public ParticleSystem.EmissionModule emission;

    // Start is called before the first frame update
    void Start()
    {
        SeasonIntensitySlider.onValueChanged.AddListener((val) => ValueChangeCheck());  // Assign listener to UI slider
        emission = GetComponent<ParticleSystem>().emission;                             // Assign emission ref
    }

    void ValueChangeCheck()
    {
        Debug.Log(SeasonIntensitySlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        float mInput = Input.mouseScrollDelta.y;                                        // Input ref incase we need it later for something else

        if (mInput != 0)
        {
            int direction = (mInput > 0 ? 1 : -1);                                      // Positive multiply if scrolling up, negative if scrolling down
            SeasonIntensitySlider.value += 0.1f * direction;                            // Multiply for UI slider
            emission.rateOverTime = (emission.rateOverTime.constant + 5f) * direction;  // Multiply for particle ROT constant variable
        }
    }
}
