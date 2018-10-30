using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * レーザーポインターを出すクラス
 */
public class LaserPointer : MonoBehaviour
{

    [SerializeField]
    private Transform _RightHandAnchor; // 右手

    [SerializeField]
    private Transform _LeftHandAnchor;  // 左手

    [SerializeField]
    private Transform _CenterEyeAnchor; // 目の中心

    [SerializeField]
    private float _MaxDistance = 100.0f; // 距離

    [SerializeField]
    private LineRenderer _LaserPointerRenderer; // LineRenderer

    // コントローラー
    private Transform Pointer
    {
        get
        {
            // 現在アクティブなコントローラーを取得
            var controller = OVRInput.GetActiveController();
            if (controller == OVRInput.Controller.RTrackedRemote)
            {
                return _RightHandAnchor;
            }
            else if (controller == OVRInput.Controller.LTrackedRemote)
            {
                return _LeftHandAnchor;
            }
            // どちらも取れなければ目の間からビームが出る
            return _CenterEyeAnchor;
        }
    }

    void Update()
    {
        var pointer = Pointer; // コントローラーを取得
                               // コントローラーがない or LineRendererがなければ何もしない
        if (pointer == null || _LaserPointerRenderer == null)
        {
            return;
        }
        // コントローラー位置からRayを飛ばす
        Ray pointerRay = new Ray(pointer.position, pointer.forward);

        // レーザーの起点
        _LaserPointerRenderer.SetPosition(0, pointerRay.origin);

        RaycastHit hitInfo;
        if (Physics.Raycast(pointerRay, out hitInfo, _MaxDistance))
        {
            // Rayがヒットしたらそこまで
            _LaserPointerRenderer.SetPosition(1, hitInfo.point);

            /* ---以下追加した部分--- */
            // ヒットしたオブジェクトを取得
            GameObject obj = hitInfo.collider.gameObject;
            // ヒットしたオブジェクトのScaleを取得
            Vector3 scale = obj.transform.localScale;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                // トリガーボタンを押した時
                Vector3 maxScale = new Vector3(5f, 5f, 5f);
                // スケールの各値が5より小さい場合は0.1大きくする
                if (scale.sqrMagnitude < maxScale.sqrMagnitude)
                {
                    obj.transform.localScale = new Vector3(scale.x + 0.1f, scale.y + 0.1f, scale.z + 0.1f);
                }
            }
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))
            {
                // タッチパッドボタンを押した時
                Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
                // スケールの各値が0.5より大きい場合は0.1小さくする
                if (scale.sqrMagnitude > minScale.sqrMagnitude)
                {
                    obj.transform.localScale = new Vector3(scale.x - 0.1f, scale.y - 0.1f, scale.z - 0.1f);
                }
            }
            /* ---追加した部分ここまで--- */
        }
        else
        {
            // Rayがヒットしなかったら向いている方向にMaxDistance伸ばす
            _LaserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _MaxDistance);
        }
    }
}