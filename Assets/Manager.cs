using UnityEngine;
using System;

public abstract class Manager <DerivedManager>
        where DerivedManager : Manager <DerivedManager>
{
    private static DerivedManager s_Instance;

    public static DerivedManager Instance
    {
        get {return s_Instance;}
    }

    protected abstract void Terminate();

    public static DerivedManager Create()
    {
        try
        {
            if (null == s_Instance)
            {
                s_Instance = Activator.CreateInstance(typeof(DerivedManager), true) as DerivedManager;
            }
            else
            {
                string exceptionMessage = System.String.Format("Instance of {0} already exists", typeof(DerivedManager).ToString());
                throw new Exception(exceptionMessage);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return s_Instance;
    }

    public static void Destroy()
    {
        s_Instance.Terminate();
    }

    public override String ToString()
    {
        return "Base Manager Class";
    }
}
