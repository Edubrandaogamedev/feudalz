
using System.Collections.Generic;
using System.Threading.Tasks;
public class TaskManager
{

    private static List<Task> registeredTasks = new List<Task>();
    public static void RegisterTask(Task _task)
    {
        registeredTasks.Add(_task);   
    }
    public static void DisposeTask(Task _task)
    {
        registeredTasks.Remove(_task);
    }
    public static List<Task> GetRegisteredTasks()
    {
        return registeredTasks;
    }
}
