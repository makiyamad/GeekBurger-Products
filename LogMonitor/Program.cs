using System;

namespace LogMonitor
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Inform ## and a new Topic and hit enter");
            var topic = Console.ReadLine();
            while (true) {
                var monitor = new Monitor();
                if (topic != null) monitor.Init(topic.Replace("##", ""));

                var otherTopic = "";
                Console.WriteLine("To inform a new topic add ##newtopic");
                while (otherTopic != null && !otherTopic.Contains("##"))
                {
                    otherTopic = Console.ReadLine();
                }
                monitor.Dispose();
                monitor = null;
            }
        }
    }
}