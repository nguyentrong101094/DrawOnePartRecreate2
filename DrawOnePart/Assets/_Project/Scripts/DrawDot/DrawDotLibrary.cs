using GestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEngine;

public class DrawDotLibrary : GestureLibrary
{
    public DrawDotLibrary(string libraryName, bool forceCopy = false, bool useV4Gesture = true) : base(libraryName, forceCopy,useV4Gesture)
    {
    }

    public override void LoadLibrary()
    {
        // Uses the XML file in resources folder if it is webplayer or the editor.
        string xmlContents = "";
        string floatSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

#if !UNITY_WEBPLAYER && !UNITY_EDITOR
            xmlContents = FileTools.Read(persistentLibraryPath);
#else
        //xmlContents = Resources.Load<TextAsset>($"{GestureFolderName}/{libraryName}").text;
        xmlContents = FileTools.Read(resourcesPath);
#endif

        gestureLibrary.LoadXml(xmlContents);


        // Get "gesture" elements
        XmlNodeList xmlGestureList = gestureLibrary.GetElementsByTagName("gesture");

        // Parse "gesture" elements and add them to library
        foreach (XmlNode xmlGestureNode in xmlGestureList)
        {

            string gestureName = xmlGestureNode.Attributes.GetNamedItem("name").Value;
            XmlNodeList xmlPoints = xmlGestureNode.ChildNodes;
            List<Vector2> gesturePoints = new List<Vector2>();

            foreach (XmlNode point in xmlPoints)
            {

                Vector2 gesturePoint = new Vector2();
                gesturePoint.x = (float)System.Convert.ToDouble(point.Attributes.GetNamedItem("x").Value.Replace(",", floatSeparator).Replace(".", floatSeparator));
                gesturePoint.y = (float)System.Convert.ToDouble(point.Attributes.GetNamedItem("y").Value.Replace(",", floatSeparator).Replace(".", floatSeparator));
                gesturePoints.Add(gesturePoint);

            }

            Gesture gesture = new Gesture(gesturePoints, true);
            library.Add(gesture);
        }
    }

    /// <param name="fileName">File name must include extension (.xml)</param>
    public static string GetResourcesPathV4(string fileName)
    {
        return Path.Combine(Path.Combine(Application.dataPath, $"Resources/{Const.GESTURE_V4_FOLDER_NAME}"), fileName);
    }
}
