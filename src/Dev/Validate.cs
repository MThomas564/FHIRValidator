using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Support;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Validation;
using System.ComponentModel.DataAnnotations;

namespace FHIRValidate.Dev {
    public class Validate {
        private readonly string _directory;
        private List<(string, Exception)> _failures;
        private List<(string, Hl7.Fhir.Model.Base)> _success;

        public Validate(string Directory) {
            Ensure.That(Directory).IsNotNullOrEmpty();
            _directory = Directory;
        }

        public void Execute() {
            //Load files
            var files = LoadFiles(_directory);
            var totalCount = files.Length;
            _success = new List<(string, Hl7.Fhir.Model.Base)>();
            _failures = new List<(string, Exception)>();
            foreach (var f in files) {
                var type = Path.GetExtension(f);
                var content = File.ReadAllText(f);

                switch(type){
                    case ".json":
                        LoadJson(content, f);
                        break;
                    case ".xml":
                        LoadXml(content, f);
                        break;
                }
            }

            Console.WriteLine($"{_success.Count} of {totalCount} passed basic syntax validation");
            Console.WriteLine("The following files failed validation");
            foreach(var f in _failures){
                Console.WriteLine($"Filename: {f.Item1}");
                Console.WriteLine($"Exception message: {f.Item2.Message}");
                Console.WriteLine("========================");
            }

            if(_failures.Count > 0){
                throw new Exception("Several resources failed basic validation");
            }

            // Console.WriteLine("Starting in depth validation");
            // foreach(var r in _success){
            //     try{
            //         var ctx = new ValidationContext(new Hl7.Fhir.Model.Questionnaire());
            //         var validator = new Validator();
            //         r.Item2.Validate(ctx);
            //     } catch(Exception e){
            //         throw e;
            //     }
            // }
        }

        protected string[ ] LoadFiles(string strDir) {
            return Directory.EnumerateFiles(strDir, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".xml") || s.EndsWith(".json")).ToArray();
        }

        //loadJson
        protected void LoadJson(string s, string fileName) {

            //try parse json for syntax
            var fr = new FhirResource();
            try {
                fr = JsonConvert.DeserializeObject<FhirResource>(s);
            } catch (Exception e) {
                _failures.Add((fileName, e));
                return;
            }

            var parser = new FhirJsonParser();

            try {
                switch (fr.ResourceType) {
                    case "Questionnaire":
                        Console.WriteLine("Questionnaire");
                        var q = parser.Parse<Hl7.Fhir.Model.Questionnaire>(s);
                        _success.Add((fileName, q));
                        break;
                    case "ValueSet":
                        Console.WriteLine("Valueset");
                        var v = parser.Parse<Hl7.Fhir.Model.ValueSet>(s);
                        _success.Add((fileName, v));
                        break;
                    case "CodeSystem":
                        Console.WriteLine("CodeSystem");
                        var c = parser.Parse<Hl7.Fhir.Model.CodeSystem>(s);
                        _success.Add((fileName, c));
                        break;
                    default:
                        Console.WriteLine("No Type");
                        break;
                }
            } catch (FormatException fe) {
                _failures.Add((fileName, fe));
            }
        }

        //loadXml
        protected void LoadXml(string s, string fileName) {
            var fr = new FhirResource();
            try {
                var doc = new XmlDocument();
                doc.LoadXml(s);
                XmlElement root = doc.DocumentElement;
                fr.ResourceType = root.Name;
                var children = root.ChildNodes;
                foreach (XmlNode item in children) {
                    if (item.Name == "name") {
                        fr.Name = item.Attributes["value"].Value;
                    }
                    if (item.Name == "url") {
                        fr.Url = item.Attributes["value"].Value;
                    }
                }
            } catch (Exception e) {
                _failures.Add((fileName, e));
            }
            var parser = new FhirXmlParser();
            try {
                switch (fr.ResourceType) {
                    case "Questionnaire":
                        Console.WriteLine("Questionnaire");
                        var q = parser.Parse<Hl7.Fhir.Model.Questionnaire>(s);
                        _success.Add((fileName, q));
                        break;
                    case "ValueSet":
                        Console.WriteLine("Valueset");
                        var v = parser.Parse<Hl7.Fhir.Model.ValueSet>(s);
                        _success.Add((fileName, v));
                        break;
                    case "CodeSystem":
                        Console.WriteLine("CodeSystem");
                        var c = parser.Parse<Hl7.Fhir.Model.CodeSystem>(s);
                        _success.Add((fileName, c));
                        break;
                    default:
                        Console.WriteLine("No Type");
                        break;
                }
            } catch (FormatException fe) {
                _failures.Add((fileName, fe));
            }

        }
    }
    public class FhirResource {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
