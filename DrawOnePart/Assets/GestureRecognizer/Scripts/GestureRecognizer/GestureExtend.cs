using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

namespace GestureRecognizer
{
    public partial class Gesture
    {
        /// <summary>
        /// Recognize the gesture by comparing it to every single gesture in the library,
        /// scoring them and finding the highest score. Don't recognize the gesture if there
        /// is less than 2 points.
        /// 
        /// There are two algorithms to recognize a gesture: $1 and Protractor. $1 is slower
        /// compared to Protractor, however, $1 provides a scoring system in [0, 1] interval
        /// which is very useful to determine "how much" the captured gesture looks like the 
        /// library gesture.
        /// 
        /// To find out more about the algorithms and how they work, see the respective method
        /// comments.
        /// </summary>
        /// <param name="gestureLibrary">The library to run the gesture against.</param>
        /// <param name="useProtractor">If this is true, the faster Protractor algorithm will be used.</param>
        /// <returns>Recognized gesture's name and its score</returns>
        public Result Recognize(GestureLibrary gestureLibrary, out Gesture recognizedGesture, bool useProtractor = false)
        {
            //this function return the recognized gesture
            recognizedGesture = null;
            if (this.Points.Count <= 2)
            {
                return new Result("Not enough points captured", 0f);
            }
            else
            {
                List<Gesture> library = gestureLibrary.Library;

                float bestDistance = float.MaxValue;
                int matchedGesture = -1;

                // Match the gesture against all the gestures in the library
                for (int i = 0; i < library.Count; i++)
                {

                    float distance = 0;

                    if (useProtractor)
                    {
                        // See ProtractorAlgorithm() method's comments to find out more about it.
                        distance = ProtractorAlgorithm(library[i].Vector, this.Vector);
                    }
                    else
                    {
                        // See DollarOneAlgorithm() method's comments to find out more about it.
                        distance = DollarOneAlgorithm(library[i], -this.ANGLE_RANGE, +this.ANGLE_RANGE, this.ANGLE_PRECISION);
                    }

                    // If distance is better than the best distance take it as the best distance, 
                    // and gesture as the recognized one.
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        matchedGesture = i;
                    }
                }

                // No match, score zero. If there is a match, send the name of the recognized gesture and a score.
                if (matchedGesture == -1)
                {
                    return new Result("No match", 0f);
                }
                else
                {
                    recognizedGesture = library[matchedGesture];
                    return new Result(library[matchedGesture].Name, useProtractor ? 1f / bestDistance : 1f - bestDistance / this.HALF_DIAGONAL);
                }
            }
        }
    }

}