using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitJsonHelper
{
    static bool isInit;
    public static void Initialize()
    {
        if (!isInit)
        {
            JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(Convert.ToDouble(obj)));
            JsonMapper.RegisterImporter<double, float>(input => Convert.ToSingle(input));
            isInit = true;
        }
    }
}
