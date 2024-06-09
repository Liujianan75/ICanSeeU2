using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public GameObject testPlane;
    private Material testPlaneMaterial;
    private float targetAlpha = 0.4f;
    private Coroutine changeAlphaCoroutine;
    private SkeletonManager skeletonManager;

    private void Start()
    {
        if (testPlane != null)
        {
            testPlaneMaterial = testPlane.GetComponent<Renderer>().material;
            SetAlpha(targetAlpha);
        }
        else
        {
            Debug.LogError("TestPlane is not assigned in the Inspector");
        }

        skeletonManager = SkeletonManager.instance;
        if (skeletonManager == null)
        {
            Debug.LogError("SkeletonManager instance not found.");
        }
    }

    private void Update()
    {
        UpdateHandGesture();
    }

    private void UpdateHandGesture()
    {
        if (skeletonManager != null)
        {
            var skeletonJoints = skeletonManager._listOfJoints;

            if (skeletonJoints != null && skeletonJoints.Count >= 21) // 验证是否有足够的关节点
            {
                // 验证手指指尖的索引是否正确
                Vector3 thumbTip = skeletonJoints[4].transform.position;  // 大拇指指尖索引为4
                Vector3 indexTip = skeletonJoints[8].transform.position;  // 食指指尖索引为8
                Vector3 middleTip = skeletonJoints[12].transform.position; // 中指指尖索引为12
                Vector3 ringTip = skeletonJoints[16].transform.position;   // 无名指指尖索引为16
                Vector3 pinkyTip = skeletonJoints[20].transform.position;  // 小指指尖索引为20

                float threshold = 0.03f;

                if (IsWithinDistance(thumbTip, indexTip, threshold) &&
                    IsWithinDistance(thumbTip, middleTip, threshold) &&
                    IsWithinDistance(thumbTip, ringTip, threshold) &&
                    IsWithinDistance(thumbTip, pinkyTip, threshold))
                {
                    if (changeAlphaCoroutine != null) StopCoroutine(changeAlphaCoroutine);
                    changeAlphaCoroutine = StartCoroutine(ChangeAlpha(1.0f));
                }
                else
                {
                    if (changeAlphaCoroutine != null) StopCoroutine(changeAlphaCoroutine);
                    changeAlphaCoroutine = StartCoroutine(ChangeAlpha(targetAlpha));
                }
            }
        }
    }

    private bool IsWithinDistance(Vector3 point1, Vector3 point2, float distance)
    {
        return Vector3.Distance(point1, point2) < distance;
    }

    private IEnumerator ChangeAlpha(float target)
    {
        float startAlpha = testPlaneMaterial.color.a;
        float duration = 1.0f; // 持续时间1秒
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, target, elapsed / duration);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(target);
    }

    private void SetAlpha(float alpha)
    {
        if (testPlaneMaterial != null)
        {
            Color color = testPlaneMaterial.color;
            color.a = alpha;
            testPlaneMaterial.color = color;
        }
    }
}
