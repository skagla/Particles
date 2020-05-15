using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using NaughtyAttributes;


public class OscSend : MonoBehaviour {

	public OSC osc;
    public string prefix;
    [ReorderableList]
    public List<Sender> senders = new List<Sender>();

    //public string ad
    // Use this for initialization
    void Start () {
        if (osc == null)
            osc = GameObject.Find("OSC").GetComponent<OSC>();

        foreach (Sender sender in senders)
        {
            sender.InitOsc(osc, prefix);
        }
    }
	
	// Update is called once per frame
	void Update () {


        foreach (Sender sender in senders)
        {
            sender.Update();
        }

        //  osc.Send(message);


    }

    public void SendOsc(string msg)
    {
        
        OscMessage message = new OscMessage();
        message.address = "/Send";
        message.values.Add(msg);
        osc.Send(message);
    }

}

[Serializable]
public class Sender
{
    public string oscAdress;
    private string prefix;
    [Range(0,1)]
    public float value;
    private float oldValue = -99;

    private OSC osc;



    public void Update()
    {
        if(value != oldValue)
        {
            oldValue = value;
            Send();
        }
    }

    public void Send()
    {
        OscMessage message = new OscMessage();
        message.address = prefix + oscAdress;
        message.values.Add(value);
        osc.Send(message);
    }

    public void InitOsc(OSC _osc, string _prefix)
    {
        osc = _osc;
        prefix = _prefix;
    }
}
