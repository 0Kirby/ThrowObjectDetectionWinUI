﻿namespace TinyThrow.Helpers;
public class Parameters
{
    private readonly string path;
    private readonly string folder;
    private readonly string img;
    private readonly string batch;
    private readonly string epoch;
    private readonly string model;
    private readonly string scale;
    private readonly string device;
    public string Path
    {
        get; set;
    }
    public string Folder
    {
        get; set;
    }
    public string Img
    {
        get; set;
    }
    public string Batch
    {
        get; set;
    }
    public string Epoch
    {
        get; set;
    }
    public string Model
    {
        get; set;
    }
    public string Scale
    {
        get; set;
    }
    public string Device
    {
        get; set;
    }
    public Parameters(string path, string folder, string img, string batch, string epoch, string model, string scale, string device)
    {
        Path = path;
        Folder = folder;
        Img = img;
        Batch = batch;
        Epoch = epoch;
        Model = model;
        Scale = scale;
        Device = device;
    }
}
