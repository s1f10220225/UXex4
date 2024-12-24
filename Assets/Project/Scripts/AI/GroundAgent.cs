using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class GroundAgent : Agent
{
    Transform ground;
    public GameObject ball;
    Rigidbody ball_Rigidbody;
    Transform ball_transform;

    public override void Initialize()
    {
        // AgentのRigidBodyの参照の取得
        ground = GetComponent<Transform>();
        ball_Rigidbody = ball.GetComponent<Rigidbody>();
        ball_transform = ball.GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        ground.rotation = Quaternion.identity; // 地面の回転をリセット
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 地面の傾き状態やボールの位置、速度を観測する
        sensor.AddObservation(ball.transform.localPosition);
        sensor.AddObservation(ball_Rigidbody.velocity);
        sensor.AddObservation(transform.rotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // アクションに基づいて地面を傾ける
        float tiltX = actions.ContinuousActions[0];
        float tiltZ = actions.ContinuousActions[1];

        Vector3 rotation = new Vector3(tiltX, 0, tiltZ) * 10; // 適切なスケールに調整
        transform.Rotate(rotation * Time.deltaTime);

        // 現在の回転角を取得して制限をかける
    Vector3 currentRotation = transform.rotation.eulerAngles;
    currentRotation.x = ClampAngle(currentRotation.x, -10f, 10f); // x軸の最大角度を設定
    currentRotation.z = ClampAngle(currentRotation.z, -10f, 10f); // z軸の最大角度を設定

        // ボールが落ちたら報酬を与える
        if (ball_transform.position.y < -5)
        {
            SetReward(10.0f);
            EndEpisode();
        }
        else
        {
            // 正の領域にいる場合は負の報酬を
            SetReward(-0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // テスト用の手動操作
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}