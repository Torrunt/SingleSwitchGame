using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using System.Xml;
using System.IO;

namespace SingleSwitchGame
{
    class AnimatedSpriteFrameNode
    {
        public string Name;
        public AnimatedSpriteData Data;
        public float X, Y;
        public double A, B, C, D;
        public int Depth, TintColour, TintMultiplier;

        public AnimatedSpriteFrameNode(string name, float x, float y, double a, double b, double c, double d, int depth, int tintColour = 0, int tintMultiplier = 0, AnimatedSpriteData data = null)
        {
            Name = name;
            X = x; Y = y; A = a; B = b; C = c; D = d; Depth = depth; TintColour = tintColour; TintMultiplier = tintMultiplier;
            Data = data;
        }
    }

    class AnimatedSpriteFrame
    {
        public int Number;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Vector2f Origin;
        public List<AnimatedSpriteFrameNode> Nodes;

        public AnimatedSpriteFrame(int number, int x, int y, int width, int height, Vector2f origin, List<AnimatedSpriteFrameNode> nodes = null)
        {
            Number = number;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Origin = origin;
            Nodes = nodes;
        }
    }

    class AnimatedSpriteDataReference
    {
        public AnimatedSpriteDataReference(AnimatedSpriteData data, Vector2f origin)
        {
            Data = data;
            Origin = origin;
        }
        public AnimatedSpriteData Data;
        public Vector2f Origin;
    }

    class AnimatedSpriteData
    {
        public string Name = "";
        public Texture Texture = null;
        public List<dynamic> Frames = new List<dynamic>();

        public AnimatedSpriteData(string xml = null)
        {
            AnimatedSpriteData.LoadAnimationFromXMLDocument(this, xml);
        }

        public void AddFrame(int number, int x, int y, int width, int height, Vector2f origin, List<AnimatedSpriteFrameNode> nodes = null)
        {
            if (number >= Frames.Count)
                Frames.Add(new AnimatedSpriteFrame(number, x, y, width, height, origin, nodes));
            else
                Frames.Insert(number, new AnimatedSpriteFrame(number, x, y, width, height, origin, nodes));
        }
        public void AddFrameReference(int number, int reference)
        {
            if (number >= Frames.Count)
                Frames.Add(reference);
            else
                Frames.Insert(number, reference);
        }
        public void AddAnimationReference(int number, AnimatedSpriteDataReference spriteAnimationDataReference)
        {
            if (number >= Frames.Count)
                Frames.Add(spriteAnimationDataReference);
            else
                Frames.Insert(number, spriteAnimationDataReference);
        }

