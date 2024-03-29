﻿namespace LiftDataManager.Core.Models;

public class BufferCalculationData
{
    public required string ProfilDescription { get; set; }
    public int NumberOfBuffer { get; set; }
    public required int BufferPillarLength { get; set; }
    public required int BucklingLength { get; set; }
    public required string ProfilMaterial { get; set; }
    public double MomentOfInertiaX { get; set; }
    public double MomentOfInertiaY { get; set; }
    public bool ReducedSafetyRoomBufferUnderCounterweight { get; set; }
}
