using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Levels;

namespace AsteroidRebuttal.Scenes
{
    public class GameScene : Scene
    {
        public AsteroidRebuttal Game;
        public QuadTree quadtree {get; protected set;}
        public List<GameObject> gameObjects { get; private set; }
        public List<Animation> animations { get; private set; }
        public ScriptManager scriptManager;

        public bool PlayerCanFire = true;



        public float currentGameTime { get; private set; }

        // This is the actual GAME window; the UI will appear to the side.
        public Rectangle ScreenArea { get; private set; }
        public GameObject fader { get; private set; }

        // This is the area of the UI.
        public Rectangle GUIArea { get; private set; }

        public PlayerShip player;

        CollisionDetection collisionDetection;

        LevelManager levelManager;

        //Content 
        public static Texture2D ExplosionTexture;
        public static Texture2D PlayerExplosionTexture;

        public static Texture2D BossHealthbarFrameTexture;
        public static Texture2D BossHealthbarBackgroundTexture;
        public static Texture2D BossHealthbarTexture;
        public static Texture2D BossHealthbarDividerTexture;

        public static Texture2D GUITexture;
        public static Texture2D GUITextureExperienceFrame;
        public static Texture2D GUIRankOrb;

        public static Texture2D GUIBossWarningTexture;
        public static Texture2D GUIJammedWarningTexture;

        public static Texture2D GUIWarning;
        public bool BossWarningShown;

        public static SoundEffect Explosion1Sound;
        public static SoundEffect Explosion2Sound;
        public static SoundEffect Explosion3Sound;
        public static SoundEffect Explosion4Sound;
        public static SoundEffect Shot1Sound;
        public static SoundEffect Shot2Sound;
        public static SoundEffect Shot3Sound;
        public static SoundEffect Shot4Sound;
        public static SoundEffect Shot5Sound;
        public static SoundEffect Shot6Sound;
        public static SoundEffect Shot7Sound;
        public static SoundEffect Shot8Sound;

        public static SoundEffect PhaseInSound;
        public static SoundEffect PhaseOutSound;

        public static SoundEffect Rank1;
        public static SoundEffect Rank2;
        public static SoundEffect Rank3;
        public static SoundEffect Rank4;
        public static SoundEffect ExtendSound;

        public static SoundEffect BossWarning;
        public static SoundEffect BossAlarm;
        public static SoundEffect JammedWarning;
        public static SoundEffect JammedAlarm;

        public int Score = 0;
        public List<int> ExtendValues;
        public int ScoreMultiplier = 1;
        public int GrazeCount = 0;
        public int GrazeValue
        {
            get
            {
                return (1 + (GrazeCount / 6)) * ScoreMultiplier;
            }
        }

        public int Lives = 2;
        public float Experience = 0f;
        public float NextRankUp = 20f;
        public float ExperienceDecay = 1f;
        public bool ExperienceDecayPaused = false;
        public float ExperienceDecayPauseDuration = 0f;
        public int Rank = 0;

        SpriteFont scoreFont;
        SpriteFont extendFont;
        SpriteFont livesFont;


        bool bossHealthbarAnimationComplete = false;

        public Boss levelBoss;
        bool bossHealthbarFrameShown = false;
        Color bossHealthbarFrameColor = Color.Transparent;
        Color bossHealthbarColor = Color.Transparent;
        float bossHealthbarWidth = 0;
        float bossHealthbarMaxWidth = 675f;

        Color BossWarningColor = Color.Transparent;
        Color bossWarningStartColor;
        Color bossWarningEndColor;
        float bossWarningColorLerpStartTime;
        float bossWarningColorLerpEndTime;

        Color TitleColor = Color.Transparent;
        Color TitleStartColor;
        Color TitleEndColor;
        float TitleColorLerpStartTime;
        float TitleColorLerpEndTime;

        public GameScene(AsteroidRebuttal thisGame)
        {
            Game = thisGame;
        }

		public Rectangle ScreenAreaIntersection (Rectangle rect)
		{
			return Rectangle.Intersect(ScreenArea, rect);
		}

