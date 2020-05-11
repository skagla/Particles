using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.VFX;
using NaughtyAttributes;

public class OscReceiver : MonoBehaviour
{
    public bool debug = false;
    public OSC osc;
    // public string oscAdress;

    [SerializeField]
    private VisualEffect visualEffect;

    public string prefix;

    [ReorderableList]
    public List<Converter> converters = new List<Converter>();

    [System.Serializable]
    public class OnOSCReceivedString : UnityEvent<string> { };
    //    public OnOSCReceivedString onOSCReceivedString;

    [System.Serializable]
    public class OnOSCReceivedFloat : UnityEvent<float> { };
    //   public OnOSCReceivedFloat onOSCReceivedFloat;

    void Start()
    {
        if (osc == null)
            osc = GameObject.Find("OSC").GetComponent<OSC>();
        if (visualEffect == null)
            visualEffect = GetComponent<VisualEffect>();
        //osc.SetAddressHandler(oscAdress, OnReceive);
        osc.SetAllMessageHandler(OnReceiveAll);

        foreach (Converter converter in converters)
        {
            converter.setVisualEffect(visualEffect);
            osc.SetAddressHandler(prefix + converter.oscAdress, converter.OnReceive);
        }

    }

    [Button]
    public void ReInit()
    {
        osc.ClearAddressHandlers();
        foreach (Converter converter in converters)
        {
            osc.SetAddressHandler(prefix + converter.oscAdress, converter.OnReceive);
        }
    }

    void OnReceiveAll(OscMessage message)
    {
        // var values = message.values; values.Count
        // output = message. ToFloat();
        // output = "" + message.GetString(0);
        //output = "" + message.GetFloat(0);
        //output = message.values.; inputs[i] = message.GetFloat(i);
        // onOSCReceivedFloat.Invoke(message.GetFloat(0));
        if (debug)
            print("osc: " + message.address + ", " + message.GetFloat(0)); //+ ", " + message.values
    }


}

[Serializable]
public class Converter
{


    VisualEffect visualEffect;
    public bool isActive;

    public string oscAdress;
    public string vfxParameterName;


    [BoxGroup("Mapping")]
    public float inputMin = 0;
    [BoxGroup("Mapping")]
    public float inputMax = 1;
    [BoxGroup("Mapping")]
    public float outputMin = 0;
    [BoxGroup("Mapping"), SerializeField]
    public float outputMax = 1;
    public bool clamp;

    [ReadOnly]
    public float input;
    [ReadOnly]
    public float output;

    public void setVisualEffect(VisualEffect vfx)
    {
        visualEffect = vfx;
    }

    public void OnReceive(OscMessage message)
    {

        input = message.GetFloat(0);
        output = MapParameter(input);
        if (clamp)
        {
            output = Mathf.Clamp(output, outputMin, outputMax);
        }
        if (isActive)
        {
            Debug.Log("osc: " + message.address + ", " + message.GetFloat(0));
            visualEffect.SetFloat(vfxParameterName, output);
        }
    }

    public float MapParameter(float input)
    {
        if (inputMax - inputMin == 0)
            return 0;
        return outputMin + (outputMax - outputMin) * ((input - inputMin) / (inputMax - inputMin));
    }
}

