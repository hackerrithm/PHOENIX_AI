using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace PHOENIX_AI
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        static void Main(string[] args)
        {
            // LIst of messages that will be selected at random when COU is under intense pressure
            List<string> cpuMaxedOutMessages = new List<string>();
            cpuMaxedOutMessages.Add("WARNING: CPU is about to catch fire!!!");
            cpuMaxedOutMessages.Add("WARNING: Do not run CPU that hard");
            cpuMaxedOutMessages.Add("WARNING: Seize action immediately!!!");
            cpuMaxedOutMessages.Add("WARNING: CPU is about to catch fire!!!");
            cpuMaxedOutMessages.Add("WARNING: Code red Code red Code red Code red CPU is being damaged");

            //Random counter
            Random rand = new Random();           
            
            //This will greet the user

            synth.Speak("Welcome to Phoenix Memory analyzer");
            synth.Speak("Please relax while we analyze your system's CPU and memory status");

            #region Performance Counters
            // This will pull the current CPU load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            // This will pull the current available memonry in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // This will pull the system uptime (in seconds)
            PerformanceCounter perUptimeCount = new PerformanceCounter("Memory", "Available MBytes");
            perUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perUptimeCount.NextValue());

            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays,
                (int)uptimeSpan.Hours,
                (int)uptimeSpan.Minutes,
                (int)uptimeSpan.Seconds
                );

            //Tells the user what the current system up time is
            synth.Speak(systemUptimeMessage);

            bool wasBrowserOpened = false;

            while (true)
            {   
                //Get the current performance counter values and type cast them to integer values
                int currentCPUPercentage    = (int)perfCpuCount.NextValue();
                int currentAvailableMemory  = (int)perfMemCount.NextValue();
                // Every 1 second / 1000 ms the CPU load in percentage is screened                
                Console.WriteLine("CPU Load         : {0}%", currentCPUPercentage);
                Console.WriteLine("Available Memory : {0}MB", currentAvailableMemory);
                Thread.Sleep(1000);

                
                // Speak to the user
                #region Logic
                if (currentAvailableMemory < 1024)
                {
                    string memLoadVocalMessage = string.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
                    Speak(memLoadVocalMessage, VoiceGender.Male);

                }

                if (currentCPUPercentage > 80)
                {
                    if (currentCPUPercentage == 100)
                    {                        
                        string cpuLoadVocalMessage = cpuMaxedOutMessages[rand.Next(5)];//string.Format("Your CPU is burning up!!!", currentCPUPercentage);
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, 3);

                        if (wasBrowserOpened == false)
                        {
                            OpenWebSite("https://www.youtube.com/");
                            wasBrowserOpened = true;
                        }
                        
                    }
                    else
                    {
                        string cpuLoadVocalMessage = string.Format("The current CPU load is {0} percent", currentCPUPercentage);
                        Speak(cpuLoadVocalMessage, VoiceGender.Male, 4);
                    }

                 }
                #endregion
            }
        }
        /// <summary>
        /// Speaks with a selected voice
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>

        public static void Speak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            //string memLoadVocalMessage = string.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
            synth.Speak(message);

        }

        /// <summary>
        /// Speaks with a selected voice at a selected speed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="voiceGender"></param>
        /// <param name="rate"></param>
        public static void Speak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(message, voiceGender);
        }

        /// <summary>
        /// Opens up a website
        /// </summary>
        /// <param name="URL"></param>
        public static void OpenWebSite(string URL)
        {
            Process p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = URL;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }
    }
}
