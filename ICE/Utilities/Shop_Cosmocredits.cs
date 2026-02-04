using System.Collections.Generic;

namespace ICE.Utilities;

public class Shop_Cosmocredits
{
    public static Dictionary<uint, ItemInfo> CosmocreditShop = new()
    {
        [12669] = new ItemInfo
        {
            Name = "Hi-Cordial",
            Cost = 40,
        },
        [45993] = new ItemInfo
        {
            Name = "Queso Fresco",
            Cost = 30,
        },
        [45994] = new ItemInfo
        {
            Name = "Woolback Loin",
            Cost = 30,
        },
        [45990] = new ItemInfo
        {
            Name = "Cassava",
            Cost = 30,
        },
        [45991] = new ItemInfo
        {
            Name = "Splendid Mate Leaves",
            Cost = 30,
        },
        [45992] = new ItemInfo
        {
            Name = "Aji Amarillo",
            Cost = 30,
        },
        [46252] = new ItemInfo
        {
            Name = "Mason's Abrasive",
            Cost = 1000,
        },
        [44035] = new ItemInfo
        {
            Name = "Sungilt Aethersand",
            Cost = 200,
        },
        [44036] = new ItemInfo
        {
            Name = "Mythloam Aethersand",
            Cost = 400,
        },
        [44037] = new ItemInfo
        {
            Name = "Mythroot Aethersand",
            Cost = 400,
        },
        [44038] = new ItemInfo
        {
            Name = "Mythbrine Aethersand",
            Cost = 400,
        },
        [46246] = new ItemInfo
        {
            Name = "Levinchrome Aethersand",
            Cost = 600,
        },
        [44848] = new ItemInfo
        {
            Name = "Condensed Solution",
            Cost = 250,
        },
        [28724] = new ItemInfo
        {
            Name = "Crafter's Delineation",
            Cost = 30,
        },
        [49121] = new ItemInfo
        {
            Name = "Cosmic Exploration Aetheryte Ticket",
            Cost = 60,
        },
        [43856] = new ItemInfo
        {
            Name = "Shucked Clam",
            Cost = 10,
        },
        [43859] = new ItemInfo
        {
            Name = "Ghost Nipper",
            Cost = 10,
        },
        [43858] = new ItemInfo
        {
            Name = "Red Maggots",
            Cost = 10,
        },
        [43857] = new ItemInfo
        {
            Name = "Dragonfly",
            Cost = 10,
        },
        [43854] = new ItemInfo
        {
            Name = "White Worm",
            Cost = 10,
        },
        [43855] = new ItemInfo
        {
            Name = "Popper Lure",
            Cost = 100,
        },
        [30116] = new ItemInfo
        {
            Name = "Ruby Red Dye",
            Cost = 600,
        },
        [30117] = new ItemInfo
        {
            Name = "Cherry Pink Dye",
            Cost = 600,
        },
        [48227] = new ItemInfo
        {
            Name = "Carmine Red Dye",
            Cost = 600,
        },
        [48163] = new ItemInfo
        {
            Name = "Neon Pink Dye",
            Cost = 600,
        },
        [48164] = new ItemInfo
        {
            Name = "Bright Orange Dye",
            Cost = 600,
        },
        [30118] = new ItemInfo
        {
            Name = "Canary Yellow Dye",
            Cost = 600,
        },
        [30119] = new ItemInfo
        {
            Name = "Vanilla Yellow Dye",
            Cost = 600,
        },
        [48166] = new ItemInfo
        {
            Name = "Neon Yellow Dye",
            Cost = 600,
        },
        [48165] = new ItemInfo
        {
            Name = "Neon Green Dye",
            Cost = 600,
        },
        [30120] = new ItemInfo
        {
            Name = "Dragoon Blue Dye",
            Cost = 600,
        },
        [30121] = new ItemInfo
        {
            Name = "Turquoise Blue Dye",
            Cost = 600,
        },
        [48168] = new ItemInfo
        {
            Name = "Azure Blue Dye",
            Cost = 600,
        },
        [48167] = new ItemInfo
        {
            Name = "Violet Purple Dye",
            Cost = 600,
        },
        [30122] = new ItemInfo
        {
            Name = "Gunmetal Black Dye",
            Cost = 1500,
        },
        [30123] = new ItemInfo
        {
            Name = "Pearl White Dye",
            Cost = 1500,
        },
        [30124] = new ItemInfo
        {
            Name = "Metallic Brass Dye",
            Cost = 1500,
        },
        [41762] = new ItemInfo
        {
            Name = "Gatherer's Guerdon Materia XI",
            Cost = 450,
        },
        [41775] = new ItemInfo
        {
            Name = "Gatherer's Guerdon Materia XII",
            Cost = 900,
        },
        [41763] = new ItemInfo
        {
            Name = "Gatherer's Guile Materia XI",
            Cost = 450,
        },
        [41776] = new ItemInfo
        {
            Name = "Gatherer's Guile Materia XII",
            Cost = 900,
        },
        [41764] = new ItemInfo
        {
            Name = "Gatherer's Grasp Materia XI",
            Cost = 450,
        },
        [41777] = new ItemInfo
        {
            Name = "Gatherer's Grasp Materia XII",
            Cost = 900,
        },
        [41765] = new ItemInfo
        {
            Name = "Craftsman's Competence Materia XI",
            Cost = 450,
        },
        [41778] = new ItemInfo
        {
            Name = "Craftsman's Competence Materia XII",
            Cost = 900,
        },
        [41766] = new ItemInfo
        {
            Name = "Craftsman's Cunning Materia XI",
            Cost = 450,
        },
        [41779] = new ItemInfo
        {
            Name = "Craftsman's Cunning Materia XII",
            Cost = 900,
        },
        [41767] = new ItemInfo
        {
            Name = "Craftsman's Command Materia XI",
            Cost = 450,
        },
        [41780] = new ItemInfo
        {
            Name = "Craftsman's Command Materia XII",
            Cost = 900,
        },

    };

    public class ItemInfo
    {
        public string Name { get; set; }
        public uint Cost { get; set; }
    }
}
