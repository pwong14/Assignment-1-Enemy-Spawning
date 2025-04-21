using System;
using System.Collections.Generic;

[Serializable]
public class Spawn
{
    public string enemy;                     // "zombie", "skeleton" …

    public string count   = "1";             // RPN or constant
    public List<int> sequence = new() { 1 }; // [1,2,3] → JSON array
    public string delay   = "2";             // seconds between bursts
    public string location = "random";       // "random", "random red", …

    public string hp     = "base";           // RPN or "base"
    public string speed  = "base";
    public string damage = "base";
}
