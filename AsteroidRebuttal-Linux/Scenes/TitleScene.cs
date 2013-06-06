using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidRebuttal.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using AsteroidRebuttal.GameObjects;
using AsteroidRebuttal.Enemies;
using AsteroidRebuttal.Enemies.Bosses;
using AsteroidRebuttal.Scripting;
using AsteroidRebuttal.Core;
using AsteroidRebuttal.Levels;
using Microsoft.Xna.Framework.Input;

namespace AsteroidRebuttal.Scenes
{
    public class TitleScene : Scene
    {
        public static Texture2D TitleScreenTexture;
        public static Texture2D HelpTexture;
        public static Texture2D CreditsTexture;
        public static Texture2D TitleTextJapanese;
        public static Texture2D TitleTextEnglish;

        public static SpriteFont titlescreenFont;

        Color backgroundCurrentColor = Color.Black;
        Color text1CurrentColor = Color.Transparent;
        Color text2CurrentColor = Color.Transparent;
        Color cheatTextCurrentColor = Color.Transparent;
        string cheatText = "";
        string cheatInputText;

        public static SoundEffect TitleTheme;
        public static SoundEffect CheatSound;
        public static SoundEffect GameStartSound;

        AsteroidRebuttal Game;
        int fadephase = 0;
        float time = 0;
        float startTextNextChange;
        float cheatInputTime;

        bool HelpShown;
        bool CreditsShown;



        SoundEffectInstance music;

        Color startTextColor = Color.Transparent;

        public TitleScene(AsteroidRebuttal thisGame)
        {
            Game = thisGame;
        }

        public override void Initialize()
        {
            music = TitleTheme.CreateInstance();
            music.IsLooped = true;
            music.Play();
            AsteroidRebuttal.ImmortalMode = false;
            AsteroidRebuttal.ManicMode = false;
            AsteroidRebuttal.HardcoreMode = false;
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (cheatTextCurrentColor != Color.Transparent)
            {
                cheatTextCurrentColor = Color.Lerp(Color.Gold, Color.Transparent, ((float)gameTime.TotalGameTime.TotalSeconds - cheatInputTime) / 4f);
            }

            if (fadephase < 2 && (KeyboardManager.KeyPressedUp(Keys.Space) || KeyboardManager.KeyPressedUp(Keys.Enter) || KeyboardManager.KeyPressedUp(Keys.Escape) || GamepadManager.ProceedButtonDown()))
            {
                backgroundCurrentColor = Color.White;
                text1CurrentColor = Color.White;
                text2CurrentColor = Color.White;
                startTextColor = Color.White;
                startTextNextChange = time + .8f;
                fadephase = 2;
            }

            // Process input on the main screen...
            if (fadephase == 2)
            {
                if (KeyboardManager.KeyDown(Keys.LeftShift) || GamepadManager.AnyShoulderButtonDown())
                {
                    // Get the input to add to the cheat string.
                    if (KeyboardManager.KeyPressedUp(Keys.Up) || GamepadManager.UpButtonPressedUp()) { cheatInputText = String.Concat(cheatInputText, "U"); }
                    if (KeyboardManager.KeyPressedUp(Keys.Down) || GamepadManager.DownButtonPressedUp()) { cheatInputText = String.Concat(cheatInputText, "D"); }
                    if (KeyboardManager.KeyPressedUp(Keys.Left) || GamepadManager.LeftButtonPressedUp()) { cheatInputText = String.Concat(cheatInputText, "L"); }
                    if (KeyboardManager.KeyPressedUp(Keys.Right) || GamepadManager.RightButtonPressedUp()) { cheatInputText = String.Concat(cheatInputText, "R"); }

                    CheckCheats(gameTime);
                }
                else
                {
                    cheatInputText = "";
                }


                if ((KeyboardManager.KeyPressedDown(Keys.Enter) || GamepadManager.ProceedButtonDown()) && !HelpShown && !CreditsShown)
                {
                    // Begin the game...
                    fadephase = 3;
                    music.Stop();
                    time = 0f;
                    startTextColor = Color.White;
                    startTextNextChange = 0.05f;
                    GameStartSound.Play();
                }
                else
                {
                    if (KeyboardManager.KeyDown(Keys.C))
                        CreditsShown = true;
                    else
                        CreditsShown = false;

                    if (KeyboardManager.KeyDown(Keys.H))
                        HelpShown = true;
                    else
                        HelpShown = false;
                }
            }
            
            switch (fadephase)
            {
                case(0):
                    backgroundCurrentColor = Color.Lerp(Color.Black, Color.White, time / 3f);
                    if (backgroundCurrentColor == Color.White) { fadephase = 1; }
                    break;
                case(1):
                    if(time > 3.5f) 
                    {
                        text1CurrentColor = Color.Lerp(Color.Transparent, Color.White, (time - 3.5f) / 1.8f);
                        text2CurrentColor = Color.Lerp(Color.Transparent, Color.White, (time - 5f) / 3f);
                    }
                    if (time > 8f) 
                    {
                        
                        startTextColor = Color.White;
                        startTextNextChange = 8.8f;
                        fadephase = 2;
                    }
                    break;
                case (2):
                    {
                        if (time > startTextNextChange)
                        {
                            if (startTextColor == Color.White)
                                startTextColor = Color.Transparent;
                            else
                                startTextColor = Color.White;

                            startTextNextChange += .8f;
                        }
                    }
                    break;
                case (3):
                    if (time > startTextNextChange)
                    {
                        if (startTextColor == Color.White)
                            startTextColor = Color.Yellow;
                        else
                            startTextColor = Color.White;

                        startTextNextChange += .05f;
                    }
                    if (time > 1.5f) 
                    {
                        startTextColor = Color.Transparent;
                        fadephase = 4;
                        time = 0;
                    }
                    break;
                case(4):
                    backgroundCurrentColor = Color.Lerp(Color.White, Color.Black, time / 1.5f);
                    text1CurrentColor = Color.Lerp(Color.White, Color.Black, time / 1.5f);
                    text2CurrentColor = Color.Lerp(Color.White, Color.Black, time / 1.5f);
                    //startTextColor = Color.Lerp(Color.White, Color.Black, time / 1.5f);

                    if (time > 1.8f)
                    {
                        Game.ChangeScene(new GameScene(Game));
                    }
                    break;
                default:
                    break;
            }
        }

