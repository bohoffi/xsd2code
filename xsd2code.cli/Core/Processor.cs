using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xsd2code.cli.Core
{
    public class Processor
    {
        public Processor(string path, ApiVersion version)
        {
            Path = path;
            ApiVersion = version;
        }

        public string Path { get; private set; }
        public ApiVersion ApiVersion { get; private set; }

        public void Process()
        {
            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(Path);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Console.WriteLine("Its a directory");
                ProcessDirectory(Path);
            }
            else
            {
                Console.WriteLine("Its a file");
                ProcessFile(Path);
            }
        }

        private void ProcessDirectory(string directoryPath)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                ProcessFile(filePath);
            }
        }

        private void ProcessFile(string filePath)
        {
            switch (ApiVersion)
            {
                case ApiVersion.v1:
                    ProcessFileV1(filePath);
                    break;
                case ApiVersion.v2:
                    ProcessFileV2(filePath);
                    break;
                default:
                    ProcessFileV1(filePath);
                    break;
            }
        }

        private void ProcessFileV1(string filePath)
        {
            var xsdFile = new v1.Xsd.XsdFile(filePath);
            xsdFile.SimpleTypes.ForEach(simple =>
            {
                var generatedClassNode = new v1.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Generated")
                    .WithClassName(simple.CleanName)
                    .WithAttribute(new v1.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {simple.Name, simple.TargetNamespace }
                        }
                    })
                    .WithClassAccessibility(Accessibility.Public)
                    .Build()
                    .GenerateClassNode();

                CodeToConsole(generatedClassNode);
                Console.WriteLine();
                Console.WriteLine();
            });
            xsdFile.ComplexTypes.ForEach(complex =>
            {
                var generatedClassNode = new v1.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Generated")
                    .WithClassName(complex.CleanName)
                    .WithAttribute(new v1.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {complex.Name, complex.TargetNamespace }
                        }
                    })
                    .WithClassAccessibility(Accessibility.Public)
                    .WithProperties(complex.SequenceElements)
                    .Build()
                    .GenerateClassNode();

                CodeToConsole(generatedClassNode);
                Console.WriteLine();
                Console.WriteLine();
            });
        }

        private void ProcessFileV2(string filePath)
        {
            var xsdFile = new v2.Xsd.XsdFile(filePath);
            xsdFile.SimpleTypes.ForEach(simple =>
            {
                var generatedClassNode = new v2.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithClassName(simple.CleanName)
                    .WithAttribute(new v2.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {"ElementName", simple.Name },
                            {"Namespace", simple.TargetNamespace }
                        }
                    })
                    //.WithClassAccessibility(Accessibility.Public)
                    .Build()
                    .GenerateClassCode();

                var generator = new v2.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithClassName(simple.CleanName)
                    .WithAttribute(new v2.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {"ElementName", simple.Name },
                            {"Namespace", simple.TargetNamespace }
                        }
                    })
                    //.WithClassAccessibility(Accessibility.Public)
                    .Build();

                //CodeToConsole(generatedClassNode);
                generator.GenerateClassNode()
                .Subscribe(CodeToConsole);
                Console.WriteLine();
                Console.WriteLine();
            });
            xsdFile.ComplexTypes.ForEach(complex =>
            {
                var generatedClassNode = new v2.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithClassName(complex.CleanName)
                    .WithAttribute(new v2.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {"ElementName", complex.Name },
                            {"Namespace", complex.TargetNamespace }
                        }
                    })
                    //.WithClassAccessibility(Accessibility.Public)
                    .WithProperties(complex.Elements.Select(e =>
                    {
                        return new v2.Core.ClassGenerator.ClassProperty(e.CleanType)
                        {
                            Name = e.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<v2.Core.ClassGenerator.Attribute>
                            {
                                new v2.Core.ClassGenerator.Attribute
                                {
                                    Name = "XmlElement",
                                    Values = new Dictionary<string, object>
                                    {
                                        {"ElementName", e.Name }
                                    }
                                }
                            }
                        };
                    }).ToList())
                    .WithProperties(complex.Attributes.Select(a =>
                    {
                        return new v2.Core.ClassGenerator.ClassProperty(a.CleanType)
                        {
                            Name = a.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<v2.Core.ClassGenerator.Attribute>
                            {
                                new v2.Core.ClassGenerator.Attribute
                                {
                                    Name="XmlAttribute",
                                    Values=new Dictionary<string, object>
                                    {
                                        {"ElementName", a.Name }
                                    }
                                }
                            }
                        };
                    }).ToList())
                    .Build()
                    .GenerateClassCode();

                var generator = new v2.Core.ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    .WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithClassName(complex.CleanName)
                    .WithAttribute(new v2.Core.ClassGenerator.Attribute
                    {
                        Name = "XmlRoot",
                        Values = new Dictionary<string, object>
                        {
                            {"ElementName", complex.Name },
                            {"Namespace", complex.TargetNamespace }
                        }
                    })
                    //.WithClassAccessibility(Accessibility.Public)
                    .WithProperties(complex.Elements.Select(e =>
                    {
                        return new v2.Core.ClassGenerator.ClassProperty(e.CleanType)
                        {
                            Name = e.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<v2.Core.ClassGenerator.Attribute>
                            {
                                new v2.Core.ClassGenerator.Attribute
                                {
                                    Name = "XmlElement",
                                    Values = new Dictionary<string, object>
                                    {
                                        {"ElementName", e.Name }
                                    }
                                }
                            }
                        };
                    }).ToList())
                    .WithProperties(complex.Attributes.Select(a =>
                    {
                        return new v2.Core.ClassGenerator.ClassProperty(a.CleanType)
                        {
                            Name = a.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<v2.Core.ClassGenerator.Attribute>
                            {
                                new v2.Core.ClassGenerator.Attribute
                                {
                                    Name="XmlAttribute",
                                    Values=new Dictionary<string, object>
                                    {
                                        {"ElementName", a.Name }
                                    }
                                }
                            }
                        };
                    }).ToList())
                    .Build();

                //CodeToConsole(generatedClassNode);
                generator
                    .GenerateClassNode()
                    .Subscribe(CodeToConsole);
                Console.WriteLine();
                Console.WriteLine();
            });
        }

        private void CodeToConsole(SyntaxNode classNode)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                classNode.WriteTo(writer);
                Console.Write(writer.ToString());
            };
        }
    }
}