        public override void Initialize()
        {
            scriptManager = new ScriptManager();
            gameObjects = new List<GameObject>();
            animations = new List<Animation>();

            ExtendValues = new List<int>()
            {
                10000,
                25000,
                50000,
                100000,
                200000,
                400000,
                800000,
                1600000,
                3200000,
                6400000,
                12800000,
                25600000,

            };

            if (AsteroidRebuttal.HardcoreMode)
            {
                ExtendValues.Clear();
                Lives = 0;
            }

            // Set the game area to 700 x 650.
            ScreenArea = new Rectangle(0, 0, 700, 650);
            fader = new Fader(this);
            
            // Set the UI window to 150 x 650, beginning after the ScreenArea.
            GUIArea = new Rectangle(700, 0, 225, 650);

            quadtree = new QuadTree(0, ScreenArea);
            collisionDetection = new CollisionDetection(this);

            levelManager = new LevelManager(this);

            // Test
            levelManager.SetLevel(1);

            //new FinalBoss(this, new Vector2(350, -300));
            player = new PlayerShip(this, new Vector2(350, 550));
        }

        public override void LoadContent(ContentManager content)
        {
            //TEST
            ExplosionTexture = content.Load<Texture2D>("Graphics/Effects/explosion");
            PlayerExplosionTexture = content.Load<Texture2D>("Graphics/Effects/explosion2");

            BossHealthbarBackgroundTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarBackground");
            BossHealthbarFrameTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarFrame");
            BossHealthbarDividerTexture = content.Load<Texture2D>("Graphics/GUI/HealthBarDivision");
            BossHealthbarTexture = content.Load<Texture2D>("Graphics/GUI/HealthBar");

            GUITexture = content.Load<Texture2D>("Graphics/GUI/GUI");
            GUITextureExperienceFrame = content.Load<Texture2D>("Graphics/GUI/GUIExperienceFrame");
            GUIRankOrb = content.Load<Texture2D>("Graphics/GUI/RankOrb");
            scoreFont = content.Load<SpriteFont>("Fonts/ScoreFont");
            extendFont = content.Load<SpriteFont>("Fonts/ExtendFont");
            livesFont = content.Load<SpriteFont>("Fonts/LivesFont");

            GUIBossWarningTexture = content.Load<Texture2D>("Graphics/GUI/BossWarning");
            GUIJammedWarningTexture = content.Load<Texture2D>("Graphics/GUI/CantFiringWarning");

            Explosion1Sound = content.Load<SoundEffect>("Audio/SFX/explosion1");
            Explosion2Sound = content.Load<SoundEffect>("Audio/SFX/explosion2");
            Explosion3Sound = content.Load<SoundEffect>("Audio/SFX/explosion3");
            Explosion4Sound = content.Load<SoundEffect>("Audio/SFX/explosion4");
            Shot1Sound = content.Load<SoundEffect>("Audio/SFX/shot1");
            Shot2Sound = content.Load<SoundEffect>("Audio/SFX/shot2");
            Shot3Sound = content.Load<SoundEffect>("Audio/SFX/shot3");
            Shot4Sound = content.Load<SoundEffect>("Audio/SFX/shot4");
            Shot5Sound = content.Load<SoundEffect>("Audio/SFX/shot5");
            Shot6Sound = content.Load<SoundEffect>("Audio/SFX/shot6");
            Shot7Sound = content.Load<SoundEffect>("Audio/SFX/shot7");
            Shot8Sound = content.Load<SoundEffect>("Audio/SFX/shot8");

            ExtendSound = content.Load<SoundEffect>("Audio/SFX/Extend");

            PhaseInSound = content.Load<SoundEffect>("Audio/SFX/phaseIn");
            PhaseOutSound = content.Load<SoundEffect>("Audio/SFX/phaseOut");

            Rank1 = content.Load<SoundEffect>("Audio/AI/LockAndLoad");
            Rank2 = content.Load<SoundEffect>("Audio/AI/BadBoy");
            Rank3 = content.Load<SoundEffect>("Audio/AI/BulletRider");
            Rank4 = content.Load<SoundEffect>("Audio/AI/HeckYeah");

            BossWarning = content.Load<SoundEffect>("Audio/AI/BossWarning");
            BossAlarm = content.Load<SoundEffect>("Audio/AI/WarningAlarm");
            JammedWarning = content.Load<SoundEffect>("Audio/AI/AllWeaponsJammed");
            JammedAlarm = content.Load<SoundEffect>("Audio/AI/JammedAlarm");
        }


