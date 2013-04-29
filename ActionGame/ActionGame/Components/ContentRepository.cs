using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.Components
{
    public class ContentRepository
    {
        readonly ActionGame game;
        readonly List<Model> innerBuildings = new List<Model>();
        readonly List<Model> borderBuildings = new List<Model>();

        public ContentRepository(ActionGame game)
        {
            this.game = game;
        }

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
    }
}
