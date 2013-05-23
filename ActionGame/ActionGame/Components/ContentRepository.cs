using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Components
{
    /// <summary>
    /// The class for holding all the content in the game. It loads and provides 3D models, textures or sounds.
    /// </summary>
    public class ContentRepository
    {
        readonly ActionGame game;
        readonly List<Model> innerBuildings = new List<Model>();
        readonly List<Model> borderBuildings = new List<Model>();
        readonly List<Model> walkers = new List<Model>();
        readonly List<Texture2D> sidewalks = new List<Texture2D>();
        SoundEffect healSound;
        public SoundEffect HealSound
        {
            get
            {
                return healSound;
            }
        }
        SoundEffect gunLoadSound;
        public SoundEffect GunLoadSound
        {
            get
            {
                return gunLoadSound;
            }
        }
        Model toolBox;
        public Model ToolBox
        {
            get
            {
                return toolBox;
            }
        }
        Model healBox;
        public Model HealBox
        {
            get
            {
                return healBox;
            }
        }
        Texture2D cross;
        Texture2D hurtFullscreenEffect;
        Texture2D respawnFullscreenEffect;
        Texture2D godModeFullscreenEffect;
        Texture2D road;
        Texture2D interfaceWall;
        public Texture2D InterfaceWall
        {
            get
            {
                return interfaceWall;
            }
        }

        public Texture2D Road
        {
            get { return road; }
        }
        Texture2D grass;

        public Texture2D Grass
        {
            get { return grass; }
        }

        public ContentRepository(ActionGame game)
        {
            this.game = game;
        }

        Model opponent;
        public Model Opponent
        {
            get
            {
                return opponent;
            }
        }
        Model player;
        public Model Player
        {
            get
            {
                return player;
            }
        }

        /// <summary>
        /// Loads the whole game audiovisual content.
        /// </summary>
        public void LoadContent()
        {
            ContentManager contentManager = game.Content;
            XmlDocument confDoc = new XmlDocument();
            confDoc.Load(@"Content\Config\UsedObjects.xml");
            XmlNodeList folders = confDoc.SelectNodes("/objects/folder");
            foreach (XmlNode folderNode in folders)
            {
                string folderName = ((XmlElement)folderNode).Attributes["name"].Value;
                XmlNodeList objects = folderNode.SelectNodes("object");
                foreach (XmlNode objectNode in objects)
                {
                    string fileName = ((XmlElement)objectNode).Attributes["file"].Value;
                    XmlNodeList uses = objectNode.SelectNodes("use");
                    Model objModel = contentManager.Load<Model>(String.Format("Objects/{0}/{1}", folderName, fileName));
                    foreach (XmlNode useNode in uses)
                    {
                        switch (((XmlElement)useNode).Attributes["as"].Value)
                        {
                            case "inner":
                                innerBuildings.Add(objModel);
                                break;
                            case "border":
                                borderBuildings.Add(objModel);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            walkers.Add(game.Content.Load<Model>("Objects/Humans/botWalker0"));
            walkers.Add(game.Content.Load<Model>("Objects/Humans/botWalker1"));

            opponent = game.Content.Load<Model>("Objects/Humans/alphaBotYellow");
            player = game.Content.Load<Model>("Objects/Humans/alphaBotBlue");

            toolBox = game.Content.Load<Model>("Objects/Decorations/ammoBox");
            healBox = game.Content.Load<Model>("Objects/Decorations/healthBox");

            gunLoadSound = game.Content.Load<SoundEffect>("Sounds/gunLoading");
            healSound = game.Content.Load<SoundEffect>("Sounds/heal");

            cross = contentManager.Load<Texture2D>("Textures/cross");
            hurtFullscreenEffect = contentManager.Load<Texture2D>("Textures/FullscreenEffects/hurtEffect");
            respawnFullscreenEffect = contentManager.Load<Texture2D>("Textures/FullscreenEffects/respawnEffect");
            godModeFullscreenEffect = contentManager.Load<Texture2D>("Textures/FullscreenEffects/godmode");
            road = game.Content.Load<Texture2D>("Textures/Ground/road0");
            grass = game.Content.Load<Texture2D>("Textures/Ground/grass0");
            interfaceWall = game.Content.Load<Texture2D>("Textures/Spatial/wall0");
            sidewalks.Add(game.Content.Load<Texture2D>("Textures/Ground/sidewalk0"));
            sidewalks.Add(game.Content.Load<Texture2D>("Textures/Ground/sidewalk1"));
        }

        public Texture2D[] Sidewalks
        {
            get
            {
                return sidewalks.ToArray();
            }
        }

        public Model[] BorderBuildings
        {
            get
            {
                return borderBuildings.ToArray();
            }
        }
        public Model[] InnerBuildings
        {
            get
            {
                return innerBuildings.ToArray();
            }
        }
        public Model[] Walkers
        {
            get
            {
                return walkers.ToArray();
            }
        }

        public Texture2D Cross
        {
            get
            {
                return cross;
            }
        }
        public Texture2D HurtFullscreenEffect
        {
            get
            {
                return hurtFullscreenEffect;
            }
        }
        public Texture2D RespawnFullscreenEffect
        {
            get
            {
                return respawnFullscreenEffect;
            }
        }
        public Texture2D GodModeFullscreenEffect
        {
            get
            {
                return godModeFullscreenEffect;
            }
        }
    }
}