        public override void Update(GameTime gameTime)
        {
            // Get rid of unneeded objects.
            RemoveObjectsOutsideScreen();

            currentGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            foreach (GameObject go in gameObjects.FindAll(x => x.FlaggedForRemoval))
            {
                scriptManager.AbortObjectScripts(go);
                gameObjects.Remove(go);
            }

            foreach (GameObject go in gameObjects.FindAll(x => x.IsNewObject))
            {
                go.IsNewObject = false;
            }

            foreach (Animation a in animations.FindAll(x => x.FlaggedForRemoval))
            {
                animations.Remove(a);
            }

            // Populate the quadtree in preparation for collision checking.
            quadtree.Clear();

            foreach (GameObject go in gameObjects.FindAll(x => !x.IsNewObject))
            {
                go.Update(gameTime);
            }

            foreach (GameObject go in gameObjects.FindAll(x => !x.IsNewObject))
            {
                quadtree.Insert(go);
            }

            foreach (Animation a in animations)
            {
                a.Update(gameTime);
            }

            scriptManager.Update(gameTime);
            collisionDetection.BroadphaseCollisionDetection();

            levelManager.Update(gameTime);

            if (Experience > 0)
            {
                if (!ExperienceDecayPaused)
                {
                    Experience -= ExperienceDecay * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Experience <= 0)
                        ResetRank();
                }
                else if (ExperienceDecayPaused && ExperienceDecayPauseDuration > 0)
                {
                    ExperienceDecayPauseDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(ExperienceDecayPauseDuration <= 0)
                        ExperienceDecayPaused = false;
                }
            }

            CheckExtend();
            AudioManager.PlayQueuedSoundEffects();
        }

        public void CheckExtend()
        {
            if (AsteroidRebuttal.HardcoreMode)
                return;

            if (Score > ExtendValues[0])
            {
                // The player has earned their next extend value!
                // Give them another life.

                Lives++;
                AudioManager.PlaySoundEffect(ExtendSound, .9f, 0f, false);
                // Play a nice sound.

                if (ExtendValues.Count > 1)
                {
                    ExtendValues.RemoveAt(0);
                }
                else
                {
                    ExtendValues[0] *= 2;
                }
            }
        }

