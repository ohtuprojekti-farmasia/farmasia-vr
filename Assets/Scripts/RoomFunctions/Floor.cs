using UnityEngine;

public class Floor : MonoBehaviour {

    private void OnCollisionEnter(Collision col) {
        GeneralItem item = GeneralItem.Find(col.gameObject.transform);
        if (item != null) {
            foreach (Package p in G.Instance.Progress.packages) {
                if (p.name == "Clean Up" && p.activeTasks.Count == 1) {
                    p.AddNewTaskBeforeTask(G.Instance.Progress.FindTaskWithType(TaskType.ScenarioOneCleanUp), p.activeTasks[0]);
                    break;
                }
            }
            Events.FireEvent(EventType.ItemDroppedOnFloor, CallbackData.Object(item));
            item.IsClean = false;
        }
    }

    private void OnTriggerExit(Collider col) {
        GeneralItem item = GeneralItem.Find(col.gameObject.transform);
        if (item != null) {
            Events.FireEvent(EventType.ItemLiftedOffFloor, CallbackData.Object(item));
        }
    }
}