using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ToDO:
// 3: Großes Thema: Automatische Kalibrierung anhand von Bewegungen mit HTC Vive

public class MeshDeformation : MonoBehaviour {

    public bool keepChangingProportions;
    public SkinnedMeshRenderer skin;
    Transform[] bones;
    Vector3[] bonesOriginalPosition; // store original positions of bones to compute relative enlargement. Important: store in Vector3[], which stores values themselves unlike Transform[], which is referential
    Quaternion[] TPose; // record angles of this character when in T-Pose (not always Quaternion.idendity)

    [Range(0.7f, 1.3f)]
    public float bodySize = 1;
    [Range(0.8f, 1.2f)]
    public float bodyCorpulent = 1;

    [Range(0.8f, 1.2f)]
    public float shoulderWidth = 1;
    [Range(0.9f, 1.1f)]
    public float hipWidth = 1;
    [Range(-0.2f, 0.2f)]
    public float waist_shape = 0;
    [Range(-0.2f, 0.2f)]
    public float hips_shape = 0;

    // arms and legs can be changed individually, but are changed individually in inspector for the sake of simplicity
    [Range(0.8f, 1.2f)]
    public float ArmSize = 1;
    [Range(0.8f, 1.2f)]
    public float ArmCorpulence = 1;
    float rightArmSize;
    float rightArmCorpulence;
    float leftArmSize;
    float leftArmCorpulence;

    [Range(0.8f, 1.2f)]
    public float LegSize = 1;
    [Range(0.8f, 1.2f)]
    public float LegCorpulence = 1;
    float rightLegSize;
    float rightLegCorpulence;
    float leftLegSize;
    float leftLegCorpulence;

    [Range(0.8f, 1.2f)]
    public float headSize1 = 1;
    [Range(0.8f, 1.2f)]
    public float headSize2 = 1;
    
    // for none-bone based deformation
    Mesh mesh;
    Vector3[] OriginalVertices;
    Vector3[] DisplacedVertices;
    Vector3 centroid_Hips; // will store middle of body at hips' height
    Vector3 centroid_Waist; // will store middle of body at waist's height


    // Use this for initialization
    void Start () {

        bones = new Transform[skin.bones.Length];
        TPose = new Quaternion[skin.bones.Length];

        bonesOriginalPosition = new Vector3[skin.bones.Length];
        for (int i = 0; i < skin.bones.Length; i++)
        {
            bonesOriginalPosition[i] = skin.bones[i].position;
            bones[i] = skin.bones[i];
            TPose[i] = bones[i].rotation;
        }


        // none-bone-based
        mesh = (Mesh)Instantiate(skin.sharedMesh);
        skin.sharedMesh = mesh;
        OriginalVertices = mesh.vertices;

        centroid_Hips = Centroid_Mesh_HeightY(OriginalVertices, bones[0].position.y, 0.02f);
        centroid_Waist = Centroid_Mesh_HeightY(OriginalVertices, bones[2].position.y, 0.02f);
        
    }
	
	void Update () {

        for (int i = 0; i < bones.Length; i++) // bring character into T-Pose, so that deformation is applied correctly. Otherwise character can still be in the pose
            // created by IK in LateUpdate()
        {
            bones[i].rotation = TPose[i];
        }

        if (keepChangingProportions)
        {
            SetIndividualLimbsToCommonValue();
            ApplyBoneBasedDistortion();
            ApplyNotBoneBasedBumps();
            mesh.RecalculateBounds();
        }
    }


    void SetIndividualLimbsToCommonValue()
    {
        rightArmSize = ArmSize;
        leftArmSize = ArmSize;
        rightArmCorpulence = ArmCorpulence;
        leftArmCorpulence = ArmCorpulence;
        rightLegSize = LegSize;
        leftLegSize = LegSize;
        rightLegCorpulence = LegCorpulence;
        leftLegCorpulence = LegCorpulence;
    }

    void ApplyBoneBasedDistortion()
    {
        for (int i = 0; i < skin.bones.Length; i++) // first set to original... 
        {
            skin.bones[i].position = bonesOriginalPosition[i];
        }


        for (int i = 1; i < 54; i++) // bodySize
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * bodySize;
        }
        skin.bones[0].localScale = new Vector3(bodyCorpulent * bodySize, bodyCorpulent * bodySize, bodyCorpulent * bodySize);

        // shoulder width
        Vector3 rightShoulderPos = skin.bones[6].position;
        Vector3 leftShoulderPos = skin.bones[26].position;
        skin.bones[6].position = leftShoulderPos + (skin.bones[6].position - leftShoulderPos) * shoulderWidth; // clavicles are not offsetted relative to parent, but to each other
        skin.bones[26].position = rightShoulderPos + (skin.bones[26].position - rightShoulderPos) * shoulderWidth;
        skin.bones[7].position = skin.bones[7].parent.position + (skin.bones[7].position - skin.bones[7].parent.position) * shoulderWidth; // shoulders are again offsetted relative to parents
        skin.bones[27].position = skin.bones[27].parent.position + (skin.bones[27].position - skin.bones[27].parent.position) * shoulderWidth;

