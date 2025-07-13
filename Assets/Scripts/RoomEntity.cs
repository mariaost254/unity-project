using System.Collections.Generic;
using UnityEngine;

public class RoomEntity
{
    public string id;
    public string nameRoom;
    public List<RoomOptions> Options = new();
}

public class RoomOptions
{
    public string Color;
    public string Designer;
    public string Price;
    public string Function;
    public string Floor;
    public string Furnishing;
    public int Viewpoint;
    public string ImagePath;
}