        // Loading Animation
        public static void LoadAnimationFromXMLDocument(AnimatedSpriteData animatedSpriteData, string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            string contents = doc.InnerXml;
            string directory = filename.Substring(0, filename.LastIndexOf("/") + 1);

            using (XmlReader reader = XmlReader.Create(new StringReader(contents)))
            {
                reader.ReadToFollowing("SpriteSheet");

                string name = reader.GetAttribute("name");
                string image = reader.GetAttribute("image");

#if DEBUG
                Console.WriteLine("-----------------------------");
                Console.WriteLine("name : " + name);
                Console.WriteLine("xml  : " + filename);
                if (image != null)
                    Console.WriteLine("image: " + directory + image);
                Console.WriteLine("-----------------------------");
#endif
                // Texture
                if (image != null)
                    animatedSpriteData.Texture = Graphics.GetTexture(directory + image);

                // Frames
                reader.ReadToDescendant("frame");
                int no = 0;
                do
                {
  
#if DEBUG
                    Console.Write(reader.GetAttribute("no") + "  ");
#endif

                    string reference = reader.GetAttribute("ref");
                    if (reference != null)
                    {
                        // Frame or Animation Reference
                        int reference_no;
                        if (int.TryParse(reference, out reference_no))
                        {
                            // Frame Reference
                            #if DEBUG
                            Console.WriteLine("Frame Reference: " + reference_no);
                            #endif

                            animatedSpriteData.AddFrameReference(no, reference_no);
                        }
                        else
                        {
                            // Animation Reference               
#if DEBUG
                            Console.WriteLine("Animation Reference: " + Graphics.ASSETS_SPRITES + reference + ".xml");
                            Console.WriteLine("   --->");
#endif

                            animatedSpriteData.AddAnimationReference(
                                no,
                                new AnimatedSpriteDataReference(
                                    Graphics.GetAnimatedSpriteData(Graphics.ASSETS_SPRITES + reference + ".xml"),
                                    new Vector2f(Convert.ToInt16(reader.GetAttribute("x")), Convert.ToInt16(reader.GetAttribute("y")))
                                    )
                                );

                            
#if DEBUG
                            Console.WriteLine("   <---");
#endif
                        }
                    }
                    else
                    {
                        // Frame
                        //<frame no="0" x="0" y="0" ref_x="26.25" ref_y="83.15" width="61" height="88"/>

#if DEBUG
                        Console.WriteLine("Frame");
#endif

                        int x = Convert.ToInt16(reader.GetAttribute("x"));
                        int y = Convert.ToInt16(reader.GetAttribute("y"));
                        int width = Convert.ToInt16(reader.GetAttribute("width"));
                        int height = Convert.ToInt16(reader.GetAttribute("height"));
                        Vector2f origin = new Vector2f(Convert.ToSingle(reader.GetAttribute("ref_x")), Convert.ToSingle(reader.GetAttribute("ref_y")));

                        // Nodes
                        List<AnimatedSpriteFrameNode> nodes = new List<AnimatedSpriteFrameNode>();
                        if (reader.ReadToDescendant("node"))
                        {
                            int node_no = 0;
                            while (reader.HasAttributes)
                            {
                                string spriteSheet = reader.GetAttribute("spriteSheet");
                                nodes.Add(new AnimatedSpriteFrameNode(
                                    reader.GetAttribute("name"),
                                    Convert.ToSingle(reader.GetAttribute("x")),
                                    Convert.ToSingle(reader.GetAttribute("y")),
                                    Convert.ToDouble(reader.GetAttribute("a")),
                                    Convert.ToDouble(reader.GetAttribute("b")),
                                    Convert.ToDouble(reader.GetAttribute("c")),
                                    Convert.ToDouble(reader.GetAttribute("d")),
                                    Convert.ToInt16(reader.GetAttribute("depth")),
                                    0, //Convert.ToInt16(reader.GetAttribute("tintColour")), 
                                    Convert.ToInt16(reader.GetAttribute("tintMultiplier")),
                                    spriteSheet == null ? null : Graphics.GetAnimatedSpriteData(Graphics.ASSETS_SPRITES + spriteSheet + ".xml")
                                    ));

#if DEBUG
                                Console.WriteLine("    node: " + nodes[nodes.Count - 1].Name);
                                Console.WriteLine("          x      : " + nodes[nodes.Count - 1].X);
                                Console.WriteLine("          y      : " + nodes[nodes.Count - 1].Y);
                                Console.WriteLine("          a      : " + nodes[nodes.Count - 1].A);
                                Console.WriteLine("          b      : " + nodes[nodes.Count - 1].B);
                                Console.WriteLine("          c      : " + nodes[nodes.Count - 1].C);
                                Console.WriteLine("          d      : " + nodes[nodes.Count - 1].D);
                                Console.WriteLine("          depth  : " + nodes[nodes.Count - 1].Depth);
                                //Console.WriteLine("          tint   : " + nodes[nodes.Count - 1].TintColour);
                                //Console.WriteLine("          tintM  : " + nodes[nodes.Count - 1].TintMultiplier);
                                Console.WriteLine("          sprite : " + Graphics.ASSETS_SPRITES + spriteSheet + ".xml");
#endif

                                reader.ReadToNextSibling("node");
                                node_no++;
                            }
                        }
                        
                        animatedSpriteData.AddFrame(no, x, y, width, height, origin, nodes);
                    }


                    reader.ReadToNextSibling("frame");
                    no++;
                }
                while (reader.HasAttributes);
            }
        }

    }
}
