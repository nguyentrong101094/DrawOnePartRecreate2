using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using GestureRecognizer;
using System.Globalization;

/// <summary>
/// Reads the gestures from an XML file and creates a library by adding each of them to a List<> of "Gesture"s.
/// Also adds a gesture to a library and saves it to a file if it is NOT a web player.
/// </summary>
namespace GestureRecognizer
{
    public class GestureLibrary
    {

        protected string libraryName;
        protected string libraryFilename;
        protected string persistentLibraryParentPath; //for making folder
        protected string persistentLibraryPath;
        protected string resourcesPath;
        protected string xmlContents;
        protected XmlDocument gestureLibrary = new XmlDocument();
        protected List<Gesture> library = new List<Gesture>();

        public List<Gesture> Library { get { return library; } }

        protected static string GestureFolderName { get => Const.GESTURE_FOLDER_NAME; }

        [Obsolete]
        public GestureLibrary(string libraryName, bool forceCopy = false)
        {
            this.libraryName = libraryName;
            this.libraryFilename = libraryName + ".xml";
            persistentLibraryParentPath = Path.Combine(Application.persistentDataPath, GestureFolderName);
            this.resourcesPath = GetResourcesPath(libraryFilename);
            this.persistentLibraryPath = Path.Combine(persistentLibraryParentPath, libraryFilename);

            CopyToPersistentPath(forceCopy, false);
            LoadLibrary();

            if (Library.Count == 0)
                Debug.LogError($"{libraryName} has 0 gestures");
        }

        public GestureLibrary(string libraryName, bool forceCopy = false, bool useV4Gesture = false)
        {
            this.libraryName = libraryName;
            this.libraryFilename = libraryName + ".xml";
            if (useV4Gesture)
            {
                persistentLibraryParentPath = Path.Combine(Application.persistentDataPath, Const.GESTURE_V4_FOLDER_NAME);
                this.resourcesPath = Path.Combine(Path.Combine(Application.dataPath, $"Resources/{Const.GESTURE_V4_FOLDER_NAME}"), libraryFilename);
            }
            else
            {
                persistentLibraryParentPath = Path.Combine(Application.persistentDataPath, GestureFolderName);
                this.resourcesPath = GetResourcesPath(libraryFilename);
            }
            this.persistentLibraryPath = Path.Combine(persistentLibraryParentPath, libraryFilename);

            CopyToPersistentPath(forceCopy, true);
            LoadLibrary();
        }

        /// <param name="fileName">File name must include extension (.xml)</param>
        public static string GetResourcesPath(string fileName)
        {
            return Path.Combine(Path.Combine(Application.dataPath, $"Resources/{GestureFolderName}"), fileName);
        }

        /// <summary>
        /// Loads the library from an XML file
        /// </summary>
        public virtual void LoadLibrary()
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
            if (string.IsNullOrEmpty(xmlContents))
            {
                Debug.LogError($"{libraryName} gesture library at {resourcesPath} is empty");
            }
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

                Gesture gesture = new Gesture(gesturePoints, gestureName);
                library.Add(gesture);
            }
        }


        /// <summary>
        /// Adds a new gesture to library and then saves it to the xml.
        /// The trick here is that we don't reload the newly saved xml.
        /// It would have been a waste of resources. Instead, we just add
        /// the new gesture to the list of gestures (the library).
        /// </summary>
        /// <param name="gesture">The gesture to add</param>
        /// <returns>True if addition is succesful</returns>
        public bool AddGesture(Gesture gesture)
        {

            // Create the xml node to add to the xml file
            XmlElement rootElement = gestureLibrary.DocumentElement;
            XmlElement gestureNode = gestureLibrary.CreateElement("gesture");
            gestureNode.SetAttribute("name", gesture.Name);

            foreach (Vector2 v in gesture.Points)
            {
                XmlElement gesturePoint = gestureLibrary.CreateElement("point");
                gesturePoint.SetAttribute("x", v.x.ToString());
                gesturePoint.SetAttribute("y", v.y.ToString());

                gestureNode.AppendChild(gesturePoint);
            }

            // Append the node to xml file contents
            rootElement.AppendChild(gestureNode);

            try
            {

                // Add the new gesture to the list of gestures
                this.Library.Add(gesture);

                // Save the file if it is not the web player, because
                // web player cannot have write permissions.
#if !UNITY_WEBPLAYER && !UNITY_EDITOR
                FileTools.Write(persistentLibraryPath, gestureLibrary.OuterXml);
#elif UNITY_EDITOR && !UNITY_WEBPLAYER
                FileTools.Write(resourcesPath, gestureLibrary.OuterXml);
#endif

                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }

        }


        /// <summary>
        /// Copy to persistent data path so that we can save a new gesture
        /// on all platforms (except web player)
        /// </summary>
        /// <param name="forceCopy">Forces to copy over the existing XML file</param>
        protected void CopyToPersistentPath(bool forceCopy, bool useV4Gesture)
        {
            System.IO.Directory.CreateDirectory(persistentLibraryParentPath);
#if !UNITY_WEBPLAYER && !UNITY_EDITOR
            if (!FileTools.Exists(persistentLibraryPath) || (FileTools.Exists(persistentLibraryPath) && forceCopy))
            {
                string resPath;
                if (useV4Gesture)
                {
                    resPath = $"{Const.GESTURE_V4_FOLDER_NAME}/{libraryName}";
                }
                else
                {
                    resPath = $"{GestureFolderName}/{libraryName}";
                }
                var textAsset = Resources.Load<TextAsset>(resPath);
                if (textAsset == null) { Debug.LogError($"Load text asset: {libraryName} failed"); }
                string fileContents = textAsset.text;
                FileTools.Write(persistentLibraryPath, fileContents);
                Debug.Log($"Gesture file written to {persistentLibraryPath}");
            }
#endif
        }

    }
}