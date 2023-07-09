using Newtonsoft;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

public class CostumePart
{

    public string CostumeName;
    private Dictionary<string, float[]> _facePositionX;
    private Dictionary<string, float[]> _facePositionY;

    public CostumePart(string costumeName)
    {
        CostumeName = costumeName;
        _facePositionX = new Dictionary<string, float[]>();
        _facePositionY = new Dictionary<string, float[]>();
        createCostume();
    }

    public void createCostume()
    {
        string imagePath = "Assets/avatar/animations/" + CostumeName + "/map/face_position.json";
        string json = File.ReadAllText(imagePath);

        SpritePositionData spritePositionData = JsonConvert.DeserializeObject<SpritePositionData>(json);

        foreach (KeyValuePair<string, DirectionData> direction in spritePositionData.directions)
        {
            _facePositionX[direction.Key] = direction.Value.x;
            _facePositionY[direction.Key] = direction.Value.y;
        }
    }

    public Vector3 getFacePositionVec(string direction, int frame)
    {
        return new Vector3(_facePositionX[direction][frame], _facePositionY[direction][frame], 0f);
    }












}