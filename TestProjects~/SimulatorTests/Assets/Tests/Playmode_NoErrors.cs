using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Playmode_NoErrors
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest, Category("ErrorCheck")]
        public IEnumerator Playmode_NoErrorsNoWarnings()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            LogAssert.NoUnexpectedReceived();
        }
    }
}
