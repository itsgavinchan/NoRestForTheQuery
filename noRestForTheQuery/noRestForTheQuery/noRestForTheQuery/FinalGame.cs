using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace noRestForTheQuery
{
    public class FinalGame : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public const int WINDOW_WIDTH = 1200;
        public const int WINDOW_HEIGHT = 600;
        public const float GRAVITY = 0.95F;
        const double HOMEWORKSPAWNPROB = 0.10 / 6000 / 6000;
        const int MAXHOMEWORK = 5;
        const int MAXLEVELS = 2;
        const int INVUL_TIME = 1500;
        const int BLINK_TIME = 100;
        int mouseX, mouseY;

        enum ScreenStatus { START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY };
        int currentStatus = (int) ScreenStatus.START;

        // START SCREEN
        const string gameTitle = "No Rest For The Query";
        const string continueMessage = "Press Space To Continue";
        Vector2 gameTitlePos, continueMessagePos;

        // INTRODUCTION SCREEN
        string[] introMessage = new string[] { 
            "Congratulations. Starting today, you're a CS student at NYU-Poly.",
            "In the incoming years, become the masochistic programmer you are ",
            "paying tuition to be and get ready to sweat blood. Good luck, young",
            "programmer. May you survive this hurdle intact and graduate (on time).",
            ""
            };

        // GAME OVER SCREEN
        const string gameOverMessage = "Game Over";
        const string gameOverContMessage = "Press R to Restart The Game";
        Vector2 gameOverPos, gameOverContPos;
        
        // WEEKEND STANDBY SCREEN
        // You may only choose three choices of the total of five options - SLEEP, STUDY, FOOD, STORE, RELAX
        const int numOfWeekendOptions = 5, weekendMargin = 50, maxChoices = 3;
        enum WeekendChoices { SLEEP, STUDY, FOOD, STORE, RELAX };

        List<int> chosenChoices = new List<int>(3);
        Texture2D[] weekendIcons = new Texture2D[numOfWeekendOptions];
        List<Vector2> weekendPositions = new List<Vector2>();

        // STORE Option Specific
        public static int PENCILCOST = 5;
        public static int SLEEPINCREMENT = 50;

        // BATTLE/WEEKDAY SCREEN
        // GAME TOOLS
        public static int defaultBlockSize = 50;
        public static int gameLevel = 1;
        public static Random rand = new Random();
        string currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";

        // CAMERA
        public static int screenOffset = 0, tempScreenOffset;
        public static Matrix translation = Matrix.Identity;

        // STUDENT
        Student1 student;
        int studentWidth = 50;
        int studentHeight = 50;
        AnimatedSprite studentAnimation;
        int hitRecoilTime = INVUL_TIME;
        int blinkDuration = BLINK_TIME;
        bool lostHealth = false; //Needed in order to decrement health only once after being hit.

        // PROFESSORS
        List<Professor> professors = new List<Professor>();

        // PLATFORMS
        List<Platform> platforms = new List<Platform>();
        
        // HOMEWORKS
        List<Homework> homeworks = new List<Homework>();

        // DISPLAY Variables
        SpriteFont mainFont;
        public static Texture2D platformSprite, studentSprite, pencilSprite, markerSprite, homeworkSprite, midtermSprite, finalSprite, notebookSprite, professorSprite;
        
        // INPUT States
        KeyboardState lastKeyState = Keyboard.GetState();
        MouseState lastMouseState = Mouse.GetState();

        public FinalGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            IsMouseVisible = true;
        }
        protected override void Initialize() { base.Initialize(); }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>(@"Fonts/MainFont");

            studentSprite = Content.Load<Texture2D>(@"Sprites/player");
            platformSprite = Content.Load<Texture2D>(@"Sprites/platform");
            pencilSprite = Content.Load<Texture2D>(@"Sprites/pencil");
            markerSprite = Content.Load<Texture2D>(@"Sprites/marker");
            homeworkSprite = Content.Load<Texture2D>(@"Sprites/homework");
            midtermSprite = Content.Load<Texture2D>(@"Sprites/midterm");
            finalSprite = Content.Load<Texture2D>(@"Sprites/final");
            notebookSprite = Content.Load<Texture2D>(@"Sprites/notebook");
            professorSprite = Content.Load<Texture2D>(@"Sprites/professor");
            
            // Temporary Implmentation - UI Will Change
            weekendIcons[(int)WeekendChoices.SLEEP] = Content.Load<Texture2D>(@"Sprites/menuItem01");
            weekendIcons[(int)WeekendChoices.FOOD] = Content.Load<Texture2D>(@"Sprites/menuItem01");
            weekendIcons[(int)WeekendChoices.STUDY] = Content.Load<Texture2D>(@"Sprites/menuItem01");
            weekendIcons[(int)WeekendChoices.STORE] = Content.Load<Texture2D>(@"Sprites/menuItem01");
            weekendIcons[(int)WeekendChoices.RELAX] = Content.Load<Texture2D>(@"Sprites/menuItem01");
            
            gameTitlePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameTitle).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameTitle).Y / 2 - 25);
            continueMessagePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(continueMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(continueMessage).Y / 2 + 25);
            gameOverPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverMessage).Y / 2 - 25);
            gameOverContPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverContMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverContMessage).Y / 2 + 25);
            
            // Temporary Signal for Selection of Options
            int sum = 0;
            for (int i = 0; i < weekendIcons.Count(); i++) { sum += weekendIcons[i].Width; }
            float itemMargin = ( (WINDOW_WIDTH - weekendMargin * 2) - sum) / weekendIcons.Count();
            for( int i = 0; i < weekendIcons.Count(); i++ ) {
                weekendPositions.Add(new Vector2(weekendMargin + itemMargin + i * (weekendIcons[i].Width + itemMargin), (WINDOW_HEIGHT - weekendIcons[i].Height) / 2));
            }

            // Animation sheet
            studentAnimation = new AnimatedSprite( Content.Load<Texture2D>(@"Sprites/simplePlayerSheet"), 
                                                   5, studentWidth, studentHeight );

            // Student starts in the top center of the screen for testing. Otherwise, it follows the level plan
            student = new Student1( ref studentAnimation,
                                    new Vector2(WINDOW_WIDTH / 2 - studentWidth / 2, 200 ), // Position
                                    new Vector2(studentWidth / 2, studentHeight / 2),       // Origin
                                    Vector2.Zero, 3.5F); 
            student.colorArr = new Color[ studentAnimation.Texture.Width * studentAnimation.Texture.Height ];
            studentAnimation.Texture.GetData<Color>( 0, studentAnimation.SourceRect, student.colorArr,
                                                     student.sprite.currentFrame*studentWidth, 
                                                     studentWidth * studentHeight );
            

            // INITIATE - Professors of All Levels; Temporary Implentation For Testing
            for (int i = 0; i < MAXLEVELS; i++) {
                professors.Add(new Professor(   (i+1) * 50,
                                                new Vector2(WINDOW_WIDTH + screenOffset + professorSprite.Width, (WINDOW_HEIGHT - professorSprite.Height) / 2),
                                                new Vector2(professorSprite.Width / 2, professorSprite.Height / 2),
                                                Vector2.Zero,
                                                1));
                professors[i].colorArr = new Color[ professorSprite.Width * professorSprite.Height ];
                professorSprite.GetData<Color>(professors[i].colorArr);
            }

            //Build the first level
            buildLevel( ref platforms, ref student );

        }
        protected override void UnloadContent() { }
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            mouseX = Mouse.GetState().X + screenOffset; 
            mouseY = Mouse.GetState().Y;

            // Implements the different screen stages -  START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY

            // START SCREEN - Saves Not Implemented Yet
            // Pressing Space will proceed to Introduction Screen
            if (IsActive && currentStatus == (int)ScreenStatus.START){
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.INTRODUCTION; }
            }
            // INTRODUCTION SCREEN - Explains the Story and Rules of Game
            // Pressing Space will proceed to the Start of Game - Weekday Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.INTRODUCTION) {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.WEEKDAY; }
            }
            // PAUSE SCREEN - Provides option to pause the screen; may implement SAVE, QUIT GAME, RESTART LEVEL in future iterations
            // Press Backspace to resume the game in Weekday Stage (Pause Stage can only be accessed from Weekday Stage)
            else if (IsActive && currentStatus == (int)ScreenStatus.PAUSE) {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.WEEKEND; }
            }
            // STATUS SCREEN - Displays the status of the player; not yet implemented; a testing screen to be removed at final implementation
            // Pressing Space will proceed back to the Weekend Stage (Status Stage can only be accessed from Weekend Stage)
            else if (IsActive && currentStatus == (int)ScreenStatus.STATUS) {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.WEEKEND; }
            }
            // GAME OVER SCREEN
            // Pressing R Key will restart the game from the beginning - Start Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.GAMEOVER) {
                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); currentStatus = (int)ScreenStatus.START; }
            }
            // WEEKEND SCREEN
            // Left-Clicking will allow you to choose three of the five options - SLEEP, STUDY, FOOD, STORE, RELAX=
            // Pressing Enter will execute your choices and provide you the boosts, but only if you select the maximum amount of choices
                // If you attempt to choose one more than the maximum amount of allowed choices, the first choice will be removed and 
                // replaced with the latest choice. This will not give the player an option to choose less or more than the maximum amount 
                // of allowed choices.
            // Pressing Space will lead you to the Status Screen
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKEND) {
                if (chosenChoices.Count() == maxChoices && Keyboard.GetState().IsKeyDown(Keys.Enter) && lastKeyState.IsKeyUp(Keys.Enter)) {
                    for (int i = 0; i < chosenChoices.Count(); i++) {
                        if (chosenChoices[i] == (int)WeekendChoices.SLEEP) { }
                        else if (chosenChoices[i] == (int)WeekendChoices.STUDY) { }
                        else if (chosenChoices[i] == (int)WeekendChoices.FOOD) { }
                        else if (chosenChoices[i] == (int)WeekendChoices.STORE) { }
                        else if (chosenChoices[i] == (int)WeekendChoices.RELAX) { }
                    }
                    currentStatus = (int)ScreenStatus.WEEKDAY; 
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && lastKeyState.IsKeyUp(Keys.Space)) { currentStatus = (int)ScreenStatus.STATUS; }

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) {
                    for (int i = 0; i < weekendIcons.Count() && i < weekendPositions.Count(); i++) {
                        if (checkMouseOverlap(weekendPositions[i], ref weekendIcons[i]) && !chosenChoices.Contains(i) ) { 
                            if (chosenChoices.Count() >= maxChoices) { chosenChoices.RemoveAt(0); chosenChoices.Add(i); }
                            else { chosenChoices.Add(i); }
                        }
                    }
                }
            }
            // WEEKDAY SCREEN - Where the Action Happens and You Die (A Lot); Currently In Testing
            // While the game is active and in the Weekday Stage, homework will continually be spawned
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKDAY) {

                if (Keyboard.GetState().IsKeyDown(Keys.R)) { reset(); }

                // Randomly Spawns Homework - Temporary Implementation
                if (homeworks.Count() < MAXHOMEWORK && rand.NextDouble() > HOMEWORKSPAWNPROB) {
                    homeworks.Add(new Homework(new Vector2(WINDOW_WIDTH + screenOffset, rand.Next(0, WINDOW_HEIGHT - homeworkSprite.Height)),
                                                    new Vector2(homeworkSprite.Width / 2, homeworkSprite.Height / 2),
                                                    Vector2.Zero));
                }

                // TEST - Initiate GAMEOVER Stage; CHECK DEATH - Student dies if goes off-screen to the left or jumps off a platform
                if (Keyboard.GetState().IsKeyDown(Keys.Q) || (student.isAlive && (student.position.X < screenOffset - studentSprite.Width * 2 || student.position.Y > WINDOW_HEIGHT + studentSprite.Height))) {
                    gameOverPos = new Vector2(gameOverPos.X + screenOffset, gameOverPos.Y);
                    gameOverContPos = new Vector2(gameOverContPos.X + screenOffset, gameOverContPos.Y); 
                    student.isAlive = false; 
                    currentStatus = (int)ScreenStatus.GAMEOVER; 
                }

                // TEST - Initiate WEEKEND Stage
                if (Keyboard.GetState().IsKeyDown(Keys.U)) {
                    // Readjusts the positions of the weekend icons (Temporary Implementation - UI will change)
                    for (int i = 0; i < weekendPositions.Count(); i++) {
                        weekendPositions[i] = new Vector2(weekendPositions[i].X + screenOffset, weekendPositions[i].Y);
                    }

                    // Transitions to WEEKEND Stage
                    currentStatus = (int)ScreenStatus.WEEKEND;
                }

                // PLAYER CONTROL - Jump
                if (Keyboard.GetState().IsKeyDown(Keys.W) && lastKeyState.IsKeyUp(Keys.W) && !student.jumping && student.onGround) { student.jump(); }

                // PLAYER CONTROL - Move Left
                if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                    if (!student.checkBoundaries()) { student.velocity.X = -student.speed; }
                    student.sprite.animateLeft(Keyboard.GetState(), lastKeyState, gameTime);
                }

                // PLAYER CONTROL - Move Right
                if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                    if (!student.checkBoundaries()) { student.velocity.X = student.speed; }
                    student.sprite.animateRight(Keyboard.GetState(), lastKeyState, gameTime);
                }

                // PLAYER CONTROL - Attack with Ammo (Pencils)
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) {
                    double x = (mouseX - (student.position.X + student.sprite.width / 2));
                    double y = (mouseY - (student.position.Y + student.sprite.height / 2));
                    student.shoot((float)(Math.Atan2(y, x)));
                }

                // TEST - Professor Appearance
                if (Keyboard.GetState().IsKeyDown(Keys.P) && lastKeyState.IsKeyUp(Keys.P)) {
                    if (!professors[gameLevel - 1].isAlive) { professors[gameLevel - 1].isAlive = !professors[gameLevel - 1].isAlive; }
                    else { professors[gameLevel - 1].reset(); }
                }

                // TEST - Professor Attack
                if (Keyboard.GetState().IsKeyDown(Keys.L) && lastKeyState.IsKeyUp(Keys.L)) {
                    if (professors[gameLevel - 1].isAlive) {
                        double x = ((student.position.X + studentSprite.Width / 2) - (professors[gameLevel - 1].position.X + professorSprite.Width / 2));
                        double y = ((student.position.Y + studentSprite.Height / 2) - (professors[gameLevel - 1].position.Y + professorSprite.Height / 2));
                        professors[gameLevel - 1].shoot(x, y);
                    }
                }

                // TEST - Progression to Next Level
                if (Keyboard.GetState().IsKeyDown(Keys.N) && lastKeyState.IsKeyUp(Keys.N)) {
                    reset();
                    if (gameLevel < MAXLEVELS) gameLevel++;
                    currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";
                    buildLevel(ref platforms, ref student);
                }

                // TEST - Controls for Camera Movement
                if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                    translation *= Matrix.CreateTranslation(new Vector3(1, 0, 0));
                    screenOffset -= 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                    translation *= Matrix.CreateTranslation(new Vector3(-1, 0, 0));
                    screenOffset += 1;
                }

                // POSITION UPDATE - Student
                student.update();

                int index = 0;
                // POSITION UPDATE - Student's Ammo (Pencils)
                index = 0;
                while (index < student.pencils.Count()) {
                    student.pencils[index].update();
                    if (student.pencils[index].checkBoundaries(pencilSprite.Width, pencilSprite.Height)) { student.pencils.RemoveAt(index); }
                    else { index++; }
                }

                // POSITION UPDATE - Professor and Their Ammo (Markers)
                //if (professors[gameLevel - 1].isAlive) { 
                index = 0;
                professors[gameLevel - 1].update();
                while (index < professors[gameLevel - 1].markers.Count()) {
                    professors[gameLevel - 1].markers[index].update(student.position.X + studentSprite.Width / 2, student.position.Y + studentSprite.Height / 2);
                    if (professors[gameLevel - 1].markers[index].checkBoundaries(markerSprite.Width, markerSprite.Height)) { professors[gameLevel - 1].markers.RemoveAt(index); }
                    else { index++; }
                }
                //}

                // POSITION UPDATE - Homeworks
                index = 0;
                while (index < homeworks.Count()) {
                    homeworks[index].update();
                    if (homeworks[index].checkBoundaries(homeworkSprite.Width, homeworkSprite.Height)) { homeworks.RemoveAt(index); }
                    else { index++; }
                }

                // POSITION UPDATE - Camera
                translation *= Matrix.CreateTranslation(new Vector3(-1, 0, 0));
                screenOffset += 1;
                
                //COLLISION UPDATE - Check if student hit by marker
                index = 0;
                while( index < professors[gameLevel-1].markers.Count() ){
                    student.handleCollision( professors[gameLevel-1].markers[index], markerSprite.Width, markerSprite.Height, 
                                             studentSprite.Width, studentSprite.Height );
                    if( student.hit ){ break; }
                    index++;
                }
                if( student.hit ){
                    hitRecoilTime -= gameTime.ElapsedGameTime.Milliseconds;
                    if( !lostHealth ){ student.decrementHealth( professors[gameLevel-1].attackPower ); lostHealth = true; }
                    if( hitRecoilTime < 0 ){
                        hitRecoilTime = INVUL_TIME;
                        student.hit = false;
                        lostHealth = false;
                    }
                }


                // BOOKEEPING
                handleSpriteMovement(ref student.sprite);
                handleStudentPlatformCollision();                               //Handle student/platform collision

                if (student.velocity.Y != 0) { student.onGround = false; }      //Check if on ground
                if (student.onGround) { student.jumping = false; }              //Reset jump state

                if (lastKeyState.IsKeyUp(Keys.Left) || lastKeyState.IsKeyUp(Keys.Right)) { student.velocity.X = 0; }
                //if (Keyboard.GetState().GetPressedKeys().Length == 0 ) { resetCurrentAnimation( student.sprite ); }
            }

            lastKeyState = Keyboard.GetState();
            lastMouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, translation);

            // Displays the different screen stages - START, INTRODUCTION, PAUSE, STATUS, GAMEOVER, WEEKEND, WEEKDAY

            // START SCREEN
            // Displays the Game Title and Continue Message
            if (IsActive && currentStatus == (int)ScreenStatus.START) {
                spriteBatch.DrawString(mainFont, gameTitle, gameTitlePos, Color.White);
                spriteBatch.DrawString(mainFont, continueMessage, continueMessagePos, Color.White);
            }

            // INTRODUCTION SCREEN
            // Displays the Welcoming Message to Greet the Player as well as brief them on the controls
            else if (IsActive && currentStatus == (int)ScreenStatus.INTRODUCTION) {
                for (int i = 0; i < introMessage.Count(); i++) {
                    spriteBatch.DrawString(mainFont, introMessage[i], new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(introMessage[i]).X / 2, i * 25 + 100), Color.White);
                }
            }

            // PAUSE SCREEN - In development
            else if (IsActive && currentStatus == (int)ScreenStatus.PAUSE) { }

            // STATUS SCREEN - In development; May Not Implement (For Testing Purposes)
            else if (IsActive && currentStatus == (int)ScreenStatus.STATUS) { }

            // GAMEOVER SCREEN
            else if (IsActive && currentStatus == (int)ScreenStatus.GAMEOVER) {
                spriteBatch.DrawString(mainFont, gameOverMessage, gameOverPos, Color.White);
                spriteBatch.DrawString(mainFont, gameOverContMessage, gameOverContPos, Color.White);
            }

            // WEEKEND SCREEN
            // Temporary implementation; actual UI will be different. Selected choices will be brown and unselected choices will be red. 
            // Hovering over a choice will show the choice as black. 
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKEND) {
                for (int i = 0; i < weekendIcons.Count() && i < weekendPositions.Count(); i++) {
                    if (checkMouseOverlap(weekendPositions[i], ref weekendIcons[i]) && !chosenChoices.Contains(i)) {
                        spriteBatch.Draw(weekendIcons[i], weekendPositions[i], Color.Black);
                    }
                    else if (chosenChoices.Contains(i)) { spriteBatch.Draw(weekendIcons[i], weekendPositions[i], Color.Brown); }
                    else { spriteBatch.Draw(weekendIcons[i], weekendPositions[i], Color.Crimson); }
                }
            }
            // WEEKDAY SCREEN
            else if (IsActive && currentStatus == (int)ScreenStatus.WEEKDAY) {

                // Platform Display
                for (int i = 0; i < platforms.Count; ++i) { spriteBatch.Draw(platformSprite, platforms[i].position, platforms[i].rectangle, Color.Black); }
                
                // Spawned Homework Display
                for (int i = 0; i < homeworks.Count(); i++) { spriteBatch.Draw(homeworkSprite, homeworks[i].position, Color.Orange); }
                
                // Student Display
                //spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.Red);

                // Notebook Shield Display
                if (student.notebook.isAlive) { spriteBatch.Draw(notebookSprite, student.notebook.position, null, Color.White, student.notebook.rotation, student.notebook.origin, 1.0F, SpriteEffects.None, 0.0F); }
            
                // Ammo (Pencil) Display
                for (int i = 0; i < student.pencils.Count(); i++) { spriteBatch.Draw(pencilSprite, student.pencils[i].position, null, Color.White, student.pencils[i].rotation, student.pencils[i].origin, 1.0F, SpriteEffects.None, 0.0F); }
                
                // Professor Display and their Markers
                if (professors[gameLevel - 1].isAlive) {
                    spriteBatch.Draw(homeworkSprite, professors[gameLevel - 1].position, Color.Blue);
                    for (int i = 0; i < professors[gameLevel - 1].markers.Count(); i++) {
                        spriteBatch.Draw(markerSprite, professors[gameLevel - 1].markers[i].position, null, Color.White, professors[gameLevel - 1].markers[i].rotation, professors[gameLevel - 1].markers[i].origin, 1.0F, SpriteEffects.None, 0.0F);
                    }
                }
                //Draw the student
                if (!student.hit)
                    spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.Red);
                else
                {
                    blinkDuration -= gameTime.ElapsedGameTime.Milliseconds;
                    if (blinkDuration < 0)
                    {
                        spriteBatch.Draw(student.sprite.Texture, student.position, student.sprite.SourceRect, Color.Red);
                        blinkDuration = BLINK_TIME;
                    }
                }
                Vector2 healthPos = new Vector2( screenOffset, 0 );
                spriteBatch.DrawString( mainFont, "Health: "+ student.currentHealth, healthPos, Color.White);
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool checkMouseOverlap(Vector2 position, ref Texture2D sprite) {
            if (mouseX > position.X && mouseX < position.X + sprite.Width && mouseY > position.Y && mouseY < position.Y + sprite.Height) return true;
            else { return false; }
        }
        public bool purchasePencils(int quantity) {
            if (quantity * PENCILCOST > student.budget) { return false; }
            else { student.amtPencil += quantity; student.budget -= quantity * PENCILCOST; return true; }
        }
        protected void handleStudentPlatformCollision() {
            int interLeft, interRight, interTop, interBot, interWidth, interHeight;
            for (int i = 0; i < platforms.Count; ++i) {
                interLeft = Math.Max((int)student.position.X, platforms[i].rectangle.Left);
                interTop = Math.Max((int)student.position.Y, platforms[i].rectangle.Top);
                interRight = Math.Min((int)student.position.X + studentSprite.Width, platforms[i].rectangle.Right);
                interBot = Math.Min((int)student.position.Y + studentSprite.Height, platforms[i].rectangle.Bottom);
                interWidth = interRight - interLeft;
                interHeight = (interBot - interTop);

                if (interWidth >= 0 && interHeight >= 0) {  //If the intersecting rect is valid, they hit!
               
                    student.colliding = true;

                    //Movement collision on ground
                    if (interHeight > interWidth) {
                        //If student going to the right, stop them at the left edge of the platform
                        if (student.velocity.X > 0) { student.position.X -= interWidth; }

                        //If going to the left
                        if (student.velocity.X < 0) { student.position.X += interWidth; }

                        //They collided, so stop moving
                        student.velocity.X = 0;
                    }

                    //Vertical movement collision (Adjustments added compensate for odd corner cases)
                    if (interWidth > interHeight-Math.Abs(student.velocity.Y) && interWidth > student.speed+1) {
                        //If student is falling, stop them at the top edge of the platform (Make sure it's above the platform)
                        if (student.velocity.Y > 0 && student.position.Y < platforms[i].position.Y) {
                            student.position.Y -= interHeight; 
                            student.onGround = true;
                            student.velocity.Y = 0;
                        }

                        //If student is going upward from a jump, put them down (w/ a little force)
                        if (student.velocity.Y < 0) { 
                            student.position.Y += interHeight; 
                            student.velocity.Y *= -.1F; 
                        }
                    }
                }
                else { student.colliding = false; }
            }
        }
        protected void reset() {
            // Reset The Objects (Or Clear the Lists)
            homeworks.Clear();
            professors[gameLevel - 1].reset();
            student.reset();

            // Reset The Game Mechaics
            gameLevel = 1;
            currentLevelFile = "../../../Layouts/level" + gameLevel + ".txt";
            buildLevel(ref platforms, ref student); 
            screenOffset = 0;
            translation = Matrix.Identity;

            // Reset the Positions
            gameTitlePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameTitle).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameTitle).Y / 2 - 25);
            continueMessagePos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(continueMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(continueMessage).Y / 2 + 25);
            gameOverPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverMessage).Y / 2 - 25);
            gameOverContPos = new Vector2(WINDOW_WIDTH / 2 - mainFont.MeasureString(gameOverContMessage).X / 2, WINDOW_HEIGHT / 2 - mainFont.MeasureString(gameOverContMessage).Y / 2 + 25);
            
        }
        private void buildLevel( ref List<Platform> platforms, ref Student1 student )
        {
            platforms.Clear();
            int x = 0; int y = 0;
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader( currentLevelFile );
            
            while( (line = file.ReadLine()) != null ){
                char[] levelRow = line.ToCharArray();
                foreach( char symbol in levelRow ){
                    if (symbol == '.' || symbol == ' ') { x += defaultBlockSize; }
                    if( symbol == 's' ){ student.position = new Vector2( x, y ); }
                    if( symbol == 'x' ){ 
                        platforms.Add( new Platform( new Vector2( x, y ), Vector2.Zero, 0 ) );
                        x += defaultBlockSize;
                    }
                }
                x = 0;
                y += defaultBlockSize;
            }
        }
        public void handleSpriteMovement( ref AnimatedSprite sprite ) {
            //Update the current sprite being taken from the sprite sheet
            sprite.SourceRect = new Rectangle(sprite.currentFrame * sprite.width, 0, sprite.width, sprite.height);

            //Keep the animation loop
            if (Keyboard.GetState().GetPressedKeys().Length == 0) {
                if (sprite.currentFrame > 0 && sprite.currentFrame < 4) {
                    sprite.currentFrame = 0;
                }
                if (sprite.currentFrame > 4 && sprite.currentFrame < 8) {
                    sprite.currentFrame = 4;
                }
            }
        }
    }
}
