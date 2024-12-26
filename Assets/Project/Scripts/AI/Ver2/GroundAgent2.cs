using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GroundAgent2 : Agent
{
    private Rigidbody ballRigidbody;
    private Transform ballTransform;
    private int stepCount; // ステップ数を追跡

    public override void Initialize()
    {
        // 並列用にボールを自動的に探す
        ballTransform = transform.parent.Find("Ball");
        ballRigidbody = ballTransform.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ステップ数をリセット
        stepCount = 0;

        // 床の位置をリセット
        transform.rotation = Quaternion.identity; // 地面の回転をリセット
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ボールの位置と速度を観測データとして収集
        sensor.AddObservation(ballTransform.localPosition);
        sensor.AddObservation(ballRigidbody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float tiltX = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1, 1);
        float tiltZ = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1, 1);

        Vector3 rotation = new Vector3(tiltX, 0, tiltZ) * 100;
        transform.Rotate(rotation * Time.deltaTime);

        // 回転を一定角度に制限
        Vector3 clampedRotation = transform.localRotation.eulerAngles;
        clampedRotation.x = Mathf.Clamp(clampedRotation.x, -40f, 40f);
        clampedRotation.z = Mathf.Clamp(clampedRotation.z, -40f, 40f);
        transform.localRotation = Quaternion.Euler(clampedRotation);

        // 報酬関数
        if (ballTransform.localPosition.y < -8f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            AddReward(0.1f / MaxStep);
        }

        // エピソードを一定時間で終了
        if (stepCount >= MaxStep)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 手動操作用
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}