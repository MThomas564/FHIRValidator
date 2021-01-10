using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.IO;
using System.Linq;

namespace FHIRValidate.Dev {
    public class Validate {
        private readonly string _directory;
        public Validate(string Directory) {
            Ensure.That(Directory).IsNotNull();
            _directory = Directory;
        }

        public void Execute() {
            //Load files
            var files = LoadFiles(_directory);
            foreach (var f in files) {
                var type = Path.GetExtension(f);
                var content = File.ReadAllText(f);

                if (type == ".json") {
                    var @return = LoadJson(content);
                    // _resourceParser.Parse()
                }
                if (type == ".xml") {
                    var @return = LoadXml(content);
                }


            }
        }

        protected string[ ] LoadFiles(string strDir) {
            return Directory.EnumerateFiles(strDir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".xml") || s.EndsWith(".json")).ToArray();
        }

        //loadJson
        protected Hl7.Fhir.Model.Base LoadJson(string s) {
            var parser = new FhirJsonParser();

            try {
                var parsed = parser.Parse(s);
                return parsed;
            } catch (FormatException fe) {
                throw fe;
            }
        }

        //loadXml
        protected Hl7.Fhir.Model.Base LoadXml(string s) {
            var parser = new FhirXmlParser();

            try {
                var parsed = parser.Parse(s);
                return parsed;
            } catch (FormatException fe) {
                // the boring stuff
                throw fe;
            }

        }
    }
}