        public void RemoveObjectsOutsideScreen()
        {
            foreach (GameObject go in gameObjects)
            {
                if (go.Center.X < ScreenArea.X - go.DeletionBoundary.X ||
                    go.Center.X > ScreenArea.Width + go.DeletionBoundary.X ||
                    go.Center.Y < ScreenArea.Y - go.DeletionBoundary.Y ||
                    go.Center.Y > ScreenArea.Height + go.DeletionBoundary.Y)
                {
                    go.Destroy();
                }
                        
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            levelManager.Draw(spriteBatch);

            foreach (GameObject go in gameObjects)
            {
                go.Draw(spriteBatch);
            }

            foreach (Animation a in animations)
            {
                a.Draw(spriteBatch);
            }

            if (bossHealthbarFrameShown)
            {
                if(bossHealthbarAnimationComplete)
                    bossHealthbarWidth = (levelBoss.Health / levelBoss.MaxHealth) * bossHealthbarMaxWidth;

                spriteBatch.Draw(BossHealthbarBackgroundTexture, Vector2.Zero, null, bossHealthbarFrameColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, .1f);

                spriteBatch.Draw(BossHealthbarTexture, new Rectangle(20, 0, (int)bossHealthbarWidth, 20), null, bossHealthbarColor, 0f, Vector2.Zero, SpriteEffects.None, .09f);

                spriteBatch.Draw(BossHealthbarFrameTexture, Vector2.Zero, null, bossHealthbarFrameColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);

                foreach (int i in levelBoss.PhaseChangeValues)
                {
                    spriteBatch.Draw(BossHealthbarDividerTexture, new Vector2(20 + (((float)i / (float)levelBoss.MaxHealth) * bossHealthbarMaxWidth) - 2, 0), null, bossHealthbarFrameColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, .07f);
                }
            }

            // Draw the GUI!
            spriteBatch.Draw(GUITexture, GUIArea, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, .1f);


            //draw experience bar
            spriteBatch.Draw(BossHealthbarTexture, new Rectangle(GUIArea.X + 36, 166, (int)(150 * (Experience / NextRankUp)), 22), null, Color.PaleGreen, 0f, Vector2.Zero, SpriteEffects.None, .09f);
            spriteBatch.Draw(GUITextureExperienceFrame, new Vector2(GUIArea.X, 163), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);

            if (Rank > 0)
            {
                for (int i = 0; i < Rank; i++)
                { 
                    spriteBatch.Draw(GUIRankOrb, new Vector2(GUIArea.X + 57 + (28 * i), 138), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .09f);
                }
            }

            if (ScoreMultiplier > 1)
            {
                spriteBatch.DrawString(scoreFont, "X" + ScoreMultiplier.ToString(), new Vector2(GUIArea.X + 176, 193), Color.Gold, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);
            }

            spriteBatch.DrawString(scoreFont, String.Format("{0:000,000,000}", Score), new Vector2(GUIArea.X + 42, 38), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);
            if(ExtendValues.Count > 0)
                spriteBatch.DrawString(extendFont, "Next Extend: " + String.Format("{0:0,000}", ExtendValues[0]), new Vector2(GUIArea.X + 42, 74), Color.Gold, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);

            spriteBatch.DrawString(scoreFont, GrazeCount.ToString(), new Vector2(GUIArea.X + 108, 234), Color.Teal, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);
            spriteBatch.DrawString(scoreFont, GrazeValue.ToString(), new Vector2(GUIArea.X + 108, 276), Color.Teal, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);
            spriteBatch.DrawString(livesFont, Lives.ToString(), new Vector2(GUIArea.X + 46, 602), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .08f);

            if (BossWarningShown)
            {
                spriteBatch.Draw(GUIWarning, new Vector2((ScreenArea.Width / 2f) - (GUIWarning.Width / 2f), (ScreenArea.Height / 2f) - (GUIWarning.Height / 2f)), null, BossWarningColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            if (levelManager.currentLevel.TitleShown)
            {
                spriteBatch.Draw(levelManager.currentLevel.TitleTexture, new Vector2(0, 200), null, TitleColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public void AddGameObject(GameObject newObject)
        {
            gameObjects.Add(newObject);
        }


        public override void Unload()
        {
        }


        // Sets the game's level
        public void SetLevel()
        {
        }

        public IEnumerator<float> ShowBossHealthBar()
        {
            bossHealthbarFrameShown = true;
            bossHealthbarColor = Color.Transparent;
            bossHealthbarFrameColor = Color.Transparent;
            bossHealthbarAnimationComplete = false;

            float elapsedTime = 0f;
            float lastTime = currentGameTime;

            while (elapsedTime < 1f)
            {
                bossHealthbarFrameColor = Color.Lerp(Color.Transparent, Color.White, elapsedTime);
                elapsedTime += currentGameTime - lastTime;
                lastTime = currentGameTime;
                yield return .03f;
            }

            bossHealthbarFrameColor = Color.White;
            elapsedTime = 0f;
            bossHealthbarColor = Color.Yellow;            

            while (elapsedTime < 1f)
            {
                bossHealthbarWidth = (elapsedTime / 1) * bossHealthbarMaxWidth;
                elapsedTime += currentGameTime - lastTime;
                lastTime = currentGameTime;
                yield return .03f;
            }

            bossHealthbarAnimationComplete = true;
            bossHealthbarWidth = bossHealthbarMaxWidth;
        }

        public void HideBossHealthbar()
        {
            bossHealthbarFrameShown = false;
            bossHealthbarColor = Color.Transparent;
            bossHealthbarFrameColor = Color.Transparent;
        }

        public void BossPhaseChange()
        {
            // Maybe play a sound or something here.
            bossHealthbarColor = Color.Lerp(Color.Red, Color.Yellow, ((float)levelBoss.Health / (float)levelBoss.MaxHealth));
        }

        public void GainExperience(float xpGain)
        {
            Experience += xpGain;
            if (Experience >= NextRankUp)
                RankUp();
        }

        public void RankUp()
        {
            if (Rank < 4)
            {
                Rank++;
                Experience = 5;
                NextRankUp += 5;
                ExperienceDecay *= 1.15f;
                ScoreMultiplier *= 2;
                PauseExperienceDecay(5f);

                switch (Rank)
                {
                    case (1):
						AudioManager.PlaySoundEffect(Rank1, .8f, 0f, false);
                        break;
                    case (2):
                        AudioManager.PlaySoundEffect(Rank2, .8f, 0f, false);
                        break;
                    case (3):
                        AudioManager.PlaySoundEffect(Rank3, .8f, 0f, false);
                        break;
                    case (4):
                        AudioManager.PlaySoundEffect(Rank4, .8f, 0f, false);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Experience = NextRankUp;
            }
        }

        public void ResetRank()
        {
            Rank = 0;
            Experience = 0;
            ExperienceDecay = .5f;
            ScoreMultiplier = 1;
            NextRankUp = 20f;
        }

        public void PauseExperienceDecay()
        {
            ExperienceDecayPaused = true;
        }

        public void PauseExperienceDecay(float duration)
        {
            ExperienceDecayPaused = true;
            ExperienceDecayPauseDuration = duration;
        }

        public void ResumeExperienceDecay()
        {
            ExperienceDecayPaused = false;
        }

        public void PlayBossWarning()
        {
            scriptManager.Execute(ShowBossWarning);
        }

        //6.5 second total including final fade out
        public IEnumerator<float> ShowBossWarning()
        {
            GUIWarning = GUIBossWarningTexture;
            PauseExperienceDecay(10f);
            BossWarningShown = true;
            LerpBossWarningColor(Color.White, 1.5f);

            AudioManager.PlaySoundEffect(BossAlarm, .8f, 0f, false);
            yield return .75f;

            AudioManager.PlaySoundEffect(BossWarning, .6f, 0f, false);
            yield return .75f;


            for (int i = 0; i < 2; i++)
            {
                AudioManager.PlaySoundEffect(BossAlarm, .8f, 0f, false);
                LerpBossWarningColor(new Color(.5f, .5f, .5f, .5f), 1f);
                yield return 1f;
                LerpBossWarningColor(Color.White, 1f);
                yield return 1f;
            }

            LerpBossWarningColor(Color.Transparent, 1f);
            yield return 1f;
            BossWarningShown = false;
        }

        //5 second total including final fade out
        public IEnumerator<float> ShowJammedWarning()
        {
            GUIWarning = GUIJammedWarningTexture;
            PauseExperienceDecay(10f);
            BossWarningShown = true;
            LerpBossWarningColor(Color.White, 1f);

            AudioManager.PlaySoundEffect(JammedAlarm, .5f, 0f, false);
            yield return .5f;

            AudioManager.PlaySoundEffect(JammedWarning, .8f, 0f, false);
            yield return .5f;


            for (int i = 0; i < 4; i++)
            {
                if(i%2 != 0)
                    AudioManager.PlaySoundEffect(JammedAlarm, .5f, 0f, false);

                LerpBossWarningColor(new Color(.3f, .3f, .3f, .3f), .5f);
                yield return .5f;
                LerpBossWarningColor(Color.White, .5f);
                yield return .5f;
            }

            LerpBossWarningColor(Color.Transparent, .5f);
            yield return 1f;
            BossWarningShown = false;
        }

        public void LerpBossWarningColor(Color targetColor, float duration)
        {
            bossWarningStartColor = BossWarningColor;
            bossWarningEndColor = targetColor;

            bossWarningColorLerpStartTime = currentGameTime;
            bossWarningColorLerpEndTime = currentGameTime + duration;

            scriptManager.Execute(BossWarningColorLerpScript);
        }

        public IEnumerator<float> BossWarningColorLerpScript()
        {
            while (currentGameTime < bossWarningColorLerpEndTime)
            {
                BossWarningColor = Color.Lerp(bossWarningStartColor, bossWarningEndColor, (currentGameTime - bossWarningColorLerpStartTime) / (bossWarningColorLerpEndTime - bossWarningColorLerpStartTime));
                yield return 0.00f;
            }

            BossWarningColor = bossWarningEndColor;
        }

        public void LerpTitleColor(Color targetColor, float duration)
        {
            TitleStartColor = TitleColor;
            TitleEndColor = targetColor;

            TitleColorLerpStartTime = currentGameTime;
            TitleColorLerpEndTime = currentGameTime + duration;

            scriptManager.Execute(TitleColorLerpScript);
        }

        public IEnumerator<float> TitleColorLerpScript()
        {
            while (currentGameTime < TitleColorLerpEndTime)
            {
                TitleColor = Color.Lerp(TitleStartColor, TitleEndColor, (currentGameTime - TitleColorLerpStartTime) / (TitleColorLerpEndTime - TitleColorLerpStartTime));
                yield return 0.00f;
            }

            TitleColor = TitleEndColor;
        }

        public void PlayAnimation(Animation anim)
        {
            animations.Add(anim);
        }

        public bool PointOnScreen(Vector2 point)
        {
            float x, y;
            x = point.X;
            y = point.Y;

            return (x > ScreenArea.X && x < ScreenArea.X + ScreenArea.Width && y > ScreenArea.Y && y < ScreenArea.Y + ScreenArea.Height);
        }

        public IEnumerator<float> TryRespawn()
        {
            // Check if the number of lives is greater than 0
            if (Lives > 0 || AsteroidRebuttal.ImmortalMode)
            {
                // First, wait a second or so for the explosion to complete
                yield return 1.7f;

                // Remove a life.
                if(!AsteroidRebuttal.ImmortalMode)
                    Lives--;

                scriptManager.Execute(SpawnPlayer);
            }
            else
            {
                MediaPlayer.Stop();
				AudioManager.StopBGM();
                levelManager.HaltAudio();
                yield return 2f;
                fader.LerpColor(Color.Black, 2.5f);
                yield return 2.5f;
                Game.ChangeScene(new GameOverScene(Game));
            }
        }

        public IEnumerator<float> SpawnPlayer()
        {
            // Ensure that the last player object was destroyed if it ever existed
            if (player != null)
                player.Destroy();

            // Respawn the player at the set coordinates, with phasing.
            player = new PlayerShip(this, new Vector2(350, 550));
            player.Phasing = true;

            float timeElapsed = 0f;
            while (timeElapsed < 2.1f)
            {
                player.Color = new Color(.35f, .35f, .35f, .35f);
                yield return .06f;

                player.Color = new Color(1f, 1f, 1f, 1f);
                yield return .06f;

                timeElapsed += .12f;
            }

            player.Phasing = false;
        }

        public IEnumerator<float> BossExplosion(GameObject go)
        {
            // First, cancel all scripts, then do this.

            float x;
            float y;

            Random rand = new Random();

            for (int i = 0; i < 30; i++)
            {
                x = go.Center.X + (float)(((rand.NextDouble() * 2f) - 1f) * 75);
                y = go.Center.Y + (float)(((rand.NextDouble() * 2f) - 1f) * 75);

                AudioManager.PlaySoundEffect(GameScene.Explosion4Sound, .4f);
                PlayAnimation(new Animation(GameScene.ExplosionTexture, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 48, 40f, new Vector2(x - 24, y - 25), false));
                yield return .1f;
            }

            if (!(go is FinalBoss))
            {
                AudioManager.PlaySoundEffect(GameScene.Explosion4Sound, 1f);
                AudioManager.PlaySoundEffect(GameScene.Explosion1Sound, .6f);
                fader.LerpColor(Color.White, .3f);

                yield return .3f;
                go.Destroy();
            }
        }
    }

    

}