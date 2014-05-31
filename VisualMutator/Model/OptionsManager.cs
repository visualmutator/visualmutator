namespace VisualMutator.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using UsefulTools.Paths;

    public interface IOptionsManager
    {
        OptionsModel ReadOptions();
        void WriteOptions(OptionsModel options);
        IObservable<OptionsManager.EventType> Events { get; }
    }

    public class OptionsManager : IOptionsManager
    {
        public enum EventType
        {
            Updated,
            Loaded
        }

        public OptionsManager()
        {
            _events = new Subject<EventType>();
        }

        private readonly Subject<EventType> _events; 
        private const string OptionsFileName = "VisualMutator-options.xml";

        public OptionsModel ReadOptions()
        {
            string path = GetOptionsFilePath();
            if(File.Exists(path))
            {
                using (StreamReader stream = new StreamReader(GetOptionsFilePath()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(OptionsModel));
                    _events.OnNext(EventType.Loaded);
                    return (OptionsModel)serializer.Deserialize(stream);
                }
            }
            else
            {
                _events.OnNext(EventType.Loaded);
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
            _events.OnNext(EventType.Updated);
        }

        public IObservable<EventType> Events { get { return _events; } }


        private string GetOptionsFilePath()
        {
            return new FilePathAbsolute(Assembly.GetExecutingAssembly().Location)
                .GetBrotherFileWithName(OptionsFileName).Path;
        }

    }
}