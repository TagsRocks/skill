using UnityEngine;
using System.Collections;
namespace Skill.Framework
{
    public static class AndroidLogger
    {
        static int _Counter = 0;
        public static void Log(string str)
        {

            string fileName = Application.persistentDataPath + "/Log.txt";


            if (_Counter == 0)
            {
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }
            _Counter++;
            var stream = System.IO.File.AppendText(fileName);
            stream.WriteLine(str);
            stream.Close();
        }
    }
}