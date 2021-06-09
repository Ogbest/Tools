using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using OfficeOpenXml;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class Exporter_Protobuf
    {

        // 根据生成的类，动态编译把json转成protobuf
        public static void ExportExcelProtobuf(string classPath, string protoPath, string jsonPath, string template)
        {
            string @namespace = GetNamespace(template);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            List<string> protoNames = new List<string>();
            foreach (string classFile in Directory.GetFiles(classPath, "*.cs"))
            {
                protoNames.Add(Path.GetFileNameWithoutExtension(classFile));
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(classFile)));
            }

            List<PortableExecutableReference> references = new List<PortableExecutableReference>();

            string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            references.Add(AssemblyMetadata.CreateFromFile(typeof(object).Assembly.Location).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(typeof(ProtoMemberAttribute).Assembly.Location).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(typeof(BsonDefaultValueAttribute).Assembly.Location).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(typeof(IConfig).Assembly.Location).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(typeof(Attribute).Assembly.Location).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(Path.Combine(assemblyPath, "System.dll")).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(Path.Combine(assemblyPath, "netstandard.dll")).GetReference());
            references.Add(AssemblyMetadata.CreateFromFile(typeof(ISupportInitialize).Assembly.Location).GetReference());

            CSharpCompilation compilation = CSharpCompilation.Create(
                null,
                syntaxTrees.ToArray(),
                references.ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using MemoryStream memSteam = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memSteam);
            if (!emitResult.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Diagnostic t in emitResult.Diagnostics)
                {
                    stringBuilder.AppendLine(t.GetMessage());
                }
                throw new Exception($"动态编译失败:\n{stringBuilder}");
            }

            memSteam.Seek(0, SeekOrigin.Begin);

            Assembly ass = Assembly.Load(memSteam.ToArray());

            string dir = protoPath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (string protoName in protoNames)
            {
                Type type = ass.GetType($"{@namespace}.{protoName}Category");
                Type subType = ass.GetType($"{@namespace}.{protoName}");
                Serializer.NonGeneric.PrepareSerializer(type);
                Serializer.NonGeneric.PrepareSerializer(subType);

                string json = File.ReadAllText(Path.Combine(jsonPath, $"{protoName}.txt"));

                object deserialize = BsonSerializer.Deserialize(json, type);

                string path = Path.Combine(dir, $"{protoName}Category.bytes");

                using FileStream file = File.Create(path);
                Serializer.Serialize(file, deserialize);
            }
        }

        public static string GetNamespace(string templatePath)
        {
            string[] lines = templatePath.Split("\n"); //File.ReadAllLines(templatePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("namespace"))
                {
                    return line.Replace("namespace", "").Trim();
                }
            }
            return "ETHotfix";
        }

    }
}
