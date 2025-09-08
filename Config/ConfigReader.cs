// using System.IO;
// using System.Linq;
// using YamlDotNet.Serialization;
// using YamlDotNet.Serialization.NamingConventions;

// namespace Dont_Be_Humble.Config {
//     public class ConfigReader {
//         public static Config readCurrentConfig() {
//             var deserializer = new DeserializerBuilder()
//                 .WithNamingConvention(UnderscoredNamingConvention.Instance) // see height_in_inches in sample yml 
//                 .Build();
//             return Directory
//                 .EnumerateFiles(Directory.GetCurrentDirectory() + "\\Assets")
//                 .Where(file => !file.EndsWith(".meta"))
//                 .Where(file => file.Contains("DBHConfig.yaml"))
//                 .Select(File.ReadAllText)
//                 .Select(s => deserializer.Deserialize<Config>(s))
//                 .First();
//         }
//     }
// }