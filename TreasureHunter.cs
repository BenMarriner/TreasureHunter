using SplashKitSDK;
using System;
using System.IO;

namespace TreasureHunter
{
    class TreasureHunter
    {   
        static int DisplayMenu(String menuText, int numberOfMenuItems)
        {
            Console.Clear();
            return ConsoleInputValidation.ReadInteger(menuText, 1, numberOfMenuItems);
        }

        static int DisplayMenu(String message, String[] menuItems)
        {
            Console.Clear();

            for (int i = 0; i < menuItems.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + menuItems[i]);
            }
            Console.WriteLine(menuItems.Length + 1 + ". Back");

            return ConsoleInputValidation.ReadInteger(message, 0, menuItems.Length + 1);
        }

        static String DisplayLevelList()
        {
            String[] levelFileNames = FileManagement.GetLevelList();
            String[] levelNames = new String[levelFileNames.Length];
            Array.Copy(levelFileNames, levelNames, levelFileNames.Length);
            int levelSelection;

            // Remove file extensions before displaying in the terminal
            for (int i = 0; i < levelNames.Length; i++)
            {
                levelNames[i] = levelNames[i].Remove(levelNames[i].Length - 5, 5);
            }

            Console.Clear();
            levelSelection = DisplayMenu("Enter the number of the level you want to select:", levelNames);

            if (levelSelection == levelFileNames.Length + 1)
            {
                // Implies that the player has decided selected the back option and therefore not a level
                return null;
            }

            return levelFileNames[levelSelection - 1];
        }

        static void DisplayEditorMenu()
        {
            int menuOption;
            do
            {
                Editor editor;
                menuOption = DisplayMenu
                                ("Would you like to edit an existing level or begin creating a new one?" +
                                "\n1. Create New Level" +
                                "\n2. Open Existing Level" +
                                "\n3. Back",
                                3);

                switch (menuOption)
                {
                    case 1:
                        // Create new level
                        editor = new Editor(ConsoleInputValidation.ReadString("Enter a name for your new level:"));
                        editor.WindowOpen();
                        editor.Update();
                        editor.WindowClose();
                        break;
                    case 2:
                        // Display Level List and then open level editor with chosen level
                        Console.WriteLine("Retrieving levels. Please wait...");
                        String levelSelection = DisplayLevelList();
                        if (levelSelection != null)
                        {
                            Level level = FileManagement.LoadLevel(levelSelection);
                            if (level != null)
                            {
                                editor = new Editor(level);
                                editor.WindowOpen();
                                editor.Update();
                                editor.WindowClose();
                            }
                            else
                            {
                                ConsoleInputValidation.ReadInteger
                                ("This level failed to load. The file contents could not be read" +
                                "\n Please type 3 to go back and select another level", 3, 3);
                            }
                        }
                        break;
                    case 3:
                        break;
                }
            }
            while (menuOption != 3);
        }

        static void Main(string[] args)
        {
            Game game;
            int menuOption;
            FileManagement.LoadResources();

            do
            {
                Console.Clear();

                menuOption = DisplayMenu
                ("Welcome to Treasure Hunter. Would you like to play a level or make your own?" +
                "\nEnter the number of your choice:" +
                "\n1. Play Game" +
                "\n2. Open Level Editor" +
                "\n3. Quit Treasure Hunter",
                3);

                switch (menuOption)
                {
                    case 1:
                        GlobalSettings.ProgramMode = 1;
                        String levelSelection = DisplayLevelList();
                        if (levelSelection != null)
                        {
                            Level level = FileManagement.LoadLevel(levelSelection);
                            if (level != null)   
                            {
                                game = new Game(level);
                                game.WindowOpen();
                                game.Update();
                                game.WindowClose();
                            }
                            else
                            {
                                ConsoleInputValidation.ReadInteger
                                ("This level failed to load. The file contents could not be read" +
                                "\n Please type 3 to go back and select another level", 3, 3);
                            }     
                        }
                        break;
                    case 2:
                        GlobalSettings.ProgramMode = 0;
                        // Display Editor Menu
                        DisplayEditorMenu();
                        break;
                    case 3:
                        // Quit
                        break;
                }
            }
            while (menuOption != 3);
        }
    }
}
