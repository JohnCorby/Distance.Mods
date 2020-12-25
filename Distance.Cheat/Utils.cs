using System.Text;
using UnityEngine;

namespace Distance.Cheat {
    public static class Utils {
        /// log all the objects in the current scene and their components
        public static void PrintObjects() {
            var s = new StringBuilder();
            s.AppendLine("############# BEGIN PRINTING OBJECTS #############");

            foreach (var gameObject in Object.FindObjectsOfType<GameObject>()) {
                s.AppendLine(gameObject.name);
                foreach (var component in gameObject.GetComponents<Component>())
                    s.AppendLine("\t" + component.GetType());
            }

            s.AppendLine("############# END PRINTING OBJECTS #############");
            Entry.LOG.Info(s);
        }

        public static void PrintObject(GameObject gameObject) {
            var s = new StringBuilder();

            s.AppendLine(gameObject.name);
            foreach (var component in gameObject.GetComponents<Component>())
                s.AppendLine("\t" + component.GetType());

            Entry.LOG.Info(s);
        }
    }
}
