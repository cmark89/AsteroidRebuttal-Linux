using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using AsteroidRebuttal.Scenes;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace AsteroidRebuttal.Levels
{
    public class LevelManager
    {
        public GameScene thisScene;
        ScriptManager scriptManager;
        public Level currentLevel;

        public LevelManager(GameScene newScene)
        {
            thisScene = newScene;
            scriptManager = newScene.scriptManager;
        }

        public static void LoadContent(ContentManager content)
        {
            Level1.Level1GroundTexture = content.Load<Texture2D>("Graphics/Backgrounds/level1");
            Level2.Level2GroundTexture = content.Load<Texture2D>("Graphics/Backgrounds/level2");
            Level2.Level2CloudTexture = content.Load<Texture2D>("Graphics/Backgrounds/level2clouds");
            Level3.Level3GroundTexture = content.Load<Texture2D>("Graphics/Backgrounds/level3");
            Level4.Level4GroundTexture = content.Load<Texture2D>("Graphics/Backgrounds/level4");
            Level4.Level4Text1_1 = content.Load<Texture2D>("Graphics/Backgrounds/text1-1");
			Level4.Level4Text1_2 = content.Load<Texture2D>("Graphics/Backgrounds/text1-2");
			Level4.Level4Text1_3 = content.Load<Texture2D>("Graphics/Backgrounds/text1-3");
			Level4.Level4Text1_4 = content.Load<Texture2D>("Graphics/Backgrounds/text1-4");
			Level4.Level4Text1_5 = content.Load<Texture2D>("Graphics/Backgrounds/text1-5");
			Level4.Level4Text1_6 = content.Load<Texture2D>("Graphics/Backgrounds/text1-6");
            Level4.Level4Text2_1 = content.Load<Texture2D>("Graphics/Backgrounds/text2-1");
			Level4.Level4Text2_2 = content.Load<Texture2D>("Graphics/Backgrounds/text2-2");
			Level4.Level4Text2_3 = content.Load<Texture2D>("Graphics/Backgrounds/text2-3");
			Level4.Level4Text2_4 = content.Load<Texture2D>("Graphics/Backgrounds/text2-4");
			Level4.Level4Text2_5 = content.Load<Texture2D>("Graphics/Backgrounds/text2-5");
			Level4.Level4Text3_1 = content.Load<Texture2D>("Graphics/Backgrounds/text3-1");
			Level4.Level4Text3_2 = content.Load<Texture2D>("Graphics/Backgrounds/text3-2");
			Level4.Level4Text3_3 = content.Load<Texture2D>("Graphics/Backgrounds/text3-3");
			Level4.Level4Text4_1 = content.Load<Texture2D>("Graphics/Backgrounds/text4-1");
			Level4.Level4Text4_2 = content.Load<Texture2D>("Graphics/Backgrounds/text4-2");
			Level4.Level4Text4_3 = content.Load<Texture2D>("Graphics/Backgrounds/text4-3");
            Level4.Level4Warning1Texture = content.Load<Texture2D>("Graphics/Backgrounds/warning1");
            Level4.Level4Warning2Texture = content.Load<Texture2D>("Graphics/Backgrounds/warning2");
            Level5.Level5SpaceTexture = content.Load<Texture2D>("Graphics/Backgrounds/level5space");
            Level5.Level5NebulaTexture = content.Load<Texture2D>("Graphics/Backgrounds/level5nebula");
            Level5.Level5Stars1Texture = content.Load<Texture2D>("Graphics/Backgrounds/level5farstars");
            Level5.Level5Stars2Texture = content.Load<Texture2D>("Graphics/Backgrounds/level5midstars");
            Level5.Level5Stars3Texture = content.Load<Texture2D>("Graphics/Backgrounds/level5closestars");
            Level5.Level5DebrisTexture = content.Load<Texture2D>("Graphics/Backgrounds/level5debris");

            Level1.Level1Theme = content.Load<SoundEffect>("Audio/Music/ThisWarTornMote");
            Level2.Level2Theme = content.Load<SoundEffect>("Audio/Music/AboveTheCarnage");
            Level3.Level3Theme = content.Load<SoundEffect>("Audio/Music/ApolloGraveyard");
            Level4.Level4Theme = content.Load<SoundEffect>("Audio/Music/Technoclasm");
            Level5.Level5Theme = content.Load<SoundEffect>("Audio/Music/AsteroidRebuttal");
            Level5.FinalAttackTheme = content.Load<SoundEffect>("Audio/Music/ReptilianFinale");

            Level1.Level1TitleTexture = content.Load<Texture2D>("Graphics/GUI/Stage1Title");
            Level2.Level2TitleTexture = content.Load<Texture2D>("Graphics/GUI/Stage2Title");
            Level3.Level3TitleTexture = content.Load<Texture2D>("Graphics/GUI/Stage3Title");
            Level4.Level4TitleTexture = content.Load<Texture2D>("Graphics/GUI/Stage4Title");
            Level5.Level5TitleTexture = content.Load<Texture2D>("Graphics/GUI/Stage5Title");

            Level.BossTheme = content.Load<SoundEffect>("Audio/Music/Incursion");
        }

        public void Update(GameTime gameTime)
        {
            if (currentLevel != null)
                currentLevel.Update(gameTime);
        }

        public void HaltAudio()
        {
            if (currentLevel.bossTheme != null)
                currentLevel.bossTheme.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentLevel != null)
                currentLevel.Draw(spriteBatch);
        }

        public void SetLevel(int levelNumber)
        {
            Level newLevel = new Level(this);
            switch (levelNumber)
            {
                case(1):
                    newLevel = new Level1(this);
                    break;
                case (2):
                    newLevel = new Level2(this);
                    break;
                case (3):
                    newLevel = new Level3(this);
                    break;
                case (4):
                    newLevel = new Level4(this);
                    break;
                case (5):
                    newLevel = new Level5(this);
                    break;
                default:
                    break;
            }

            currentLevel = newLevel;
            scriptManager.Execute(currentLevel.LevelScript);
        }
    }
}
