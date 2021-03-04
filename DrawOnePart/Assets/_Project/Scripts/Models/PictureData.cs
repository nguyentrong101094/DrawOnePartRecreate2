using SimpleSQL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureData
{
    public int id { get; set; }
    [PrimaryKey]
    [SQLite4Unity3d.PrimaryKey]
    public string name { get; set; }
    public float bound_x { get; set; }
    public float bound_y { get; set; }
    public float bound_width { get; set; }
    public float bound_height { get; set; }
    public float bound_angle_z { get; set; }
    //public float picture_center_x { get; set; }
    //public float picture_center_y { get; set; }
    //public Vector2 PictureCenterPos => new Vector2(picture_center_x, picture_center_y);
    public float score_required { get; set; }
    public float ScoreRequired
    {
        get
        {
            if (score_required > 0.0001f)
            {
                return score_required;
            }
            else if (!use_v4) return Const.GESTURE_SCORE_REQUIRED_DEFAULT;
            else return Const.GESTURE_V4_SCORE_REQUIRED_DEFAULT;
        }
    }

    public bool use_v4 { get; set; }
}
