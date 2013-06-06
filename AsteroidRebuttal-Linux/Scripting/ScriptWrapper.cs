using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal.GameObjects;

namespace AsteroidRebuttal.Scripting
{
    public class ScriptWrapper
    {
        // Stores the script itself
        IEnumerator<float> thisScript;
        public bool IsObjectScript { get; private set; }
        public GameObject ScriptObject { get; private set; }

        // The amount of time this script should sleep for
        double SleepTime;

        // Stores the current state of the script
        public ScriptState State { get; private set; }

        // This is true when the script is ready to be run.  If set to false, it will take 1 frame to activate in order to 
        // prevent changing the manager's enumeration during update
        public bool Initialized { get; set; }


        // Returns the current float value of the enumerator
        public float Current
        {
            get { return thisScript.Current; }
        }

        // Moves the script to the next yield and returns true if it exists
        public bool MoveNext()
        {
            if (thisScript.MoveNext())
                return true;
            else
                return false;
        }

        public ScriptWrapper(Script newScript, bool initialized = false)
        {
            thisScript = newScript();
            State = ScriptState.Running;
            SleepTime = 0f;

            IsObjectScript = false;
            Initialized = initialized;
        }

        public ScriptWrapper(GameObjectScript newScript, GameObject go, bool initialized = false)
        {
            thisScript = newScript(go);
            State = ScriptState.Running;
            SleepTime = 0f;

            IsObjectScript = true;
            ScriptObject = go;

            Initialized = initialized;
        }

        public void Update(GameTime gameTime)
        {
            if (IsObjectScript && ScriptObject == null)
                State = ScriptState.Completed;

            if (State == ScriptState.Completed)
                return;

            switch (State)
            {
                // The script is running
                case ScriptState.Running:
                    if (MoveNext())
                    {
                        // The script is still executing; sleep if the current value is greater than 0
                        if (Current > 0)
                        {
                            State = ScriptState.Sleeping;
                            SleepTime = Current;
                        }
                        // If the value is 0, then simply break
                        break;
                    }
                    else
                    {
                        // The script has reached its end
                        State = ScriptState.Completed;
                    }
                    break;

                // The script is sleeping
                case ScriptState.Sleeping:
                    // Decrement the sleep timer
                    SleepTime -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (SleepTime <= 0)
                    {
                        // If the timer is less than 0, then set it back to running
                        SleepTime = 0;
                        State = ScriptState.Running;
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetCompleted()
        {
            State = ScriptState.Completed;
        }
    }

    public enum ScriptState
    {
        Running,
        Sleeping,
        Completed
    }
}