using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine.Unity;

[RequireComponent (typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    private const string ANIMATION_SINGLEARROW = "Attack";
    private const string ANIMATION_ULTIMATE = "Ultimate";
    public static PlayerController instanceController;
    
    static SkeletonAnimation skeletonAnimation;
    Spine.AnimationState animationState;

    public GameObject SwingParticle;
    public GameObject ShootingArrowVFX;
    public GameObject Explosion01;
    public GameObject ArrowInHandVFX;
    public GameObject Explosion02;
    public GameObject UltimateArrow;

    [SerializeField]
    int NumberOfArrowsOnUltimate = 3;

    Transform boneFollower;
    BoxCollider2D objCollider;
    
    public static PlayerState playerState
    {
        get;
        set;
    }
    
    void Awake()
    {
        objCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        instanceController = this;

        boneFollower = GetComponentsInChildren<Transform>()[1];
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        
        animationState.Event += OnEvent;
    }

    private void OnEvent(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        {
            case "OnSwingStart":
                Instantiate(SwingParticle, transform.position + new Vector3(0,0.5f), Quaternion.identity,transform);
                break;
            case "OnSwingEnd":
                Instantiate(ArrowInHandVFX, boneFollower.position, Quaternion.identity, boneFollower).transform.rotation = boneFollower.rotation;
                break;
            case "OnAttackFinished":
            case "OnUltimateFinished":
                skeletonAnimation.AnimationName = "Idle";
                playerState = PlayerState.Idle;
                break;
        }
    }
    
    private ArrowScript SpawnSingleArrow()
    {
        Spine.Bone bone = skeletonAnimation.skeleton.FindBone("arrow 1");

        if(bone != null)
        {
            Vector3 spawnPosition = new Vector2(bone.WorldX, bone.WorldY);
            GameObject arrow = new GameObject("Arrow");
            arrow.transform.position = transform.position + spawnPosition;
            ArrowScript arrowScript = arrow.AddComponent<ArrowScript>();
            return arrowScript;
        } else
        {
            return null;
        }
    }

    IEnumerator WaitForSingleArrowAnimation(GameObject targetObject)
    {
        yield return new WaitForSpineEvent(animationState, "OnTargetHit");
        
        Destroy(boneFollower.GetChild(0).gameObject);

        ArrowScript arrow = SpawnSingleArrow();
        if(arrow)
        {
            arrow.ShootSingle(targetObject, Explosion01);
            GameObject obj = Instantiate(ShootingArrowVFX, arrow.transform.position, arrow.transform.rotation, arrow.transform);
            obj.transform.LookAt(targetObject.transform.position + new Vector3(0, targetObject.GetComponent<BoxCollider2D>().bounds.size.y / 2));
            Instantiate(Explosion01, arrow.transform.position, arrow.transform.rotation);
        }
    }

    IEnumerator WaitForUltimateAnimation(GameObject[] enemies)
    {
        yield return new WaitForSpineEvent(animationState, "OnUltimateFired");
        Instantiate(Explosion02, boneFollower.transform.position, Quaternion.identity);
        foreach (var enemy in enemies)
        {
            for (int i = 0; i < NumberOfArrowsOnUltimate; i++)
            {
                ArrowScript arrow = instanceController.SpawnSingleArrow();
                if(arrow)
                {
                    Instantiate(UltimateArrow, arrow.transform.position, arrow.transform.rotation, arrow.transform).transform.LookAt(arrow.transform.position + new Vector3(1,0,0));
                    arrow.Ultimate(enemy.transform.position, Explosion01);
                }
            }
        }
    }

    public static void FireSingleArrow(GameObject targetObject)
    {
        skeletonAnimation.AnimationName = ANIMATION_SINGLEARROW;
        instanceController.StartCoroutine("WaitForSingleArrowAnimation", targetObject);
    }

    public static void UseUltimate()
    {
        skeletonAnimation.AnimationName = ANIMATION_ULTIMATE;
        instanceController.StartCoroutine("WaitForUltimateAnimation", GameMode.Instance.GetEnemiesInBattle());
    }

    void OnDrawGizmos()
    {
        if (objCollider)
        {
            Handles.Label(transform.position + new Vector3(0, objCollider.bounds.size.y + 1, 0), playerState.ToString());
        }
    }
}

public enum PlayerState
{
    Idle, 
    Aiming,
    Firing,
}

