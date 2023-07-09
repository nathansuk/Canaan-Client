using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EquipmentPart
{
    public Dictionary<string, GameObject> AvatarEquipment { get; set; }

    public Dictionary<string, Sprite> EquipmentSprites { get; set; }

    private static readonly string[] SPRITES_DIRECTIONS =
{
        "down_0",
        "down_1",
        "down_2",
        "down_3",

        "down_right_0",
        "down_right_1",
        "down_right_2",
        "down_right_3",

        "idle_down_0",
        "idle_down_right_0",
        "idle_right_0",
        "idle_up_0",
        "idle_up_right_0",

        "right_0",
        "right_1",
        "right_2",
        "right_3",

        "up_0",
        "up_1",
        "up_2",
        "up_3",

        "up_right_0",
        "up_right_1",
        "up_right_2",
        "up_right_3"

    };

    public EquipmentPart()
    {
        AvatarEquipment = new Dictionary<string, GameObject>();
    }

    public void AddEquipment(string equipmentName, int sortingOrder)
    {
        GameObject equipment = CreateEquipment(equipmentName, sortingOrder);
        AvatarEquipment.Add(equipmentName, equipment);
    }

    /**
     * Create a Gameobject by finding the correct assets
     */
    public GameObject CreateEquipment(string partName, int sortingOrder)
    {

        string imagePath = "Assets/avatar/animations/" + partName + "/map/";

        foreach (string positions in SPRITES_DIRECTIONS)
        {
            if (positions.Contains("left"))
            {
                string reflectedDirection = positions.Replace("left", "right");

                Sprite equipmentSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + positions + ".png");
                EquipmentSprites.Add(key: partName + '/' + positions, value: equipmentSprite);

            }
            else
            {

                Sprite equipmentSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + positions + ".png");
                EquipmentSprites.Add(key: partName + '/' + positions, value: equipmentSprite);
            }

        }

        GameObject equipment = new GameObject();
        equipment.AddComponent<SpriteRenderer>();

        equipment.GetComponent<SpriteRenderer>().sprite = EquipmentSprites[partName + "/down_right_0"];
        equipment.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;


        return equipment;
    }

    public void Animate(string direction)
    {

        


    }





}