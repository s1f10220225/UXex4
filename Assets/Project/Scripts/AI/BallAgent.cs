using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    Rigidbody ballRigidbody;
    public Transform ground;
    
    public override void Initialize()
    {
        // AgentのRigidBodyの参照の取得
        this.ballRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // エピソード開始時のリセット
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // センサーにより観測データを収集
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ground.rotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 受け取ったアクションに基づいてエージェントを制御
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        ballRigidbody.AddForce(controlSignal * 25);
        
        // 地面から落ちたら負の報酬を与えてエピソードを終了
        if (ballRigidbody.position.y < -5)
        {
            SetReward(-10.0f);
            EndEpisode();
        }
        else
        {
            // まだプレイできるなら正の報酬を与える
            SetReward(0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 手動でテストするためのアクションを定義
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}