using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mocopi.Receiver;

public class MocopiBVHRecorder : MonoBehaviour
{
    private MocopiAvatar mocopiAvatar;
    private StreamWriter bvhWriter;
    private string filePath = "recorded_motion.bvh";
    private float frameTime = 1.0f / 50.0f; // 50fps
    private int framesToRecord = 50 * 10;  // 50fps * 10秒間
    private int currentFrame = 0;
    private bool isRecording = false;

    void Start()
    {
        mocopiAvatar = GetComponent<MocopiAvatar>();
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Recording is already in progress.");
            return;
        }
        
        bvhWriter = new StreamWriter(filePath);
        WriteBVHHeader();
        currentFrame = 0;
        isRecording = true;

        StartCoroutine(RecordMotion());
    }

    private IEnumerator RecordMotion()
    {
        while (currentFrame < framesToRecord && isRecording)
        {
            WriteBVHFrame();
            currentFrame++;

            // Wait for the next frame
            yield return new WaitForSecondsRealtime(frameTime);
        }

        isRecording = false;
        FinishRecording();
    }

    private void FinishRecording()
    {
        if (bvhWriter != null)
        {
            bvhWriter.Close();
            Debug.Log("Recording finished.");
        }
    }

    private void WriteBVHHeader()
    {
        // Write the header including hierarchy definition
        bvhWriter.WriteLine("HIERARCHY");
        bvhWriter.WriteLine("ROOT Hips");
        bvhWriter.WriteLine("{");
        bvhWriter.WriteLine("\tOFFSET 0.00 0.00 0.00");
        bvhWriter.WriteLine("\tCHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");
        // Continue for other bones...
        bvhWriter.WriteLine("}");

        bvhWriter.WriteLine("MOTION");
        bvhWriter.WriteLine($"Frames: {framesToRecord}");
        bvhWriter.WriteLine($"Frame Time: {frameTime.ToString("F6")}");
    }

    private void WriteBVHFrame()
    {
        if (mocopiAvatar == null) return;

        // For each bone, get its current transform position and rotation and write to file
        foreach (var bone in mocopiAvatar.bones)
        {
            Vector3 position = bone.Transform.localPosition;
            Quaternion rotation = bone.Transform.localRotation;
            Vector3 eulerRotation = rotation.eulerAngles;

            // Write the position and rotation for the current frame
            bvhWriter.Write($"{position.x:F3} {position.y:F3} {position.z:F3} {eulerRotation.z:F3} {eulerRotation.x:F3} {eulerRotation.y:F3} ");
        }
        bvhWriter.WriteLine();
    }
}
