namespace VisualMutator.Model
{
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using UsefulTools.Paths;

    public interface IOptionsManager
    {
        OptionsModel ReadOptions();
        void WriteOptions(OptionsModel options);
    }

    public class OptionsManager : IOptionsManager
    {
        private const string OptionsFileName = "VisualMutator-options.xml";

        public OptionsModel ReadOptions()
        {
            string path = GetOptionsFilePath();
            if(File.Exists(path))
            {
                using (StreamReader stream = new StreamReader(GetOptionsFilePath()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(OptionsModel));
                    return (OptionsModel)serializer.Deserialize(stream);
                }
            }
            else
            {
                return new OptionsModel();
            }
        }

        public void WriteOptions(OptionsModel options)
        {
            using (StreamWriter stream = new StreamWriter(GetOptionsFilePath()))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OptionsModel));
                serializer.Serialize(stream, options);
            }
        }

        private string GetOptionsFilePath()
        {
            return new FilePathAbsolute(Assembly.GetExecutingAssembly().Location)
                .GetBrotherFileWithName(OptionsFileName).Path;
        }

    }
}