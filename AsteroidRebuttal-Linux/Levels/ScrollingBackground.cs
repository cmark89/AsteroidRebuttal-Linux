using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ScrollingBackground
{
    public Texture2D texture;
    public Vector2 position;

    public ScrollingBackground(Texture2D newTexture)
    {
        texture = newTexture;
    }
}