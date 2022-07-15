using GRM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public static IList<Player> players = new List<Player>() {
                    new Player(){ Position=1, Name="John", Score = 0, PlayedAll = false},
                    new Player(){ Position=2, Name="Bob", Score = 0, PlayedAll = false},
                    new Player(){ Position=3, Name="Mike", Score = 0, PlayedAll = false},
                    new Player(){ Position=4, Name="Tom", Score = 0, PlayedAll = false},
                    new Player(){ Position=5, Name="Brian", Score = 0, PlayedAll = false},
                    new Player(){ Position=6, Name="Adam", Score = 0, PlayedAll = false},
                };


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            ViewBag.ShowScoring = false;
        }
        public IEnumerable<Player> GetRandomTwo()
        {

            List<Player> playersList = new List<Player>(players);
            var gameDone = 0;
            foreach (var item in playersList)
            {
                if (item.PlayedAll)
                    gameDone++;    
            }

            if (gameDone == 6)
                return null;

            Player firstPlayer = GetRandomPlayer(playersList);

            firstPlayer.PlayedAll = firstPlayer.PlayedWith.Count() == 5;

            while(firstPlayer.PlayedAll)
            {
          
                firstPlayer = GetRandomPlayer(playersList);
                firstPlayer.PlayedAll = firstPlayer.PlayedWith.Count() == 5;
            }

            playersList.Remove(firstPlayer);

            Player secondPlayer = GetRandomPlayer(playersList);

            while (CheckIfPlayed(firstPlayer, secondPlayer))
            {
                secondPlayer = GetRandomPlayer(playersList);
            };

            secondPlayer.PlayedAll = secondPlayer.PlayedWith.Count() == 5;


            if (!CheckIfPlayed(firstPlayer, secondPlayer)) { 
                firstPlayer.PlayedWith.Add(secondPlayer);
                secondPlayer.PlayedWith.Add(firstPlayer);
            }


            playersList.Clear();

            IEnumerable<Player> TwoRandomPlayers = new[] { firstPlayer, secondPlayer };

            if (firstPlayer == null && secondPlayer == null)
                TwoRandomPlayers = null;

                
            return TwoRandomPlayers;
        }

        private static bool CheckIfPlayed(Player first, Player second)
        {
            return first.PlayedWith.Contains(second) || second.PlayedWith.Contains(first);
        }

        private static Player GetRandomPlayer(List<Player> playersList)
        {

            var random = new Random();

            var player = playersList.OrderBy(u => random.Next()).Take(1).First();

            return player;
        }

        public IActionResult Index()
        {
            ViewBag.ShowScoring = false;
            ViewBag.ShowList = false;


            return View();
        }

        [HttpPost]
        public IActionResult Start()
        {
            ViewBag.ShowScoring = true;
            ViewBag.ShowList = false;


            ViewData["CurrentPlayers"] = GetRandomTwo();
            ViewData["AllPlayers"] = players;

            return View("Index");
        }

        [HttpPost]
        public ActionResult CalculateScore(string firstPlayer, int firstScore, int secondScore, string secondPlayer)
        {

            ViewBag.ShowScoring = true;
            ViewBag.ShowList = true;

            var currentPlayers = GetRandomTwo();

            if (currentPlayers == null)
                return RedirectToAction("GameOver");

            ViewData["CurrentPlayers"] = currentPlayers;
            ViewData["AllPlayers"] = players;

            if (firstScore == secondScore)
            {
                ViewBag.ShowList = false;
                return View("Index");
            }



            /*
              After the submit event is triggered, the list item with the higher value will have its Score attribute
              value increased by one.

              After the submit event, the user will be presented with two new list items that will be compared and
              the list will be sorted by the value of the Score attribute. This will continue until every item is 
              compared with every other item in the list.
              */

            //firstScore > secondScore? players.Where(p => p.Name == firstPlayer).Select(p => p.Score++) : players.Where(p => p.Name == secondPlayer).Select(p => p.Score++);

            var player = firstScore > secondScore ? firstPlayer : secondPlayer;

            players.Where(c => c.Name == player).Select(c => ++c.Score).ToList();

            ViewData["AllPlayers"] = players.OrderByDescending(player => player.Score).ToList();


            return View("Index");
        }

        public IActionResult GameOver()
        {
            ViewData["AllPlayers"] = players.OrderByDescending(player => player.Score).ToList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}