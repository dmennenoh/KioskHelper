using UnityEngine;
using KioskHelper;
using System.Diagnostics;

/*
 * Just drag onto a gameObject and play the scene to test the filter. 
 */

public class TestProfanityFilter : MonoBehaviour
{   
    private readonly string[] shouldFlagTrue =
    {
        "fuck",
        "Fuck!",
        "FUCK",
        "shit",
        "bitch",
        "asshole",
        "dick",
        "pussy",
        "cunt",
        "motherfucker",
        "D1ck!",
        "fu ck m e",
        "fu ck",
        "fu_ck",
        "sh!t",
        "f.u.c.k",
        "f-u-c-k",
        "f u c k",
        "f u c k m e",
        "pu55y",
        "pu**y",
        "d*ck",
        "sh!ts",
        "pu55ycunt",
        "fuckdaddyhard",
        "fuckface",
        "bullshit",
        "dumbass",
        "you are an asshole",
        "what the fuck",
        "this is bullshit",
        "dumb ni**er"
    };

    private readonly string[] shouldFlagFalse =
    {
        "bitche",//town in france
        "hello",
        "Hello world",
        "Have a nice day",
        "Good morning!",
        "Unity development",
        "Blender modeling",
        "This is fantastic",
        "Classic car",
        "Assignment complete",
        "Scunthorpe",//town in england
        "Cockatoo",
        "Peacock",
        "Assessment",
        "Passionate developer",
        "Night shift",
        "Dickens novels",
        "The assistant arrived",
        "I love cooking",
        "The grass is green",
        "A class assignment"
    };

    void Start()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        DoTest("SHOULD FLAG TRUE", shouldFlagTrue, true);
        DoTest("SHOULD FLAG FALSE", shouldFlagFalse, false);
        stopWatch.Stop();
        print($"Elapsed test time: {stopWatch.ElapsedMilliseconds} ms");
    }

    void DoTest(string title, string[] wordList, bool expected)
    {
        print($"=============== {title} ===============");

        foreach (string words in wordList)
        {
            bool result = ProfanityFilter.ContainsProfanity(words);
            string status = result == expected ? "PASS" : "FAIL";
            print($"{status} | Expected: {expected} | Actual: {result} | \"{words}\"");
        }
    }
}