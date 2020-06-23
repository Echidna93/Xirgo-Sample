using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // ridelog.dat placed in bin/Debug/netcore
            System.IO.StreamReader file = new System.IO.StreamReader("ridelog.dat");
            // list to hold usage logs for bikes
            List<Dictionary<String, String>> logList = new List<Dictionary<String, String>> { };
            // read entire file and trim leading and trailing white spaces
            string content = file.ReadToEnd().Trim();
            // split file by "%%" pattern, denoting the end of a log
            string[] lines = System.Text.RegularExpressions.Regex.Split(content, "%%", System.Text.RegularExpressions.RegexOptions.None);
            // totalSecs temp variable to hold total number of seconds
            int totalSecs = 0;
            // holds nunmber of riders over the age of 40
            int numOver40 = 0;
            // holds number of members
            int numMembers = 0;
            // holds number of times most-used bike is used
            int mostUsedBikeCount = 0;
            // holds id of most-used bike
            string mostUsedBikeID = "";
            // holds type of most used bike
            string mostUsedBikeType = "";
            // holds bike id and number of usages 
            // key is concatenated bikeId + ":" + bikeType
            Dictionary<String, int> bikeIDList = new Dictionary<String, int> { };
            // variables to hold the start time data
            int leastPopularStartTimeCount = 20000;
            int mostPopularStartTimeCount = 0;
            int leastPopularStartTime = 0;
            int mostPopularStartTime = 0;
            int[] startTimeList = new int[24];

            // grab all logs in the file
            for (int i = 0; i < lines.Length - 1; i++)
            {
                // put all logs in list of dictionarys with keys and values
                logList.Add(DataToDictionary(lines[i]));

            }
            // step thru every log in the .dat file
            foreach (Dictionary<String, String> log in logList)
            {
                // grab number of hours for given log, increment total number of seconds by that amount
                totalSecs += Convert.ToInt32(log["duration"]);
                // check if user in current log is over 40
                if ((Convert.ToInt32(DateTime.Now.Year.ToString()) - Convert.ToInt32(log["userBirthYear"])) > 40)
                {
                    // if so increment numOver40
                    numOver40 += 1;
                }
                // increment startTimeList at index hour of the day 
                startTimeList[Convert.ToInt32(DateTime.FromOADate(Convert.ToDouble(log["startTime"])).ToString("HH"))] += 1;

                //Console.WriteLine(log["endTime"]);
                // check if userType is a subscription
                if (String.Equals(log["userType"], "01"))
                {
                    numMembers++;
                }
                // check if key exists in list already
                if (bikeIDList.ContainsKey(log["bikeId"] + ":" + log["bikeType"]))
                {
                    // if it does increment the int at that key
                    bikeIDList[log["bikeId"] + ":" + log["bikeType"]] += 1;
                }
                // if it doesn't add the key to the list
                else if (!bikeIDList.ContainsKey(log["bikeId"] + ":" + log["bikeType"]))
                {

                    bikeIDList.Add(log["bikeId"] + ":" + log["bikeType"], 1);
                }
            }
            // step thru each bike id in list of bike ids
            foreach (KeyValuePair<String, int> bikeID in bikeIDList)
            {
                // check if bike with highest number of uses is less than number uses of current bike
                if (mostUsedBikeCount < bikeID.Value)
                    // split the key, this part is the bike id
                    mostUsedBikeID = bikeID.Key.Split(':', StringSplitOptions.None)[0];
                // split the key, second part is the bike type
                mostUsedBikeType = bikeID.Key.Split(':', StringSplitOptions.None)[1];
                // set most used bike count to new value
                mostUsedBikeCount = bikeID.Value;
            }
            // step thru list of start times
            for (int i = 0; i < startTimeList.Length; i++)
            {
                // check if current start time at index is greater than current most popular start time
                if (startTimeList[i] > mostPopularStartTimeCount)
                {
                    // if so set that to be the most popular
                    mostPopularStartTime = i;
                    mostPopularStartTimeCount = startTimeList[i];
                }
                // check if current start time at index is less than current least popular start time
                if (startTimeList[i] < leastPopularStartTimeCount)
                {
                    // if so set that to be the least popular
                    leastPopularStartTime = i;
                    leastPopularStartTimeCount = startTimeList[i];
                }
            }

            /*
             * print statements
             */
            // total number of hours
            Console.WriteLine("total number of hours used: " + secondsToHours(totalSecs));
            // Calculate percent of users over 40
            Console.WriteLine("percent of users over 40: " + Math.Round((Convert.ToDouble(numOver40) / Convert.ToDouble(logList.Count) * 100), 2));
            // Number of members
            Console.WriteLine("number of members who completed ride: " + numMembers);
            // most popular start time
            Console.WriteLine("most popular startTime (HH): " + mostPopularStartTime);
            // least popular start time
            Console.WriteLine("least popular startTime (HH): " + leastPopularStartTime);
            // Most used bike id
            Console.WriteLine("most used bike ID: " + mostUsedBikeID);
            // most used bike type
            Console.WriteLine("most used bike type: " + mostUsedBikeType);

            // close the .dat file we opened
            file.Close();
        }
        /*
         * small method that converts seconds to hours as a double of precision 2
         */
        static double secondsToHours(int seconds)
        {
            return Math.Round((seconds / 60.0 / 60.0), 2);
        }

        /*
         * parses a chunk of data and returns a dictionary of strings, representing each element of dat file data
         * representing key value pairs
         */
        static Dictionary<String, String> DataToDictionary(String currentLine)
        {
            var dict = new Dictionary<string, string> { };
            string[] parsedSegments = currentLine.Split(new Char[] { '|', ',' }, StringSplitOptions.None);
            // index will keep track of current index
            int index = 0;
            foreach (string s in parsedSegments)
            {

                switch (index)
                {
                    case 0:
                        dict["duration"] = System.Text.RegularExpressions.Regex.Split(s, "!!", System.Text.RegularExpressions.RegexOptions.None)[1];
                        break;
                    case 1:
                        dict["startTime"] = s;
                        break;
                    case 2:
                        dict["endTime"] = s;
                        break;
                    case 3:
                        dict["startStationId"] = s;
                        break;
                    case 4:
                        dict["startStation"] = s;
                        break;
                    case 5:
                        dict["startLat"] = s;
                        break;
                    case 6:
                        dict["startLong"] = s;
                        break;
                    case 7:
                        dict["endStationId"] = s;
                        break;
                    case 8:
                        dict["endStation"] = s;
                        break;
                    case 9:
                        dict["endLat"] = s;
                        break;
                    case 10:
                        dict["endLong"] = s;
                        break;
                    case 11:
                        dict["bikeId"] = s;
                        break;
                    case 12:
                        dict["bikeType"] = s;
                        break;
                    case 13:
                        dict["userType"] = s;
                        break;
                    case 14:
                        dict["userBirthYear"] = s;
                        break;
                    case 15:
                        dict["userGender"] = s;
                        break;
                }
                index++;
            }
            return dict;
        }
    }
}


