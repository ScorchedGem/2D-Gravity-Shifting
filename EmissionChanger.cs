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
        SeasonIntensitySlider.onValueChanged.AddListener((val) => ValueChangeCheck());
        emission = GetComponent<ParticleSystem>().emission;
    }

    void ValueChangeCheck()
    {
        Debug.Log(SeasonIntensitySlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        float mInput = Input.mouseScrollDelta.y;

        if (mInput != 0)
        {
            int direction = (mInput > 0 ? 1 : -1);
            SeasonIntensitySlider.value += 0.1f * direction;
            emission.rateOverTime = (emission.rateOverTime.constant + 5f) * direction;
        }
    }
}
