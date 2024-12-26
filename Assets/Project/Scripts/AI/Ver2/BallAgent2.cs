using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent2 : Agent
{
    private Rigidbody ballRigidbody;
    private int stepCount; // ステップ数を追跡

    public override void Initialize()
    {
        ballRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ステップ数をリセット
        stepCount = 0;

        transform.localPosition = new Vector3(0, 2, 0);
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ボールの位置を観測データとして収集
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        Vector3 force = new Vector3(actionBuffers.ContinuousActions[0], 0, actionBuffers.ContinuousActions[1]) * 2f;
        ballRigidbody.AddForce(force);

        // 報酬関数
        if (transform.localPosition.y < -8f)
        {
            SetReward(1f);
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
        // 手動操作用
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}