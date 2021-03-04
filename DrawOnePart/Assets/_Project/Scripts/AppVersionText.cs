using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppVersionText : MonoBehaviour
{
    [SerializeField] TMP_Text versionText;
    //[SerializeField] int buildVersion;
    // Start is called before the first frame update
    void Start()
    {
        versionText.text = string.Format("v{0} ({1})", Application.version, Const.BUILD_VERSION_CODE);
    }

    private void Reset()
    {
        versionText = GetComponent<TMP_Text>();
    }
}
