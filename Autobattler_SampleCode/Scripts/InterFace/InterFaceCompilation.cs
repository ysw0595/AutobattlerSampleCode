using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMelee
{
    public void UpMelee();
    public void DownMelee();
}

interface IRange
{
    public void UpRange();
    public void DownRange();
}

interface ISpeed
{
    public void UpSpeed();
    public void DownSpeed();
}

interface IAncient
{
    public void UpAncient();
    public void DownAncient();
}

interface IClassic
{
    public void UpClassic();
    public void DownClassic();
}