using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static System.Environment;


string desktop = GetFolderPath(SpecialFolder.DesktopDirectory);

string folderPath = Path.Combine(desktop, "Serialisierung");

if (!Directory.Exists(folderPath))
    Directory.CreateDirectory(folderPath);

Console.WriteLine(folderPath);

string filePath = Path.Combine(folderPath, "Test.txt");

List<Fahrzeug> fahrzeuge = new List<Fahrzeug>
    {
        new Fahrzeug(0, 251, FahrzeugMarke.BMW),
        new Fahrzeug(1, 274, FahrzeugMarke.BMW),
        new Fahrzeug(2, 146, FahrzeugMarke.BMW),
        new Fahrzeug(3, 208, FahrzeugMarke.Audi),
        new Fahrzeug(4, 189, FahrzeugMarke.Audi),
        new Fahrzeug(5, 133, FahrzeugMarke.VW),
        new Fahrzeug(6, 253, FahrzeugMarke.VW),
        new Fahrzeug(7, 304, FahrzeugMarke.BMW),
        new Fahrzeug(8, 151, FahrzeugMarke.VW),
        new Fahrzeug(9, 250, FahrzeugMarke.VW),
        new Fahrzeug(10, 217, FahrzeugMarke.Audi),
        new Fahrzeug(11, 125, FahrzeugMarke.Audi)
    };

    string json = System.Text.Json.JsonSerializer.Serialize(fahrzeuge[0], options: new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, Converters = { new JsonStringEnumConverter() } });
	File.WriteAllText(filePath, json);

    Console.WriteLine(json);

    Fahrzeug read = System.Text.Json.JsonSerializer.Deserialize<Fahrzeug>(json, options: new JsonSerializerOptions { IncludeFields = true, Converters = { new JsonStringEnumConverter() } });

    Console.WriteLine(read.Marke);

