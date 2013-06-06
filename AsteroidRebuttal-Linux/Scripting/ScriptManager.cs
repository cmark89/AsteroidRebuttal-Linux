using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AsteroidRebuttal.GameObjects;

namespace AsteroidRebuttal.Scripting
{
    // The script delegate handles generic scripts
    public delegate IEnumerator<float> Script();

    // The GameObjectScript delegate handles scripts that are attached to a specific object
    public delegate IEnumerator<float> GameObjectScript(GameObject go);

    

    public class ScriptManager
    {
        public List<ScriptWrapper> scripts { get; set; }

        public ScriptManager()
        {
            scripts = new List<ScriptWrapper>();
        }

        public void Update(GameTime gameTime)
        {
            // First, remove all completed scripts from the list of active scripts
            foreach(ScriptWrapper s in scripts.FindAll(x => x.State == ScriptState.Completed))
            {
                scripts.Remove(s);
            }

            // Then, initialized scripts added during the previous cycle
            foreach (ScriptWrapper s in scripts.FindAll(x => !x.Initialized))
            {
                s.Initialized = true;
            }

            // Then, update the remaining scripts
            foreach (ScriptWrapper s in scripts.FindAll(x => x.Initialized))
            {
                s.Update(gameTime);
            }
        }

        public void Execute(Script newScript, bool initialized = false)
        {
            scripts.Add(new ScriptWrapper(newScript, initialized));
        }

        public void Execute(GameObjectScript newScript, GameObject go, bool initialized = false)
        {
            scripts.Add(new ScriptWrapper(newScript, go, initialized));
        }

        public void AbortObjectScripts(GameObject go)
        {
            foreach (ScriptWrapper sw in scripts.FindAll(x => x.ScriptObject == go))
            {
                sw.SetCompleted();
            }
        }
    }
}