        void CheckCheats(GameTime gameTime)
        {
            if (cheatInputText == "UULRLRD" && !AsteroidRebuttal.ImmortalMode)
            {
                AsteroidRebuttal.ImmortalMode = true;
                cheatInputTime = (float)gameTime.TotalGameTime.TotalSeconds;
                ActivateImmortalMode();
                cheatInputText = "";
            }
            else if (cheatInputText == "RDLRDLUU" && !AsteroidRebuttal.ManicMode)
            {
                AsteroidRebuttal.ManicMode = true;
                cheatInputTime = (float)gameTime.TotalGameTime.TotalSeconds;
                ActivateManicMode();
                cheatInputText = "";
            }
            else if (cheatInputText == "RULDRDLUD" && !AsteroidRebuttal.HardcoreMode)
            {
                AsteroidRebuttal.HardcoreMode = true;
                cheatInputTime = (float)gameTime.TotalGameTime.TotalSeconds;
                ActivateHardcoreMode();
                cheatInputText = "";
            }
        }

        void ActivateImmortalMode()
        {
            CheatSound.Play();
            cheatText = "Immortal mode activated";
            cheatTextCurrentColor = Color.Gold;
        }

        void ActivateManicMode()
        {
            CheatSound.Play();
            cheatText = "Manic mode activated";
            cheatTextCurrentColor = Color.Gold;
        }

        void ActivateHardcoreMode()
        {
            CheatSound.Play();
            cheatText = "Hardcore mode activated";
            cheatTextCurrentColor = Color.Gold;
        }

        private void FadeComplete()
        {
            //Game.ChangeScene(new TitleScene(Game));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TitleScreenTexture, Vector2.Zero, null, backgroundCurrentColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            spriteBatch.Draw(TitleTextJapanese, Vector2.Zero, null, text1CurrentColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, .9f);
            spriteBatch.Draw(TitleTextEnglish, Vector2.Zero, null, text2CurrentColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, .8f);

            if(!HelpShown && !CreditsShown)
                spriteBatch.DrawString(titlescreenFont, "Press ENTER to begin", new Vector2((925f / 2f) - (titlescreenFont.MeasureString("Press ENTER to begin").X / 2f), 500), startTextColor);

            if (fadephase == 2 && !HelpShown && !CreditsShown)
            {
                spriteBatch.DrawString(titlescreenFont, "Press C for credits", new Vector2((925f) - (titlescreenFont.MeasureString("Press C for credits").X + 30), 605), Color.Gray);
                spriteBatch.DrawString(titlescreenFont, "Press H for help", new Vector2((925f) - (titlescreenFont.MeasureString("Press H for help").X + 30), 620), Color.Gray);
            }

            if (HelpShown)
            {
                spriteBatch.Draw(HelpTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (CreditsShown)
            {
                spriteBatch.Draw(CreditsTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            
            
            spriteBatch.DrawString(titlescreenFont, cheatText, new Vector2((925f / 2f) - (titlescreenFont.MeasureString(cheatText).X / 2f), 600), cheatTextCurrentColor);
        }

        public override void Unload()
        {
            // Nothing!
        }

        public override void LoadContent(ContentManager content)
        {
            if (TitleScreenTexture == null)
                TitleScreenTexture = content.Load<Texture2D>("Graphics/GUI/TitleScreen");
            if (TitleTextJapanese == null)
                TitleTextJapanese = content.Load<Texture2D>("Graphics/GUI/TitleTextJapanese");
            if (TitleTextEnglish == null)
                TitleTextEnglish = content.Load<Texture2D>("Graphics/GUI/TitleTextEnglish");
            if (titlescreenFont == null)
                titlescreenFont = content.Load<SpriteFont>("Fonts/ScoreFont");
            if (TitleTheme == null)
                TitleTheme = content.Load<SoundEffect>("Audio/Music/AsteroidRebuttalTitleVersion");
            if (CheatSound == null)
                CheatSound = content.Load<SoundEffect>("Audio/SFX/Extend");
            if (GameStartSound == null)
                GameStartSound = content.Load<SoundEffect>("Audio/SFX/gamestart");
            if (CreditsTexture == null)
                CreditsTexture = content.Load<Texture2D>("Graphics/GUI/Credits");
            if (HelpTexture == null)
                HelpTexture = content.Load<Texture2D>("Graphics/GUI/Help");
        }
    }
}
