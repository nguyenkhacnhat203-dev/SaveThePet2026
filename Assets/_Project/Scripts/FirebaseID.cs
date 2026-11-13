using UnityEngine;

public class FirebaseID
{
    public string DeviceID { get; private set; }

    public FirebaseID()
    {
        DeviceID = SystemInfo.deviceUniqueIdentifier;
    }
}