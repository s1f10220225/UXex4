using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent3 : Agent
{
    private Transform groundTransform;
    private Rigidbody ballRigidbody;
    private int stepCount; // ステップ数を追跡
    private EnvironmentParameters resetParams; // 環境パラメータを取得

    public override void Initialize()
    {
        ballRigidbody = GetComponent<Rigidbody>();
        groundTransform = transform.parent.Find("Ground");
        resetParams = Academy.Instance.EnvironmentParameters;
    }

    public override void OnEpisodeBegin()
    {
        // ボールの位置・回転・速度をリセット
        transform.localPosition = new Vector3(0, 2, 0);
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;

        // レッスンに応じて床の設定を変更
        int currentLesson = (int)resetParams.GetWithDefault("lesson", 1);
        switch (currentLesson)
        {
            case 1:
                // レッスン1: 通常の床
                groundTransform.rotation = Quaternion.identity;
                break;
            case 2:
                // レッスン2: ランダムな傾きの床
                float angleX = Random.Range(-40f, 40f);
                float angleZ = Random.Range(-40f, 40f);
                groundTransform.rotation = Quaternion.Euler(angleX, 0, angleZ);
                break;
            case 3:
                // レッスン3: ボールの進行方向に応じて傾く床
                // この設定はOnActionReceived内で動的に変更
                break;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 地面の傾き状態やボールの位置、速度を観測する
        sensor.AddObservation(groundTransform.rotation);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(ballRigidbody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 force = new Vector3(actionBuffers.ContinuousActions[0], 0, actionBuffers.ContinuousActions[1]) * 2f;
        ballRigidbody.AddForce(force);

        if ((int)resetParams.GetWithDefault("lesson", 1) == 3)
        {
            // ボールの進行方向に応じて床を傾ける
            Vector3 tilt = new Vector3(-ballRigidbody.velocity.x, 0, -ballRigidbody.velocity.z) * Random.Range(0f, 20f);
            groundTransform.Rotate(tilt);
            // 回転を一定角度に制限
            Vector3 clampedRotation = transform.localRotation.eulerAngles;
            clampedRotation.x = Mathf.Clamp(clampedRotation.x, -30f, 30f);
            clampedRotation.z = Mathf.Clamp(clampedRotation.z, -30f, 30f);
            groundTransform.localRotation = Quaternion.Euler(clampedRotation);
        }

        // 報酬関数
        if (transform.localPosition.y < -11f)
        {
            AddReward(1f);
            EndEpisode();
        }
        else
        {
            AddReward(-0.1f / MaxStep);
        }

        // エピソードを一定時間で終了
        if (stepCount >= MaxStep)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
