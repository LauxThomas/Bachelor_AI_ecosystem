using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

public class Testing : MonoBehaviour
{
    [SerializeField] private bool useJobs;

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = reallyToughTaskJob();
                jobHandleList.Add(jobHandle);
            }

            JobHandle.CompleteAll(jobHandleList);
            jobHandleList.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }

        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f + " ms");
    }

    private void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }

    private JobHandle reallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }
}
[BurstCompile]
public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}