using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDotData
{
    public int id { get; set; }
    public int stage_id { get; set; }
    public int order { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    Vector2? _position;
    public Vector2 Position
    {
        get
        {
            if (!_position.HasValue)
                _position = new Vector2(x, y);
            return _position.Value;
        }
    }
}
