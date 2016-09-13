using UnityEngine;
using LMWidgets;
using System.Collections;

public class LigandMenuSliderDataBinder : DataBinderSlider
{
    public float outputValue = 0.0f;
    public float min = 0.0f;
    public float max = 1.0f;
    private PopulateLigandMenu menuScript;

    //void Awake()
    //{
    //    Mathf.Clamp(outputValue, min, max);
    //    base.Awake(); //required if using Awake!
    //}

    override public float GetCurrentData()
    {
        return (outputValue - min) / (max - min);
    }

    override protected void setDataModel(float value)
    {
        if (menuScript == null)
        {
            menuScript = transform.parent.gameObject.GetComponent<PopulateLigandMenu>();
        }
        
        outputValue = value * (max - min) + min;
        menuScript.Pagination(outputValue);
    }
}
