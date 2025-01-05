using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UdpConnectionHandler : MonoBehaviour
{
    // UDP受信を監視するターゲットポート
    [SerializeField]
    private int targetPort = 12351;

    // チェック間隔（秒）
    [SerializeField]
    private float checkInterval = 1.0f;

    // MountainGameManager の参照
    [SerializeField]
    private MountainGameManager gameManager;

    // UDPレシーバー参照
    private Mocopi.Receiver.Core.MocopiUdpReceiver udpReceiver;

    // 通信状態
    private bool isConnected = false;

    // チェックタスクのキャンセレーショントークン
    private CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        udpReceiver = new Mocopi.Receiver.Core.MocopiUdpReceiver(targetPort);
        cancellationTokenSource = new CancellationTokenSource();
        StartConnectionCheck(cancellationTokenSource.Token);
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        udpReceiver?.UdpStop();
    }

    private async void StartConnectionCheck(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                CheckConnection();
                await Task.Delay(TimeSpan.FromSeconds(checkInterval), token);
            }
        }
        catch (TaskCanceledException)
        {
            // タスクキャンセル時の例外は無視
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UdpConnectionHandler] Error in connection check: {ex.Message}");
        }
    }

    private void CheckConnection()
    {
        bool currentStatus = udpReceiver.IsRuning;

        if (currentStatus && !isConnected)
        {
            // 再接続時の処理
            isConnected = true;
            gameManager?.OnDeviceReconnected();
            Debug.Log("[UdpConnectionHandler] Device reconnected.");
        }
        else if (!currentStatus && isConnected)
        {
            // 切断時の処理
            isConnected = false;
            gameManager?.OnDeviceDisconnected();
            Debug.Log("[UdpConnectionHandler] Device disconnected.");
        }
    }
}
