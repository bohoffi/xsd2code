using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using xsd2code.v2.Core;

namespace xsd2code.v2
{
    public class Processor
    {
        public Processor(ProcessorOptions options)
        {
            Options = options;
        }

        public ProcessorOptions Options { get; private set; }

        public IObservable<List<string>> Process()
        {
            return Observable.Create<List<string>>(observer =>
            {
                var classes = new List<string>();
                Options.Files.Select(file => ProcessFile(file)).ToList().ForEach(classList => classes.AddRange(classList));
                observer.OnNext(classes);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        private List<string> ProcessFile(string filePath)
        {
            var generatedClasses = new List<string>();

            var xsdFile = new Xsd.XsdFile(filePath);

            generatedClasses.AddRange(xsdFile.SimpleTypes.Select(simple =>
            {
                var generatedClassNode = new ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    //.WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
                    .WithClassName(simple.CleanName)
                    .WithAttribute(new ClassGenerator.Attribute
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

                StringBuilder sb = new StringBuilder();
                using (StringWriter writer = new StringWriter(sb))
                {
                    generatedClassNode.WriteTo(writer);
                    return writer.ToString();
                };
            }));
            generatedClasses.AddRange(xsdFile.ComplexTypes.Select(complex =>
            {
                var generatedClassNode = new ClassGenerator.Builder()
                    .WithUsing("System.Xml.Serialization")
                    //.WithNameSpace("Xsd2Code.Classes.Generated")
                    .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
                    .WithClassName(complex.CleanName)
                    .WithAttribute(new ClassGenerator.Attribute
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
                        return new ClassGenerator.ClassProperty(e.CleanType)
                        {
                            Name = e.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<ClassGenerator.Attribute>
                            {
                                new ClassGenerator.Attribute
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
                        return new ClassGenerator.ClassProperty(a.CleanType)
                        {
                            Name = a.CleanName,
                            //Accessibility = Accessibility.Public,
                            IsAutoProperty = true,
                            Attributes = new List<ClassGenerator.Attribute>
                            {
                                new ClassGenerator.Attribute
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

                StringBuilder sb = new StringBuilder();
                using (StringWriter writer = new StringWriter(sb))
                {
                    generatedClassNode.WriteTo(writer);
                    return writer.ToString();
                };
            }));

            //xsdFile.SimpleTypes.ForEach(simple =>
            //{
            //    var generatedClassNode = new ClassGenerator.Builder()
            //        .WithUsing("System.Xml.Serialization")
            //        //.WithNameSpace("Xsd2Code.Classes.Generated")
            //        .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
            //        .WithClassName(simple.CleanName)
            //        .WithAttribute(new ClassGenerator.Attribute
            //        {
            //            Name = "XmlRoot",
            //            Values = new Dictionary<string, object>
            //            {
            //                {"ElementName", simple.Name },
            //                {"Namespace", simple.TargetNamespace }
            //            }
            //        })
            //        //.WithClassAccessibility(Accessibility.Public)
            //        .Build()
            //        .GenerateClassCode();

            //    var generator = new ClassGenerator.Builder()
            //        .WithUsing("System.Xml.Serialization")
            //        //.WithNameSpace("Xsd2Code.Classes.Generated")
            //        .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
            //        .WithClassName(simple.CleanName)
            //        .WithAttribute(new ClassGenerator.Attribute
            //        {
            //            Name = "XmlRoot",
            //            Values = new Dictionary<string, object>
            //            {
            //                {"ElementName", simple.Name },
            //                {"Namespace", simple.TargetNamespace }
            //            }
            //        })
            //        //.WithClassAccessibility(Accessibility.Public)
            //        .Build();

            //    //CodeToConsole(generatedClassNode);
            //    generator.GenerateClassNode()
            //    .Subscribe(CodeToConsole);
            //    Console.WriteLine();
            //    Console.WriteLine();
            //});
            //xsdFile.ComplexTypes.ForEach(complex =>
            //{
            //    var generatedClassNode = new ClassGenerator.Builder()
            //        .WithUsing("System.Xml.Serialization")
            //        //.WithNameSpace("Xsd2Code.Classes.Generated")
            //        .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
            //        .WithClassName(complex.CleanName)
            //        .WithAttribute(new ClassGenerator.Attribute
            //        {
            //            Name = "XmlRoot",
            //            Values = new Dictionary<string, object>
            //            {
            //                {"ElementName", complex.Name },
            //                {"Namespace", complex.TargetNamespace }
            //            }
            //        })
            //        //.WithClassAccessibility(Accessibility.Public)
            //        .WithProperties(complex.Elements.Select(e =>
            //        {
            //            return new ClassGenerator.ClassProperty(e.CleanType)
            //            {
            //                Name = e.CleanName,
            //                //Accessibility = Accessibility.Public,
            //                IsAutoProperty = true,
            //                Attributes = new List<ClassGenerator.Attribute>
            //                {
            //                    new ClassGenerator.Attribute
            //                    {
            //                        Name = "XmlElement",
            //                        Values = new Dictionary<string, object>
            //                        {
            //                            {"ElementName", e.Name }
            //                        }
            //                    }
            //                }
            //            };
            //        }).ToList())
            //        .WithProperties(complex.Attributes.Select(a =>
            //        {
            //            return new ClassGenerator.ClassProperty(a.CleanType)
            //            {
            //                Name = a.CleanName,
            //                //Accessibility = Accessibility.Public,
            //                IsAutoProperty = true,
            //                Attributes = new List<ClassGenerator.Attribute>
            //                {
            //                    new ClassGenerator.Attribute
            //                    {
            //                        Name="XmlAttribute",
            //                        Values=new Dictionary<string, object>
            //                        {
            //                            {"ElementName", a.Name }
            //                        }
            //                    }
            //                }
            //            };
            //        }).ToList())
            //        .Build()
            //        .GenerateClassCode();

            //    var generator = new ClassGenerator.Builder()
            //        .WithUsing("System.Xml.Serialization")
            //        //.WithNameSpace("Xsd2Code.Classes.Generated")
            //        .WithNameSpace(!string.IsNullOrWhiteSpace(Options.Namespace) ? Options.Namespace : "Xsd2Code.Classes.Generated")
            //        .WithClassName(complex.CleanName)
            //        .WithAttribute(new ClassGenerator.Attribute
            //        {
            //            Name = "XmlRoot",
            //            Values = new Dictionary<string, object>
            //            {
            //                {"ElementName", complex.Name },
            //                {"Namespace", complex.TargetNamespace }
            //            }
            //        })
            //        //.WithClassAccessibility(Accessibility.Public)
            //        .WithProperties(complex.Elements.Select(e =>
            //        {
            //            return new ClassGenerator.ClassProperty(e.CleanType)
            //            {
            //                Name = e.CleanName,
            //                //Accessibility = Accessibility.Public,
            //                IsAutoProperty = true,
            //                Attributes = new List<ClassGenerator.Attribute>
            //                {
            //                    new ClassGenerator.Attribute
            //                    {
            //                        Name = "XmlElement",
            //                        Values = new Dictionary<string, object>
            //                        {
            //                            {"ElementName", e.Name }
            //                        }
            //                    }
            //                }
            //            };
            //        }).ToList())
            //        .WithProperties(complex.Attributes.Select(a =>
            //        {
            //            return new ClassGenerator.ClassProperty(a.CleanType)
            //            {
            //                Name = a.CleanName,
            //                //Accessibility = Accessibility.Public,
            //                IsAutoProperty = true,
            //                Attributes = new List<ClassGenerator.Attribute>
            //                {
            //                    new ClassGenerator.Attribute
            //                    {
            //                        Name="XmlAttribute",
            //                        Values=new Dictionary<string, object>
            //                        {
            //                            {"ElementName", a.Name }
            //                        }
            //                    }
            //                }
            //            };
            //        }).ToList())
            //        .Build();

            //    //CodeToConsole(generatedClassNode);
            //    generator
            //        .GenerateClassNode()
            //        .Subscribe(CodeToConsole);
            //    Console.WriteLine();
            //    Console.WriteLine();
            //});

            return generatedClasses;
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
