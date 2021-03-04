using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gront
{
    public class DebugSwitchActive : MonoBehaviour
    {
        public enum Modes
        {
            Unset = 0,
            ShowOnlyInDebugMode = 1,
            ShowInDebugBuild = 2
        }

        public Modes mode;

        void Awake()
        {
            switch (mode)
            {
                case Modes.Unset:
                case Modes.ShowOnlyInDebugMode:
                    if (!DebugManager.IsDebugMode())
                        gameObject.SetActive(false);
                    break;
                case Modes.ShowInDebugBuild:
                    if (!Debug.isDebugBuild)
                        gameObject.SetActive(false);
                    break;
            }
        }
    }
}