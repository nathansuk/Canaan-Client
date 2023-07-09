using Newtonsoft;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UIElements;
using Pathfinding;

public class PlayerComponent : MonoBehaviour
{
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

        "down_left_0",
        "down_left_1",
        "down_left_2",
        "down_left_3",
        
        "idle_down_0",
        "idle_down_right_0",
        "idle_down_left_0",
        "idle_right_0",
        "idle_left_0",
        "idle_up_0",
        "idle_up_right_0",
        "idle_up_left_0",

        "right_0",
        "right_1",
        "right_2",
        "right_3",

        "left_0",
        "left_1",
        "left_2",
        "left_3",
        
        "up_0",
        "up_1",
        "up_2",
        "up_3",
        
        "up_right_0",
        "up_right_1",
        "up_right_2",
        "up_right_3",

        "up_left_0",
        "up_left_1",
        "up_left_2",
        "up_left_3"

    };
    
    public GameObject player;

    public string playerName;
    public int playerLevel;

    public string aHairPart;
    public string aFacePart;
    public string aBodyPart;
    public string aLegsPart;
    public string aDirection; // Direction ([direction]_[sprite_number])

    public string aEquipmentChestPlate;
    public string aEquipmentGloves;
    public string aEquipmentPants;
    public string aEquipmentShoes;
    public string aCostume;
    public string aMount;


    public CostumePart costumePart;
    public MountPart mountPart;

    private Dictionary<string, Sprite> aSprites = new Dictionary<string,Sprite>();
    private Dictionary<string, GameObject> AvatarParts = new Dictionary<string, GameObject>();

    private bool isWalking = false;

    private Camera playerCamera;

    private Vector2 playerDestination;
    private AIPath aiPath;


    public void Walk()
    {
        isWalking = true;

        StopCoroutine(ToggleMountIdleAnimation());
        StopCoroutine(ToggleAvatarBounce(player.transform.position));

        StartCoroutine(ToggleWalkSprites());
        //StartCoroutine(moveAvatar());
    }

    public bool getIsWalking()
    {
        return isWalking;
    }

    public void setIsWalking(bool state)
    {
        isWalking = state;
    }

    public void StopWalking()
    {
        isWalking = false;
        StopCoroutine(ToggleWalkSprites());
        Idle();
    }

    public void setDirection(string direction)
    {
        Debug.Log("Changement de direction");
        aDirection = direction;
    }

    public void setCellPosition()
    {
        transform.position = GameObject.Find("GridPlayer").GetComponent<GridLayout>().WorldToCell(transform.position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.25f, transform.position.z);
    }


    // Créer un GameObject pour une partie du corps
    private GameObject CreateBodyPartGameObject(string partName, int sortingOrder, Vector3 localPosition)
    {

        string imagePath = "Assets/avatar/animations/" + partName + "/map/";

        foreach (string direction in SPRITES_DIRECTIONS)
        {
            string tempDirection = direction.Contains("left") ? direction.Replace("left", "right") : direction;
            Sprite partSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath + tempDirection + ".png");
            
            if(partSprite != null)
            {
                this.aSprites.Add(key: partName + '/' + direction, value: partSprite);
            }
        }

        GameObject bodyPart = new GameObject();
        bodyPart.AddComponent<SpriteRenderer>();

        if(partName.Contains("mount"))
        {
            bodyPart.GetComponent<SpriteRenderer>().sprite = aSprites[partName + "/"+ aDirection + "_0"];
        }
        else
        {
            bodyPart.GetComponent<SpriteRenderer>().sprite = aSprites[partName + "/idle_" + aDirection + "_0"];
        }
        bodyPart.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        bodyPart.name = this.player.transform.name + '_' + partName;
        bodyPart.transform.SetParent(this.player.transform);
        bodyPart.transform.localPosition = localPosition;

        bodyPart.GetComponent<SpriteRenderer>().flipX = aDirection.Contains("left");

        AvatarParts.Add(partName, bodyPart);

        return bodyPart;
    }
    
   
    // Start is called before the first frame update
    void Start()
    {
        setCellPosition();
        aiPath = GetComponent<AIPath>();
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<CameraScript>().FreezeCamera(player.transform.position);

        GameObject face = CreateBodyPartGameObject(this.aFacePart, 7, new Vector3(0.013f, 0.943f, 0f));

        if (this.aCostume != "")
        {
            GameObject costume = CreateBodyPartGameObject(this.aCostume, 8, new Vector3(0.044f, 0.68f, 0f));
            costumePart = new CostumePart(aCostume);
            costumePart.createCostume();
        }
        else
        {
            GameObject hair = CreateBodyPartGameObject(this.aHairPart, 8, new Vector3(0f, 1f, 0f));
            GameObject body = CreateBodyPartGameObject(this.aBodyPart, 4, new Vector3(0.01f, 0.68f, 0f));
            GameObject legs = CreateBodyPartGameObject(this.aLegsPart, 1, new Vector3(0.006f, 0.43f, 0f));

            GameObject chestplate = CreateBodyPartGameObject(this.aEquipmentChestPlate, 5, new Vector3(0.01f, 0.68f, 0f));
            GameObject gloves = CreateBodyPartGameObject(this.aEquipmentGloves, 6, new Vector3(-0.0014f, 0.591f, 0f));
            GameObject pants = CreateBodyPartGameObject(this.aEquipmentPants, 2, new Vector3(-0.012f, 0.55f, 0f));
            GameObject shoes = CreateBodyPartGameObject(this.aEquipmentShoes, 3, new Vector3(0.003f, 0.346f, 0f));

        }

        if (this.aMount != "")
        {
            GameObject mount = CreateBodyPartGameObject(this.aMount, 1, new Vector3(-0.19f, 0.32f, 0f));
            mountPart = new MountPart(aMount);
            mountPart.createMount();
        }

        Idle();
        ToggleMountIdleAnimation();

    }

    void Idle()
    {
        string idleDirection = "idle_" + aDirection;
        GameObject mount = GameObject.Find(this.player.transform.name + "_" + this.aMount);

        foreach (KeyValuePair<string, GameObject> avatarPart in AvatarParts)
        {
            if (avatarPart.Key.Contains("mount"))
            {
                mount.transform.localPosition = mountPart.getFacePositionVec(aDirection, 0);
            }
            else
            {
                avatarPart.Value.GetComponent<SpriteRenderer>().sprite = aSprites[avatarPart.Key + "/" + idleDirection + "_0"];
            }

            avatarPart.Value.GetComponent<SpriteRenderer>().flipX = aDirection.Contains("left");

            if (avatarPart.Key.Contains("costume"))
            {
                GameObject face = GameObject.Find(this.player.transform.name + "_" + this.aFacePart);
                face.transform.localPosition = costumePart.getFacePositionVec(idleDirection, 0);
            }
        }

        if(mountPart != null)
        {
            StartCoroutine(ToggleAvatarBounce(player.transform.position));
            StartCoroutine(ToggleMountIdleAnimation());
        }


    }

    IEnumerator ToggleMountIdleAnimation()
    {
        int mountFrame = 0;
        Vector3 avatarInitialPosition = player.transform.position;

        GameObject playerMount = GameObject.Find(this.player.transform.name + "_" + this.aMount);

        while (!isWalking)
        {
            playerMount.GetComponent<SpriteRenderer>().sprite = aSprites[this.aMount + "/" + aDirection + "_" + mountFrame];
            mountFrame = (mountFrame + 1) % 3;

            yield return new WaitForSeconds(0.2f);

        }

    }

    IEnumerator ToggleAvatarBounce(Vector3 initialPosition)
    {
        float duration = 0.15f;
        float elapsed = 0f; 

        while (!isWalking)
        {
            float bounceHeight = Mathf.Sin(Mathf.PI * elapsed / duration);
            player.transform.position = new Vector3(player.transform.position.x, initialPosition.y + bounceHeight * 0.01f, player.transform.position.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        player.transform.position = initialPosition;

    }

    public IEnumerator ToggleWalkSprites()
    {
        Debug.Log("ANIMATION AVATAR");
        GameObject face = GameObject.Find(this.player.transform.name + "_" + this.aFacePart);
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        int frame = 0;
        int mountFrame = 0;
        string direction = aDirection;

        while (isWalking)
        {
            //If mount we set idle position here
            string frameName = direction + "_" + frame;
            string mountFrameName  = direction + "_" + mountFrame;

            foreach (KeyValuePair<string, GameObject> avatarPart in AvatarParts)
            {
                if (avatarPart.Key.Contains("mount"))
                {
                    avatarPart.Value.GetComponent<SpriteRenderer>().sprite = aSprites[avatarPart.Key + "/" + mountFrameName];
                    avatarPart.Value.transform.localPosition = mountPart.getFacePositionVec(direction, mountFrame);
                }
                else
                {
                    if(!AvatarParts.ContainsKey(aMount))
                        avatarPart.Value.GetComponent<SpriteRenderer>().sprite = aSprites[avatarPart.Key + "/" + frameName];
                }
                
                avatarPart.Value.GetComponent<SpriteRenderer>().flipX = direction.Contains("left");

                if (avatarPart.Key.Contains("costume"))
                    face.transform.localPosition = costumePart.getFacePositionVec(direction, frame);
                    
            }


            frame = (frame + 1) % 4;
            mountFrame = (mountFrame + 1) % 3;


            yield return new WaitForSeconds(0.2f);
        }

    }


}