        // hips width
        Vector3 rightHipPos = skin.bones[46].position;
        Vector3 leftHipPos = skin.bones[50].position;
        skin.bones[46].position = leftHipPos + (skin.bones[46].position - leftHipPos) * hipWidth;
        skin.bones[50].position = rightHipPos + (skin.bones[50].position - rightHipPos) * hipWidth;


        for (int i = 7; i < 25; i++) // right Arm
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * rightArmSize;
        }
        skin.bones[6].localScale = new Vector3(rightArmCorpulence * rightArmSize, rightArmCorpulence * rightArmSize, rightArmCorpulence * rightArmSize);


        for (int i = 27; i < 45; i++) // left Arm
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * leftArmSize;
        }
        skin.bones[26].localScale = new Vector3(leftArmCorpulence * leftArmSize, leftArmCorpulence * leftArmSize, leftArmCorpulence * leftArmSize);


        for (int i = 47; i < 49; i++) // right Leg
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * rightLegSize;
        }
        skin.bones[46].localScale = new Vector3(rightLegCorpulence * rightLegSize, rightLegCorpulence * rightLegSize, rightLegCorpulence * rightLegSize);


        for (int i = 51; i < 53; i++) // left Leg
        {
            skin.bones[i].position = skin.bones[i].parent.position + (skin.bones[i].position - skin.bones[i].parent.position) * leftLegSize;
        }
        skin.bones[50].localScale = new Vector3(leftLegCorpulence * leftLegSize, leftLegCorpulence * leftLegSize, leftLegCorpulence * leftLegSize);


        // head
        skin.bones[5].position = skin.bones[5].parent.position + (skin.bones[5].position - skin.bones[5].parent.position) * headSize1;
        skin.bones[4].localScale = new Vector3(headSize2 * headSize1, headSize2 * headSize1, headSize2 * headSize1);
    }

    // see discussion at https://forum.unity.com/threads/manually-updating-a-skinned-mesh.67047/
    void ApplyNotBoneBasedBumps()
    {
        float whip = 8; // size of effect of bump // // HIER EINFACH WERT EINGESETZT
        DisplacedVertices = new Vector3[OriginalVertices.Length]; // first simply take original vertices as baseline
        for (int i = 0; i < OriginalVertices.Length; i++)
        {
            DisplacedVertices[i] = OriginalVertices[i];
        }

        for (int i = 0; i < DisplacedVertices.Length; i++)
        {
            // first Apply Hip Bump
            float distance = Mathf.Abs(DisplacedVertices[i].y - centroid_Hips.y); 
            if (distance < 0.5f * Mathf.PI / whip) 
            {
                float stretch = hips_shape * Mathf.Sin(distance * whip + 0.5f*Mathf.PI);
                DisplacedVertices[i].x = (OriginalVertices[i].x - centroid_Hips.x) * (stretch + 1) + centroid_Hips.x;
                DisplacedVertices[i].z = (OriginalVertices[i].z - centroid_Hips.z) * (stretch * 0.5f + 1) + centroid_Hips.z; // HIER EINFACH WERT EINGESETZT: Hüften gehen 2* so stark in die Breite wie in die Tiefe
            }

            // then apply waist bump
            float wwaist = 6;
            distance = Mathf.Abs(DisplacedVertices[i].y - centroid_Waist.y);
            if (distance < 0.5f * Mathf.PI / wwaist)
            {
                float stretch = waist_shape * Mathf.Sin(distance * wwaist + 0.5f * Mathf.PI);
                DisplacedVertices[i].x = (DisplacedVertices[i].x - centroid_Waist.x) * (stretch + 1) + centroid_Waist.x;
                DisplacedVertices[i].z = (DisplacedVertices[i].z - centroid_Waist.z) * (stretch + 1) + centroid_Waist.z;
            }

        }

        mesh.vertices = DisplacedVertices;
    }

    // takes a mesh vertices, selects those within a certain hight range, computes average x and z coordinates of that selection. This is then called the centroid at that height. 
    public Vector3 Centroid_Mesh_HeightY(Vector3[] vertices, float height, float heightrange)
    {
        Vector3 result = Vector3.zero;
        List<float> Point_X = new List<float>();
        List<float> Point_Z = new List<float>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Abs(vertices[i].y - height) < heightrange)
            {
                Point_X.Add(vertices[i].x);
                Point_Z.Add(vertices[i].z);
            }
        }

        for (int i = 0; i < Point_X.Count; i++) { result.x += Point_X[i]; }
        result.x = result.x / Point_X.Count;

        for (int i = 0; i < Point_Z.Count; i++) { result.z += Point_Z[i]; }
        result.z = result.z / Point_Z.Count;

        result.y = height;

        return result;
    }
}
