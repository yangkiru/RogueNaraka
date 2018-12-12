//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Shadow : MonoBehaviour
//{

//    public Unit origin;
//    public Animator animator;
//    public PixelArtRotation.PixelRotation pixelRotation;

//    [ContextMenu("Init")]
//    public void Init()
//    {
//        if (QualitySettings.GetQualityLevel() == 0)
//        {
//            gameObject.SetActive(false);
//            return;
//        }
//        animator.runtimeAnimatorController = origin.animator.runtimeAnimatorController;

//        transform.localPosition = origin.data.shadowPos;

//        pixelRotation.isRotate = origin.data.isStanding;
//        if (!Application.isPlaying && pixelRotation.isRotate)
//        {
//            pixelRotation.Awake();
//            pixelRotation.Rotate();
//        }
//    }
//}
