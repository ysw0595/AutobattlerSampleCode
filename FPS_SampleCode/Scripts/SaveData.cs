using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

[System.Serializable, StructLayout(LayoutKind.Explicit)]
public struct IntToByteArray
{
    [FieldOffset(0)]
    public byte asByte0;
    [FieldOffset(1)]
    public byte asByte1;
    [FieldOffset(2)]
    public byte asByte2;
    [FieldOffset(3)]
    public byte asByte3;

    [FieldOffset(0)]
    public int asInteger;
}

public class SaveData : MonoBehaviour
{
    public static string scorePath => Application.persistentDataPath + "/Score.sav";

    public static void Save()
    {
        if(!File.Exists(scorePath))
        {
            FileStream fs  = File.Create(scorePath);
            fs.Close();
        }

        IntToByteArray intToByteArray = new IntToByteArray();
        intToByteArray.asInteger = GameManager.instance.Score;
        byte[] asByteArray = { intToByteArray.asByte0, intToByteArray.asByte1, intToByteArray.asByte2, intToByteArray.asByte3 };
        File.WriteAllBytes(scorePath, asByteArray);
    }

    public static void Load()
    {
        if (!File.Exists(scorePath))
        {
            return;
        }
        else
        {
            byte[] asByteArray = File.ReadAllBytes(scorePath); 

            IntToByteArray converter = new IntToByteArray();
            converter.asByte0 = asByteArray[0];
            converter.asByte1 = asByteArray[1];
            converter.asByte2 = asByteArray[2];
            converter.asByte3 = asByteArray[3];

            GameManager.instance.Score = converter.asInteger;
        }
    }
}
