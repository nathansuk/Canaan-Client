using Newtonsoft;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

public class MountPart
{
    public string MountName;
    private Dictionary<string, float[]> _mountPositionsX;
    private Dictionary<string, float[]> _mountPositionsY;

    public MountPart(string mountName)
    {
        MountName = mountName;
        _mountPositionsX = new Dictionary<string, float[]>();
        _mountPositionsY = new Dictionary<string, float[]>();
        createMount();
    }

    public void createMount()
    {

        string imagePath = "Assets/avatar/animations/" + MountName + "/map/mount_position.json";
        string json = File.ReadAllText(imagePath);

        SpritePositionData spritePositionData = JsonConvert.DeserializeObject<SpritePositionData>(json);

        foreach (KeyValuePair<string, DirectionData> direction in spritePositionData.directions)
        {
            _mountPositionsX[direction.Key] = direction.Value.x;
            _mountPositionsY[direction.Key] = direction.Value.y;
        }

    }

    public Vector3 getFacePositionVec(string direction, int frame)
    {
        return new Vector3(_mountPositionsX[direction][frame], _mountPositionsY[direction][frame], 0f);
    }


}