using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ActionGame.World
{
    class EmptyTownQuarterOwner : ITownQuarterOwner
    {
        TownQuarterOwnerContent content;
        public TownQuarterOwnerContent Content
        {
            get { return content; }
        }

        EmptyTownQuarterOwner(TownQuarterOwnerContent content)
        {
            this.content = content;
        }

        static EmptyTownQuarterOwner instance;
        internal static EmptyTownQuarterOwner Instance
        {
            get { return EmptyTownQuarterOwner.instance; }
        } 

        internal static void LoadContent(ContentManager contentManager)
        {
            instance = new EmptyTownQuarterOwner(new TownQuarterOwnerContent
                {
                    RoadSignTexture = contentManager.Load<Texture2D>("Textures/roadSignGreen"),
                    FlagModel = contentManager.Load<Model>("Objects/Decorations/flagEmpty2"),
                    AllyHumanModel = contentManager.Load<Model>("Objects/Humans/human0")
                });
        }

        public People.Human CreateAllyGuard(TownQuarter targetQuarter)
        {
            return null;
        }
    }
}
