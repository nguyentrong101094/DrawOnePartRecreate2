using GestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DrawDotStageSetup : MonoBehaviour
{
    [SerializeField] DrawDot drawDotPrefab;
    [SerializeField] LineRenderer lineDotConnect;
    List<DrawDot> dotsInStage = new List<DrawDot>();

    public PlayerFingerDraw playerFingerDraw;
    [SerializeField] TMP_Text debugTextWrongDraw;

    public System.EventHandler<GestureLibrary> onGestureLibraryLoaded;

    public float checkRadius = 2f;
    public float CheckRadiusSqr => Mathf.Pow(checkRadius, 2f);
    int lastCorrectDotID = -1; //last dot that player has crossed correctly
    int drawingDirection = 0; //check drawing direction. If player draw clockwise, set to positive, otherwise, set to negative

    float defaultLimitErrorRadiusSqr = 9f; //if player draw this far away from next check dot, mark the line error
    float limitErrorRadiusSqr;
    public float LimitErrorRadiusSqr
    {
        get => limitErrorRadiusSqr; set
        {
            limitErrorRadiusSqr = value;
            Debug.Log($"error radius {limitErrorRadiusSqr}");
        }
    }

    bool drawStayInCorrectArea;
    public delegate void OnDrawScoreDelegate(float score);
    public event OnDrawScoreDelegate onDrawScore;

    private void Start()
    {
        //playerFingerDraw.paintHitScreen.onFingerHitUpdate += CheckDrawCorrect;
        //playerFingerDraw.onClearDraw += OnReset;
        OnReset(null, null);
    }

    //old original method, discarded
    public void Setup(int stageID)
    {
        var drawDotData = DatabaseSimpleSQL.Instance.GetDrawDots(stageID).ToList();
        for (int i = 0; i < drawDotData.Count; i++)
        {
            var dot = Instantiate(drawDotPrefab, drawDotData[i].Position, Quaternion.identity);
            dotsInStage.Add(dot);
            playerFingerDraw.onClearDraw += dot.OnReset;
            if (i > 0)
            {
                var line = Instantiate(lineDotConnect);
                line.SetPosition(0, dotsInStage[i - 1].transform.position);
                line.SetPosition(1, dot.transform.position);
            }
        }
    }

    public void Setup(PictureData data)
    {
        if (data.bound_width > 0f)
            checkRadius = data.bound_width;
        //load draw dot position
        string libraryToLoad = data.name;
        DrawDotLibrary gl = new DrawDotLibrary(libraryToLoad, true, true);
        if (gl.Library.Count == 0)
            Debug.LogError($"{data.name} has 0 gestures");
        else
        {
            List<Vector2> pointsList = gl.Library[0].Points;
            for (int i = 0; i < pointsList.Count; i++)
            {
                var dot = Instantiate(drawDotPrefab, pointsList[i] + new Vector2(0f, Const.ADDITION_PLAY_FIELD_Y_WORLD), Quaternion.identity);
                dot.Setup(checkRadius);
                dotsInStage.Add(dot);
                //playerFingerDraw.onClearDraw += dot.OnReset;
                //if (i > 0)
                //{
                //    var line = Instantiate(lineDotConnect);
                //    line.SetPosition(0, dotsInStage[i - 1].transform.position);
                //    line.SetPosition(1, dot.transform.position);
                //}
            }
            onGestureLibraryLoaded?.Invoke(this, gl);
        }
    }

    public void OnDrawBegin(object sender, Vector3 pos)
    {
        drawStayInCorrectArea = CheckDrawMarchLine(pos);
    }

    public void CheckDrawCorrect(object sender, Vector3 pos)
    {
        if (drawStayInCorrectArea)
        {
            drawStayInCorrectArea = drawStayInCorrectArea && CheckDrawMarchLine(pos);
        }

        for (int i = 0; i < dotsInStage.Count; i++)
        {
            if (!dotsInStage[i].isCorrect) CheckDrawCorrectPoint(i, pos);
        }

        //Check correct following line, only allow correct in sequence forward or backward
        /* if (lastCorrectDotID == -1)
        {
            for (int i = 0; i < dotsInStage.Count; i++)
            {
                if (CheckDrawCorrectPoint(i, pos)) break;
            }
        }
        else if (drawingDirection == 0)
        {
            //check drawing direction
            //int prevPoint = lastCorrectDotID - 1 >= 0 ? lastCorrectDotID - 1 : dotsInStage.Count - 1;
            //int nextPoint = lastCorrectDotID + 1 < dotsInStage.Count ? lastCorrectDotID + 1 : 0;
            int prevPoint = (int)MathModulo.Modulo(lastCorrectDotID - 1, dotsInStage.Count);
            int nextPoint = (int)MathModulo.Modulo(lastCorrectDotID + 1, dotsInStage.Count);
            if (CheckDrawCorrectPoint(prevPoint, pos))
            {
                drawingDirection = -1;
                UpdateErrorRadius(pos);
            }
            else if (CheckDrawCorrectPoint(nextPoint, pos))
            {
                drawingDirection = 1;
                UpdateErrorRadius(pos);
            }
        }
        else
        {
            int nextPoint = GetNextPointID();
            if (CheckDrawCorrectPoint(nextPoint, pos))
                UpdateErrorRadius(pos);
        }*/
    }

    bool CheckDrawMarchLine(Vector3 pos)
    {
        Vector3 currentCheckPoint = dotsInStage[0].transform.position;
        bool drawIsCorrect = false;
        for (int i = 0; i < dotsInStage.Count; i++)
        {
            drawIsCorrect = false;
            do
            {
                currentCheckPoint = Vector3.MoveTowards(currentCheckPoint, dotsInStage[i].transform.position, 0.05f);
                float sqrDistToCheckPoint = (pos - currentCheckPoint).sqrMagnitude;
                if (sqrDistToCheckPoint < CheckRadiusSqr)
                {
                    //draw pos is valid
                    drawIsCorrect = true;
                    break;
                }
            }
            while ((currentCheckPoint - dotsInStage[i].transform.position).sqrMagnitude > 0.01f);
            currentCheckPoint = dotsInStage[i].transform.position;
            if (drawIsCorrect) break;
        }
        if (!drawIsCorrect)
        {
            GameDrawController.instance.SetMessage("wrong, draw point has left correct area");
        }
        return drawIsCorrect;
    }

    int GetNextPointID() => (int)MathModulo.Modulo(lastCorrectDotID + drawingDirection, dotsInStage.Count);

    bool CheckDrawCorrectPoint(int dotIDToCheckFor, Vector3 pos)
    {
        float distanceSqr = Vector3.SqrMagnitude(dotsInStage[dotIDToCheckFor].transform.position - pos);
        if (distanceSqr <= CheckRadiusSqr)
        {
            dotsInStage[dotIDToCheckFor].SetCorrect();
            lastCorrectDotID = dotIDToCheckFor;
            //.Log(lastCorrectDotID);
            return true;
        }
        /*else if (distanceSqr > LimitErrorRadiusSqr && drawingDirection != 0)
        {
            //.Log($"Wrong. Checking dot {dotIDToCheckFor}, current dist {distanceSqr}, error radius {LimitErrorRadiusSqr}");
            debugTextWrongDraw.gameObject.SetActive(true);
        }
        else
        {
            LimitErrorRadiusSqr = distanceSqr;
        }*/
        return false;
    }

    void UpdateErrorRadius(Vector3 currentDrawPos)
    {
        LimitErrorRadiusSqr = Mathf.Max(defaultLimitErrorRadiusSqr,
                       (dotsInStage[GetNextPointID()].transform.position - currentDrawPos).sqrMagnitude + defaultLimitErrorRadiusSqr);
    }

    public void ToggleDebugDrawRadius(bool value)
    {
        foreach (var item in dotsInStage)
        {
            item.ToggleSpriteRenderer(value);
        }
    }

    void OnReset(object sender, System.EventArgs args)
    {
        lastCorrectDotID = -1;
        drawingDirection = 0;
        //debugTextWrongDraw.gameObject.SetActive(false);
        LimitErrorRadiusSqr = defaultLimitErrorRadiusSqr;
        foreach (var item in dotsInStage)
        {
            item.OnReset(this, null);
        }
    }

    public void JudgeScore(object sender, System.EventArgs args)
    {
        bool hasCrossedAllDots = true;
        int crossedDot = 0;
        for (int i = 0; i < dotsInStage.Count; i++)
        {
            hasCrossedAllDots = hasCrossedAllDots && dotsInStage[i].isCorrect;
            if (dotsInStage[i].isCorrect) crossedDot++;
        }
        float score;
        score = (float)crossedDot / dotsInStage.Count;
        if (!drawStayInCorrectArea)
        {
            score = 0f;
        }
        if (drawStayInCorrectArea && hasCrossedAllDots)
        {
            Debug.Log("PASSED");
        }
        else
        {
            Debug.Log("FAILED");
        }
        onDrawScore?.Invoke(score);
        OnReset(this, null);
    }

    private void OnDrawGizmos()
    {
        if (dotsInStage != null)
            foreach (var item in dotsInStage)
            {
                Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                Gizmos.DrawWireSphere(item.transform.position, checkRadius);
            }
    }
}
