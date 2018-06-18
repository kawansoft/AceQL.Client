using AceQL.Client.Api.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    class JsonTest
    {
        public static void TheMain(string[] args)
        {
           

            try
            {
                //JsonParseResultAsString();
                //JsonParseResultAsFile();
                /*
                using (Stream stream = File.OpenRead(@"C:\test\json_out.txt"))
                {
                    OutParamBuilder outParamBuilder = new OutParamBuilder(stream);

                    Console.WriteLine();
                    Console.WriteLine("------- Dicts Display ------");
                    Console.WriteLine();

                    Console.WriteLine("------- GetvaluesPerParamIndex() ------");
                    foreach (KeyValuePair<int, string> outParameter in outParamBuilder.GetvaluesPerParamIndex())
                    {
                        Console.WriteLine(outParameter.Key + " / " + outParameter.Value);
                    }

                    Console.WriteLine();

                    Console.WriteLine("------- GetvaluesPerParamName() ------");
                    foreach (KeyValuePair<string, string> outParameter in outParamBuilder.GetvaluesPerParamName())
                    {
                        Console.WriteLine(outParameter.Key + " / " + outParameter.Value);
                    }

                }
                */
                Console.WriteLine();
                Console.WriteLine("Press enter to close....");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }

        }

        public static void JsonParseResultAsFile()
        {
            Dictionary<int, string> valuesPerParamIndex = new Dictionary<int, string>();
            Dictionary<string, string> valuesPerParamName = new Dictionary<string, string>();

            Stream stream =  File.OpenRead(@"C:\test\json_out.txt");

            TextReader textReader = new StreamReader(stream);
            var reader = new JsonTextReader(textReader);

            // Necessary because a SQL columns could have the name "row_count".
            // We know that we are reading the good end of file "row_count" if
            // We are not any more in a array
            bool isInsideArray = false;

            while (reader.Read())
            {
                /*
                if (reader.Value == null)
                {
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        isInsideArray = true;
                    }
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        isInsideArray = false;
                    }

                    continue;
                }
                */

                if (reader.TokenType != JsonToken.PropertyName || !reader.Value.Equals("parameters_out_per_index") || isInsideArray)
                {
                    continue;
                }

                String typeParameter = "per_index";

                string paramName = null;
                string paramValue = null;

                while (reader.Read())
                {
                    if (reader.Value != null && reader.TokenType == JsonToken.PropertyName && reader.Value.Equals("parameters_out_per_name"))
                    {
                        typeParameter = "per_name";
                    }

                    if ( reader.TokenType == JsonToken.PropertyName)
                    {
                        paramName = reader.Value.ToString();
                        paramName = paramName.Trim();
                        Console.WriteLine("property: " + paramName + ":");
                    }
                    else if(reader.TokenType == JsonToken.String)
                    {
   
                        paramValue = reader.Value.ToString();
                        Console.WriteLine("paramIndex: " + paramName + ":");
                        Console.WriteLine("value     : " + paramValue + ":");
                        if (paramValue.Equals("NULL"))
                        {
                            paramValue = null;
                        }

                        if (typeParameter.Equals("per_index")) {
                            int paramIndexInt = Int32.Parse(paramName);
                            valuesPerParamIndex.Add(paramIndexInt, paramValue);
                        }
                        else
                        {
                            valuesPerParamName.Add(paramName, paramValue);
                        }

                    }
                       
                    /*
                    // We are done at end of row
                    if (reader.TokenType.Equals(JsonToken.EndObject))
                    {
                        Console.WriteLine("Break!");
                        break;
                    }
                    */
                }
 
            }

            Console.WriteLine();
            Console.WriteLine("------- dict display ------");
            Console.WriteLine();

            foreach (KeyValuePair<int, string> outParameter in valuesPerParamIndex)
            {
                Console.WriteLine(outParameter.Key + " / " + outParameter.Value);
            }

            Console.WriteLine();

            foreach (KeyValuePair<string, string> outParameter in valuesPerParamName)
            {
                Console.WriteLine(outParameter.Key + " / " + outParameter.Value);
            }

            stream.Close();

        }

        public static void JsonParseResultAsString()
        {
            string jsonResult = File.ReadAllText(@"C:\test\json_out.txt");
            Console.WriteLine(jsonResult);
            Console.WriteLine();

            dynamic xj = JsonConvert.DeserializeObject(jsonResult);
            dynamic xjParametersOutPername = xj.parameters_out_per_name;

            if (xjParametersOutPername != null)
            {
                String dictStr = xjParametersOutPername.ToString();
                Console.WriteLine("dictStr:" + dictStr);
                Console.WriteLine();

                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dictStr);

                // For each parameter 1) get the index 2) get the dbType
                foreach (KeyValuePair<String, string> theParameter in dict)
                {
                    Console.WriteLine(theParameter.Key + " / " + theParameter.Value);
                }
            }
            else
            {
                Console.WriteLine("No Out parameters per name");
            }
        }
    }
}
