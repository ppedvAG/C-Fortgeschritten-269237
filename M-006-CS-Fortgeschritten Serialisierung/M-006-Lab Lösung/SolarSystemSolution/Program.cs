using SolarSystem;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

Node<SolarItem> sunNode = new Node<SolarItem>(new SolarItem("Sonne", SolarItemType.Star));

Node<SolarItem> merkurNode = new Node<SolarItem>(new SolarItem("Merkur", SolarItemType.Planet));
merkurNode.SetParentNode(sunNode); //Variante 1 -> ParentNode setzen

Node<SolarItem> venusNode = new Node<SolarItem>(new SolarItem("Venus", SolarItemType.Planet));

Node<SolarItem> earthNode = new Node<SolarItem>(new SolarItem("Erde", SolarItemType.Planet));
earthNode.AddChild(new SolarItem("Mond", SolarItemType.Trabant));

Node<SolarItem> marsNode = new Node<SolarItem>(new SolarItem("Mars", SolarItemType.Planet));
marsNode.AddChild(new SolarItem("Phobos", SolarItemType.Trabant));
marsNode.AddChild(new SolarItem("Deimos", SolarItemType.Trabant));

Node<SolarItem> jupiterNode = new Node<SolarItem>(new SolarItem("Jupiter", SolarItemType.Planet));
jupiterNode.AddChild(new SolarItem("Europa", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Io", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Ganymed", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Kallisto", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Metis", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Adrastea", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Amalthea", SolarItemType.Trabant));
jupiterNode.AddChild(new SolarItem("Thebe", SolarItemType.Trabant));

Node<SolarItem> saturnNode = new Node<SolarItem>(new SolarItem("Saturn", SolarItemType.Planet));
saturnNode.AddChild(new SolarItem("Titan", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Rhea", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Dione", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Tethys", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Japetus", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Telesto", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Calypso", SolarItemType.Trabant));
saturnNode.AddChild(new SolarItem("Tethys", SolarItemType.Trabant));

Node<SolarItem> uranusNode = new Node<SolarItem>(new SolarItem("Uranus", SolarItemType.Planet));
uranusNode.AddChild(new SolarItem("Miranda", SolarItemType.Trabant));
uranusNode.AddChild(new SolarItem("Ariel", SolarItemType.Trabant));
uranusNode.AddChild(new SolarItem("Umbriel", SolarItemType.Trabant));
uranusNode.AddChild(new SolarItem("Titania", SolarItemType.Trabant));
uranusNode.AddChild(new SolarItem("Oberon", SolarItemType.Trabant));
uranusNode.AddChild(new SolarItem("Triton", SolarItemType.Trabant));

Node<SolarItem> neptunNode = new Node<SolarItem>(new SolarItem("Neptun", SolarItemType.Planet));
neptunNode.AddChild(new SolarItem("Triton", SolarItemType.Trabant));
neptunNode.AddChild(new SolarItem("Proteus", SolarItemType.Trabant));
neptunNode.AddChild(new SolarItem("Halimede", SolarItemType.Trabant));
neptunNode.AddChild(new SolarItem("Nereid", SolarItemType.Trabant));
neptunNode.AddChild(new SolarItem("Naiad", SolarItemType.Trabant));
neptunNode.AddChild(new SolarItem("Thalasaa", SolarItemType.Trabant));

sunNode.AddChild(merkurNode);
sunNode.AddChild(venusNode);
sunNode.AddChild(earthNode);
sunNode.AddChild(marsNode);
sunNode.AddChild(jupiterNode);
sunNode.AddChild(saturnNode);
sunNode.AddChild(uranusNode);
sunNode.AddChild(neptunNode);

DisplaySolarSystem(sunNode);

Console.WriteLine("----------------------------------------------");
//JSON 
Console.WriteLine("JSON speichern/laden/anzeigen");
//Speichern 
SaveWithJsonSerializer(sunNode);
//Laden
Node<SolarItem> newSunNode = LoadWithJsonSerilizer();
//Anzeigen
ParentNodeBuilder(newSunNode);
DisplaySolarSystem(newSunNode);
Console.WriteLine("----------------------------------------------");


//Xml Formatter
Console.WriteLine("XmlSerializer speichern/laden/anzeigen");
SaveWithXmlSerilizer(newSunNode);
Node<SolarItem> NewSunNode2 = LoadWithXmlSerilizer();
ParentNodeBuilder(NewSunNode2);
DisplaySolarSystem(NewSunNode2);



void DisplaySolarSystem(Node<SolarItem> solarNode)
{
    if (solarNode.Item.Type == SolarItemType.Star)
        Console.WriteLine($"Sonne: {solarNode.Item.Description}");

    if (solarNode.Item.Type == SolarItemType.Planet)
        Console.WriteLine($"\tPlanet: {solarNode.Item.Description} - kreist um {solarNode.ParentNode.Item.Description}");

    if (solarNode.Item.Type == SolarItemType.Trabant)
        Console.WriteLine($"\t\t -Mond: {solarNode.Item.Description} - kreist um {solarNode.ParentNode.Item.Description}");

    if (solarNode.Item.Type != SolarItemType.Trabant)
    {
        if (solarNode.Item.Description == "Erde")
        {

        }

        foreach (Node<SolarItem> node in solarNode.Childrens)
        {
            DisplaySolarSystem(node);
        }
    }
}

void ParentNodeBuilder(Node<SolarItem> currentNode, Node<SolarItem> parentNode = default!)
{
    // Root hat keinen Parent; alle anderen Knoten bekommen den übergebenen Parent.
    if (parentNode != null)
        currentNode.ParentNode = parentNode;

    // Rekursiv für alle Ebenen (Planeten, Monde, ...)
    foreach (Node<SolarItem> childNode in currentNode.Childrens)
    {
        ParentNodeBuilder(childNode, currentNode);
    }
}

void SaveWithJsonSerializer(Node<SolarItem> solarNode)
{
    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    jsonSerializerOptions.WriteIndented = true;

    string output = JsonSerializer.Serialize<Node<SolarItem>>(solarNode, jsonSerializerOptions);
    File.WriteAllText("Solar.json", output);
}

Node<SolarItem> LoadWithJsonSerilizer()
{
    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    string input = File.ReadAllText("Solar.json");
    Node<SolarItem>? sun = JsonSerializer.Deserialize<Node<SolarItem>>(input, jsonSerializerOptions);

    if (sun == null)
        throw new JsonException("Solar.json konnte nicht gelesen werden.");

    return sun;
}



void SaveWithXmlSerilizer(Node<SolarItem> solarNode)
{
    // Konkrete Typen für Children müssen explizit übergeben werden, da XmlSerializer generische Typen nicht automatisch erkennt
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Node<SolarItem>), new Type[] { typeof(Node<SolarItem>), typeof(SolarItem) });
    using Stream stream = new FileStream("Solar.xml", FileMode.Create, FileAccess.Write);
    xmlSerializer.Serialize(stream, solarNode);
}

Node<SolarItem> LoadWithXmlSerilizer()
{
    // Dieselben Typen wie beim Speichern angeben
    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Node<SolarItem>), new Type[] { typeof(Node<SolarItem>), typeof(SolarItem) });
    using Stream stream = new FileStream("Solar.xml", FileMode.Open, FileAccess.Read);
    Node<SolarItem> sunNode = (Node<SolarItem>)xmlSerializer.Deserialize(stream)!;
    return sunNode;
}