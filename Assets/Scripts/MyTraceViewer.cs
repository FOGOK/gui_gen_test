using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class MyTraceViewer : ITraceWriter
{
    public TraceLevel LevelFilter
    {
        // trace all messages. nlog can handle filtering
        get { return TraceLevel.Verbose; }
    }

    public void Trace(TraceLevel level, string message, Exception ex)
    {
        Debug.Log(message);
    }

}