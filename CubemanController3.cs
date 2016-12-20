using UnityEngine;
using System;
using System.Collections;

public class CubemanController : MonoBehaviour
{
    public bool MoveVertically = false;
    public bool MirroredMovement = false;

    //public GameObject debugText;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Shoulder_Center;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;

    public LineRenderer SkeletonLine;

    private GameObject[] bones;
    private LineRenderer[] lines;
    private int[] parIdxs;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialPosOffset = Vector3.zero;
    private uint initialPosUserID = 0;

    enum Direction
    {
        verticle, horizontal, slash
    }

    enum Body : int
    {
        Hip_Center, Spine, Shoulder_Center, Head,  // 0 - 3
        Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,  // 4 - 7
        Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,  // 8 - 11
        Hip_Left, Knee_Left, Ankle_Left, Foot_Left,  // 12 - 15
        Hip_Right, Knee_Right, Ankle_Right, Foot_Right  // 16 - 19
    }

    /*enum hDir
    {
        left, right, none;
    }*/

    enum vDir
    {
        up, down, none
    }

    //下面是一些常用函釋
    Boolean isBodyStraight() //身體是否為直
    {
        Boolean positionCorrect;
        positionCorrect = direction(getSlopeByBodyPoint(Body.Shoulder_Center, Body.Hip_Center)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hip_Right, Body.Ankle_Right)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hip_Left, Body.Ankle_Left)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
        return true;
    }

    Direction direction(float slope)//2 straight, 1 horizontal, 0 
    {
        if (Math.Abs(slope) < 0.5)
            return Direction.horizontal;
        if (Math.Abs(slope) > 2.5)
            return Direction.verticle;
        return Direction.slash;
    }

    int LeftRight(int i)
    {
        if (i >= 4 && i <= 7)//left
        {
            return -1;
        }
        return 1;
    }

    vDir UpDown(float slope, int i)
    {
        if (slope * LeftRight(i) > 0)//up
        {
            return vDir.up;
        }
        return vDir.down;
    }

    void detect()
    {
        Boolean correctPostition = false;
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                if (bones[i].gameObject.activeSelf)
                {
                    Vector3 posJoint = bones[i].transform.localPosition;
                    // Vector3 posJoint = bones[i].transform.TransformPoint(bones[i].transform.position);

                    int parI = parIdxs[i];
                    Vector3 posParent = bones[parI].transform.localPosition;
                    // Vector3 posParent = bones[parI].transform.TransformPoint(bones[parI].transform.position);
                    //Vector3 test = bones[i].transform.TransformPoint(bones[parI].transform.position);
                    if (bones[parI].gameObject.activeSelf)
                    {

                        //lines[i].SetVertexCount(2);

                        lines[i].gameObject.SetActive(true);
                        lines[i].SetPosition(0, posParent);
                        lines[i].SetPosition(1, posJoint);
                        //Debug.Log("posParent=" + posParent * 100 + ",posJoint= " + posJoint * 100);
                        float x = posJoint.x - posParent.x;
                        float y = posJoint.y - posParent.y;
                        float slope = y / x;
                        //Debug.Log("slope=" + slope);

                        switch (i)
                        {
                            case 2:     //shoulder
                                correctPostition = (direction(slope) == Direction.verticle) ? true : false;
                                Debug.Log("isCorrect=" + correctPostition + "   shoulder=" + slope);
                                break;
                            case 6:    //Wrist_Left
                                correctPostition = ((direction(slope) == Direction.horizontal) && correctPostition) ? true : false;
                                Debug.Log("isCorrect=" + correctPostition + "   left=" + slope);

                                break;
                            case 10:    //Wrist_Right
                                correctPostition = ((direction(slope) == Direction.slash) && (UpDown(slope, i) == vDir.up) && correctPostition) ? true : false;
                                Debug.Log("isCorrect=" + correctPostition + "   right=" + slope);
                                break;

                        }

                        //Debug.Log("test=" + test);
                        //Debug.Log("s=" + Vector3.Angle(posJoint, posParent));

                    }

                }
            }
        }
        Debug.Log("isCorrect=" + correctPostition);
    }


    float getSlopeByBodyPoint(Body firstPoint, Body secondPoint)    //傳入想判斷斜率的兩個點
    {
        Vector3 posJoint = bones[(int)firstPoint].transform.localPosition;
        Vector3 posParent = bones[(int)secondPoint].transform.localPosition;

        float x = posJoint.x - posParent.x;
        float y = posJoint.y - posParent.y;
        float slope = y / x;
        return slope;
    }

    //字的Libary從這裡開始加入謝謝

    Boolean isRonWord()  //卍字正確
    {
        Boolean positionCorrect;
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Right, Body.Elbow_Right)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Elbow_Left)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Elbow_Left, Body.Shoulder_Left)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Elbow_Right, Body.Shoulder_Right)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };

        positionCorrect = isBodyStraight();
        if (!positionCorrect)
        {
            return false;
        };
        Debug.Log(">>right!!!");    //不debug時可註解此行
        return true;
    }

    Boolean isCardWord() //是卡字
    { 
        Boolean positionCorrect;
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left)).Equals(Direction.slash);
        if (!positionCorrect)
        {
            return false;
        };

        positionCorrect = isBodyStraight();
        if (!positionCorrect)
        {
            return false;
        };
        Debug.Log(">>卡right!!!");    //不debug時可註解此行
        return true;
    }

    Boolean isSkyWord()  //天字正確
    {
        Boolean positionCorrect;
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hip_Left, Body.Ankle_Left)).Equals(Direction.slash);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hip_Right, Body.Ankle_Right)).Equals(Direction.slash);
        if (!positionCorrect)
        {
            return false;
        };
        Debug.Log(">>sky right!!!");    //不debug時可註解此行
        return true;
    }

    Boolean isFireWord(int i)  //火字正確
    {
        float slope = getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right);
        Direction dir = direction(slope);
        vDir pos = UpDown(slope, i);

        float slope2 = getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left);
        Direction dir2 = direction(slope2);
        vDir pos2 = UpDown(slope2, i);

        if (dir.Equals(Direction.slash) && pos.Equals(vDir.up) && dir2.Equals(Direction.slash) && pos2.Equals(vDir.up))
        {
            Debug.Log(">>Fire right!!!");    //不debug時可註解此行
            return true;
        };
        return false;
    }
    Boolean isKingWord()//是王字
    {
        Boolean positionCorrect;
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };

        positionCorrect = isBodyStraight();
        if (!positionCorrect)
        {
            return false;
        };
        Debug.Log(">>王right!!!");    //不debug時可註解此行
        return true;
    }

    Boolean isMiWord()//是米字
    {
        Boolean positionCorrect;
        //右手往斜上,左手往斜下
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right)).Equals(Direction.slash) &&
            UpDown(getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right), (int)(Body.Hand_Right)) == vDir.down &&
            direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left)).Equals(Direction.slash) &&
            UpDown(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left), (int)(Body.Hand_Left)) == vDir.up &&
            isBodyStraight();

        if (positionCorrect)
        {
            Debug.Log(">>米right!!!");    //不debug時可註解此行
            return true;
        };
        //右手往斜上,左腳往斜下
        positionCorrect = direction(getSlopeByBodyPoint(Body.Hip_Right, Body.Ankle_Right)).Equals(Direction.slash) &&
            direction(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left)).Equals(Direction.slash) &&
            UpDown(getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left), (int)(Body.Hand_Left)) == vDir.up;

        if (positionCorrect)
        {
            Debug.Log(">>米right!!!");    //不debug時可註解此行
            return true;
        };

        return false;
    }

    Boolean isWoodWord(int i)//木
    {
        float slopeRight = getSlopeByBodyPoint(Body.Hand_Right, Body.Shoulder_Right);
        Direction dirRight = direction(slopeRight);
        vDir posRight = UpDown(slopeRight, i);
    
        float slopeLeft = getSlopeByBodyPoint(Body.Hand_Left, Body.Shoulder_Left);
        Direction dirLeft = direction(slopeLeft);
        vDir posLeft = UpDown(slopeLeft, i);
    
        if(dir.Equals(Direction.slash) && pos.Equals(vDir.down) && dir2.Equals(Direction.slash) && pos2.Equals(vDir.down))
        {
            Debug.Log(">>木right!!!");    //不debug時可註解此行
            return true;
        };
    
        return false;
    }

    Boolean isHandWord()//手
    {
        Boolean positionCorrect;
    
        positionCorrect = direction(getSlopeByBodyPoint(Body.Shoulder_Right, Body.Elbow_Right)).Equals(Direction.verticle);
        if (!positionCorrect)
        {
            return false;
        };
    
        positionCorrect = direction(getSlopeByBodyPoint(Body.Elbow_Right, Body.Hand_Right)).Equals(Direction.horizntal);
        if (!positionCorrect)
        {
            return false;
        };

        positionCorrect = direction(getSlopeByBodyPoint(Body.Shoulder_Left, Body.Hand_Left)).Equals(Direction.horizontal);
        if (!positionCorrect)
        {
            return false;
        };
    
        positionCorrect = isBodyStraight();
        if (!positionCorrect)
        {
            return false;
        };
    
        Debug.Log(">>手right!!!");    //不debug時可註解此行
        return true;

    }

    //下面開始不是我們打的code

    void Start()
    {
        //store bones in a list for easier access
        bones = new GameObject[] {
            Hip_Center, Spine, Shoulder_Center, Head,  // 0 - 3
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,  // 4 - 7
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,  // 8 - 11
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,  // 12 - 15
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right  // 16 - 19
		};

        parIdxs = new int[] {
            0, 0, 1, 2,
            2, 4, 5, 6,
            2, 8, 9, 10,
            0, 12, 13, 14,
            0, 16, 17, 18
        };

        // array holding the skeleton lines
        lines = new LineRenderer[bones.Length];

        if (SkeletonLine)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Instantiate(SkeletonLine) as LineRenderer;
                lines[i].transform.parent = transform;
            }
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //transform.rotation = Quaternion.identity;
    }



    // Update is called once per frame
    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        // get 1st player
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        if (playerID <= 0)
        {
            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;

                if (SkeletonLine)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }

            return;
        }

        // set the user position in space
        Vector3 posPointMan = manager.GetUserPosition(playerID);
        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;

        // store the initial position
        if (initialPosUserID != playerID)
        {
            initialPosUserID = playerID;
            initialPosOffset = transform.position - (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));
        }

        transform.position = initialPosOffset + (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));

        // update the local positions of the bones
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i) : i;

                if (manager.IsJointTracked(playerID, joint))
                {
                    bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(playerID, joint);
                    posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;

                    Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, !MirroredMovement);
                    rotJoint = initialRotation * rotJoint;

                    posJoint -= posPointMan;

                    if (MirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }
                    //Debug.Log(posJoint);
                    bones[i].transform.localPosition = posJoint;
                    bones[i].transform.rotation = rotJoint;
                }
                else
                {
                    bones[i].gameObject.SetActive(false);
                }
            }
        }

        if (SkeletonLine)
        {
            //detect();

            for (int i = 0; i < bones.Length; i++)
            {
                bool bLineDrawn = false;

                if (bones[i] != null)
                {
                    if (bones[i].gameObject.activeSelf)
                    {
                        //isCardWord();    //哲亞加的 可刪除
                        //isSkyWord();
                        isFireWord(i);
                        Vector3 posJoint = bones[i].transform.localPosition;
                        // Vector3 posJoint = bones[i].transform.TransformPoint(bones[i].transform.position);

                        int parI = parIdxs[i];
                        Vector3 posParent = bones[parI].transform.localPosition;
                        // Vector3 posParent = bones[parI].transform.TransformPoint(bones[parI].transform.position);
                        //Vector3 test = bones[i].transform.TransformPoint(bones[parI].transform.position);
                        if (bones[parI].gameObject.activeSelf)
                        {

                            //lines[i].SetVertexCount(2);
                            if (i == 6)
                            {
                                lines[i].gameObject.SetActive(true);
                                lines[i].SetPosition(0, posParent);
                                lines[i].SetPosition(1, posJoint);
                                // Debug.Log("posParent=" + posParent * 100+ ",posJoint= "+ posJoint * 100);
                                float x = posJoint.x - posParent.x;
                                float y = posJoint.y - posParent.y;
                                float slope = y / x;
                                // Debug.Log("slope="+slope);
                                //Debug.Log("test=" + test);
                                //Debug.Log("s=" + Vector3.Angle(posJoint, posParent));
                            }
                            bLineDrawn = true;
                        }
                    }
                }

                if (!bLineDrawn)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
        }

    }

}
